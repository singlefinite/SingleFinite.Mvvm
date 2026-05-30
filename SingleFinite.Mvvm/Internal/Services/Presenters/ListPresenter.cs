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
    /// Holds view builder used to build objects.
    /// </summary>
    private readonly IViewBuilder _viewBuilder;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="viewBuilder">Used to build view objects.</param>
    /// <param name="viewModelNode">
    /// Used to observe when a parent IsActive value changes.
    /// </param>
    public ListPresenter(
        IViewBuilder viewBuilder,
        ViewModelNode viewModelNode
    )
    {
        _viewBuilder = viewBuilder;
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
    public void SetCurrentIndex(int index)
    {
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
    }

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
    public IViewModel Add(int index, IViewModelDescriptor viewModelDescriptor)
    {
        _disposeState.ThrowIfDisposed();

        var view = _viewBuilder.BuildFromDescriptor(viewModelDescriptor);
        _views.Insert(index, view);
        Subscribe(view.ViewModel);
        UpdateViewModels();

        return view.ViewModel;
    }

    /// <inheritdoc/>
    public TViewModel Add<TViewModel>(
        int index,
        params object[] parameters
    ) where TViewModel : IViewModel =>
        (TViewModel)Add(
            index: index,
            viewModelDescriptor: new ViewModelDescriptor<TViewModel>(parameters)
        );

    /// <inheritdoc/>
    public IViewModel Add(IViewModelDescriptor viewModelDescriptor) =>
        Add(
            index: _views.Count,
            viewModelDescriptor: viewModelDescriptor
        );

    /// <inheritdoc/>
    public TViewModel Add<TViewModel>(params object[] parameters)
        where TViewModel : IViewModel =>
        (TViewModel)Add(
            index: _views.Count,
            viewModelDescriptor: new ViewModelDescriptor<TViewModel>(parameters)
        );

    /// <inheritdoc/>
    public IViewModel[] AddAll(
        int index,
        params IEnumerable<IViewModelDescriptor> viewModelDescriptors
    )
    {
        _disposeState.ThrowIfDisposed();

        var views = viewModelDescriptors
            .Select(_viewBuilder.BuildFromDescriptor)
            .ToArray();

        foreach (var view in views.Reverse())
        {
            _views.Insert(index, view);
            Subscribe(view.ViewModel);
        }

        UpdateViewModels();

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

        _views.Clear();
        UpdateViewModels();

        SetCurrent(null);

        foreach (var viewModel in viewModels)
            viewModel.Dispose();
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
            SetCurrent(null);

        view.ViewModel.Dispose();
    }

    /// <inheritdoc/>
    public void Remove(params IEnumerable<IViewModel> viewModels)
    {
        _disposeState.ThrowIfDisposed();

        var views = _views
            .Where(view => viewModels.Contains(view.ViewModel))
            .ToArray();

        foreach (var view in views)
        {
            Unsubscribe(view.ViewModel);
            _views.Remove(view);
        }

        UpdateViewModels();

        foreach (var view in views)
        {
            if (view == Current)
                SetCurrent(null);

            view.ViewModel.Dispose();
        }
    }

    /// <inheritdoc/>
    public void Dispose() => _disposeState.Dispose();

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

    #endregion
}
