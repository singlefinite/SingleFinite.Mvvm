// MIT License
// Copyright (c) 2026 Single Finite
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using SingleFinite.Essentials;
using SingleFinite.Mvvm.Services;
using SingleFinite.Mvvm.Services.Presenters;

namespace SingleFinite.Mvvm.Internal.Services.Presenters;

/// <summary>
/// Implementation of <see cref="IListPresenter"/>.
/// </summary>
internal class ListPresenter : IListPresenter, IDisposable
{
    #region Fields

    /// <summary>
    /// Holds the dispose state for this object.
    /// </summary>
    private readonly DisposeState _disposeState;

    /// <summary>
    /// Holds the underlying list of views.
    /// </summary>
    private readonly List<IView> _views = [];

    /// <summary>
    /// Holds view provider used to provide views.
    /// </summary>
    private readonly IViewProvider _viewProvider;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="viewProvider">Used to provide views.</param>
    /// <param name="viewModelNode">
    /// Used to observe when a parent IsActive value changes.
    /// </param>
    public ListPresenter(
        IViewProvider viewProvider,
        ViewModelNode viewModelNode
    )
    {
        _viewProvider = viewProvider;
        _disposeState = new(
            owner: this,
            onDispose: Clear
        );

        IsActive = viewModelNode.IsActiveFromRoot;
        viewModelNode.IsActiveFromRootChanged
            .Observe()
            .OnEach(isActiveFromRoot => IsActive = isActiveFromRoot)
            .Until(_disposeState.CancellationToken);
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public int CurrentIndex { get; private set; } = -1;

    /// <inheritdoc/>
    public IViewModel[] ViewModels { get; private set; } = [];

    /// <inheritdoc/>
    public IView? Current { get; private set; }

    /// <inheritdoc/>
    public IDictionary<string, object?> ViewState { get; } =
        new Dictionary<string, object?>();

    /// <summary>
    /// When this property is set to false it forces all view models to be
    /// deactivated.
    /// </summary>
    private bool IsActive
    {
        get;
        set
        {
            if (field == value)
                return;

            field = value;
            if (value)
                Current?.ViewModel?.Activate();
            else
                Current?.ViewModel?.Deactivate();
        }
    } = true;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public void SetCurrentIndex(int index) =>
        SetCurrentIndex(
            index: index,
            emitChanged: true
        );

    /// <inheritdoc/>
    public void SetCurrent(IViewModel? viewModel)
    {
        var index = viewModel == null ?
            -1 :
            _views.FindIndex(view => view.ViewModel == viewModel);

        SetCurrentIndex(index);
    }

    /// <inheritdoc/>
    public void SetCurrent<TViewModel>()
        where TViewModel : IViewModel
    {
        var viewModelType = typeof(TViewModel);
        var index = _views.FindIndex(view => view.ViewModel.GetType() == viewModelType);

        SetCurrentIndex(index);
    }

    /// <inheritdoc/>
    public IViewModel Add(
        int index,
        IViewModelDescriptor viewModelDescriptor
    ) =>
        Add(
            index: index,
            viewModelDescriptor: viewModelDescriptor,
            setCurrent: false
        );

    /// <inheritdoc/>
    public IViewModel AddAndSetCurrent(
        int index,
        IViewModelDescriptor viewModelDescriptor
    ) =>
        Add(
            index: index,
            viewModelDescriptor: viewModelDescriptor,
            setCurrent: true
        );

    /// <inheritdoc/>
    public TViewModel Add<TViewModel>(
        int index,
        params object[] parameters
    ) where TViewModel : IViewModel =>
        (TViewModel)Add(
            index: index,
            viewModelDescriptor: new ViewModelDescriptor<TViewModel>(parameters),
            setCurrent: false
        );

    /// <inheritdoc/>
    public TViewModel AddAndSetCurrent<TViewModel>(
        int index,
        params object[] parameters
    ) where TViewModel : IViewModel =>
        (TViewModel)Add(
            index: index,
            viewModelDescriptor: new ViewModelDescriptor<TViewModel>(parameters),
            setCurrent: true
        );

    /// <inheritdoc/>
    public IViewModel Add(IViewModelDescriptor viewModelDescriptor) =>
        Add(
            index: _views.Count,
            viewModelDescriptor: viewModelDescriptor,
            setCurrent: false
        );

    /// <inheritdoc/>
    public IViewModel AddAndSetCurrent(
        IViewModelDescriptor viewModelDescriptor
    ) =>
        Add(
            index: _views.Count,
            viewModelDescriptor: viewModelDescriptor,
            setCurrent: true
        );

    /// <inheritdoc/>
    public TViewModel Add<TViewModel>(params object[] parameters)
        where TViewModel : IViewModel =>
        (TViewModel)Add(
            index: _views.Count,
            viewModelDescriptor: new ViewModelDescriptor<TViewModel>(parameters),
            setCurrent: false
        );

    /// <inheritdoc/>
    public TViewModel AddAndSetCurrent<TViewModel>(
        params object[] parameters
    ) where TViewModel : IViewModel =>
        (TViewModel)Add(
            index: _views.Count,
            viewModelDescriptor: new ViewModelDescriptor<TViewModel>(parameters),
            setCurrent: true
        );

    /// <inheritdoc/>
    public IViewModel[] AddAll(
        int index,
        params IEnumerable<IViewModelDescriptor> viewModelDescriptors
    )
    {
        _disposeState.ThrowIfDisposed();

        var views = viewModelDescriptors
            .Select(_viewProvider.ProvideFromDescriptor)
            .ToArray();

        if (views.Length == 0)
            return [];

        foreach (var view in views.Reverse())
        {
            _views.Insert(index, view);
            Subscribe(view.ViewModel);
        }

        UpdateViewModels();

        _changedSource.Emit();

        return [.. views.Select(view => view.ViewModel)];
    }

    /// <inheritdoc/>
    public IViewModel[] AddAll(
        params IEnumerable<IViewModelDescriptor> viewModelDescriptors
    ) =>
        AddAll(
            index: _views.Count,
            viewModelDescriptors: viewModelDescriptors
        );

    /// <inheritdoc/>
    public void Clear()
    {
        var viewModels = _views
            .Select(view => view.ViewModel)
            .ToArray();

        if (viewModels.Length == 0)
            return;

        _views.Clear();
        UpdateViewModels();

        SetCurrentIndex(
            index: -1,
            emitChanged: false
        );

        foreach (var viewModel in viewModels)
            viewModel.Dispose();

        _changedSource.Emit();
    }

    /// <inheritdoc/>
    public void Remove(int index)
    {
        _disposeState.ThrowIfDisposed();

        var view = _views[index];
        Unsubscribe(view.ViewModel);
        _views.RemoveAt(index);
        UpdateViewModels();

        if (Current == view)
        {
            SetCurrentIndex(
                index: -1,
                emitChanged: false
            );
        }

        view.ViewModel.Dispose();

        _changedSource.Emit();
    }

    /// <inheritdoc/>
    public void Remove(params IEnumerable<IViewModel> viewModels)
    {
        _disposeState.ThrowIfDisposed();

        var views = _views
            .Where(view => viewModels.Contains(view.ViewModel))
            .ToArray();

        if (views.Length == 0)
            return;

        foreach (var view in views)
        {
            Unsubscribe(view.ViewModel);
            _views.Remove(view);
        }

        UpdateViewModels();

        foreach (var view in views)
        {
            if (view == Current)
            {
                SetCurrentIndex(
                    index: -1,
                    emitChanged: false
                );
            }

            view.ViewModel.Dispose();
        }

        _changedSource.Emit();
    }

    /// <inheritdoc/>
    public void Dispose() => _disposeState.Dispose();

    /// <summary>
    /// Set the current index and emit changed event only if specified.
    /// </summary>
    /// <param name="index">The index to set.</param>
    /// <param name="emitChanged">
    /// Indicates if the changed event should be emitted.
    /// </param>
    private void SetCurrentIndex(int index, bool emitChanged)
    {
        if (CurrentIndex == index)
            return;

        var view = index == -1 ? null : _views[index];
        if (Current == view)
            return;

        Current?.ViewModel?.Deactivate();
        Current = view;

        if (IsActive)
            Current?.ViewModel?.Activate();

        CurrentIndex = index;

        _currentChangedSource.Emit(
            args: new(
                view: Current,
                isNew: false
            )
        );

        _currentIndexChangedSource.Emit(index);

        if (emitChanged)
            _changedSource.Emit();
    }

    /// <summary>
    /// Add a view model and optionally set it as the current view model.
    /// </summary>
    /// <param name="index">The index to add the model at.</param>
    /// <param name="viewModelDescriptor">Descriptor of the view model.</param>
    /// <param name="setCurrent">
    /// Indicates if the view model will be set as the current view model.
    /// </param>
    /// <returns>The newly created view model.</returns>
    private IViewModel Add(
        int index,
        IViewModelDescriptor viewModelDescriptor,
        bool setCurrent
    )
    {
        _disposeState.ThrowIfDisposed();

        var view = _viewProvider.ProvideFromDescriptor(viewModelDescriptor);
        _views.Insert(index, view);
        Subscribe(view.ViewModel);
        UpdateViewModels();

        if (setCurrent)
        {
            SetCurrentIndex(
                index: index,
                emitChanged: false
            );
        }

        _changedSource.Emit();

        return view.ViewModel;
    }

    /// <summary>
    /// Update the ViewModels property to be in sync with the views collection.
    /// </summary>
    private void UpdateViewModels()
    {
        ViewModels = [.. _views.Select(view => view.ViewModel)];
    }

    /// <summary>
    /// Subscribe to events for the view model.
    /// </summary>
    /// <param name="viewModel">The view model to subscribe to.</param>
    private void Subscribe(IViewModel viewModel)
    {
        if (viewModel is ICloseObservable closable)
            closable.Closed.Event += OnClosed;
    }

    /// <summary>
    /// Unsubscribe from events for the view model.
    /// </summary>
    /// <param name="viewModel">The view model to unsubscribe from.</param>
    private void Unsubscribe(IViewModel viewModel)
    {
        if (viewModel is ICloseObservable closable)
            closable.Closed.Event -= OnClosed;
    }

    /// <summary>
    /// Handle the Closed event raised by an IClosable object.
    /// </summary>
    /// <param name="closable">The IClosable that raised the event.</param>
    private void OnClosed(ICloseObservable closable)
    {
        if (closable is IViewModel viewModel)
            Remove(viewModel);
    }

    #endregion

    #region Events

    /// <inheritdoc/>
    public IEventObservable<IPresenter.CurrentChangedEventArgs> CurrentChanged => _currentChangedSource.Observable;
    private readonly EventObservableSource<IPresenter.CurrentChangedEventArgs> _currentChangedSource = new();

    /// <inheritdoc/>
    public IEventObservable<int> CurrentIndexChanged => _currentIndexChangedSource.Observable;
    private readonly EventObservableSource<int> _currentIndexChangedSource = new();

    /// <inheritdoc/>
    public IEventObservable Changed => _changedSource.Observable;
    private readonly EventObservableSource _changedSource = new();

    #endregion
}
