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

using System.Collections;
using SingleFinite.Essentials;
using SingleFinite.Mvvm.Internal.Services;
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.Internal;

/// <summary>
/// Implementation of <see cref="IViewList"/>.
/// </summary>
internal class ViewList : IViewList, IDisposable
{
    #region Fields

    /// <summary>
    /// Holds the underlying list.
    /// </summary>
    private readonly List<IView> _list = [];

    /// <summary>
    /// The service that provides view.
    /// </summary>
    private readonly IViewProvider _viewProvider;

    /// <summary>
    /// The dispose state.
    /// </summary>
    private readonly DisposeState _disposeState;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="viewProvider">The view provider to use.</param>
    /// <param name="viewModelNode">The view model node to use.</param>
    public ViewList(
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
    public int Count => _list.Count;

    /// <inheritdoc/>
    public IView this[int index] => _list[index];

    /// <summary>
    /// The current active state.
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
            {
                foreach (var view in _list)
                    view.ViewModel.Activate();
            }
            else
            {
                foreach (var view in _list)
                    view.ViewModel.Deactivate();
            }
        }
    } = true;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public IViewModel Add(
        int index,
        IViewModelDescriptor viewModelDescriptor
    )
    {
        var viewModel = InsertAt(index, viewModelDescriptor).First();
        _changed.Emit();
        return viewModel;
    }

    /// <inheritdoc/>
    public TViewModel Add<TViewModel>(
        int index,
        params object[] parameters
    )
        where TViewModel : IViewModel =>
        (TViewModel)Add(
            index: index,
            viewModelDescriptor: new ViewModelDescriptor<TViewModel>(parameters)
        );

    /// <inheritdoc/>
    public IViewModel Add(
        IViewModelDescriptor viewModelDescriptor
    ) =>
        Add(
            index: _list.Count,
            viewModelDescriptor: viewModelDescriptor
        );

    /// <inheritdoc/>
    public TViewModel Add<TViewModel>(
        params object[] parameters
    )
        where TViewModel : IViewModel =>
        (TViewModel)Add(
            viewModelDescriptor: new ViewModelDescriptor<TViewModel>(parameters)
        );

    /// <inheritdoc/>
    public IViewModel[] AddAll(
        int index,
        params IEnumerable<IViewModelDescriptor> viewModelDescriptors
    )
    {
        var result = InsertAt(index, viewModelDescriptors);
        _changed.Emit();
        return result.ToArray();
    }

    /// <inheritdoc/>
    public IViewModel[] AddAll(
        params IEnumerable<IViewModelDescriptor> viewModelDescriptors
    ) =>
        AddAll(
            index: _list.Count,
            viewModelDescriptors: viewModelDescriptors
        );

    /// <inheritdoc/>
    public void Move(
        int fromIndex,
        int toIndex
    )
    {
        _disposeState.ThrowIfDisposed();

        if (fromIndex < 0 || fromIndex >= _list.Count)
            throw new IndexOutOfRangeException($"{nameof(fromIndex)} is outside the range.");
        if (toIndex < 0 || toIndex >= _list.Count)
            throw new IndexOutOfRangeException($"{nameof(toIndex)} is outside the range.");

        if (fromIndex == toIndex)
            return;

        var view = _list[fromIndex];
        _list.RemoveAt(fromIndex);
        _list.Insert(toIndex, view);

        _changed.Emit();
    }

    /// <inheritdoc/>
    public void Remove(int index)
    {
        RemoveAt(index);
        _changed.Emit();
    }

    /// <inheritdoc/>
    public void Remove(params IEnumerable<IViewModel> viewModels)
    {
        var isChanged = viewModels
            .Select(RemoveFor)
            .Aggregate((current, next) => current || next);

        if (isChanged)
            _changed.Emit();
    }

    /// <inheritdoc/>
    public void Clear()
    {
        if (_list.Count == 0)
            return;

        foreach (var view in _list)
        {
            Unsubscribe(view.ViewModel);
            view.ViewModel.Deactivate();
            view.ViewModel.Dispose();
        }

        _list.Clear();
        _changed.Emit();
    }

    /// <inheritdoc/>
    public IEnumerator<IView> GetEnumerator() =>
        _list.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() =>
        _list.GetEnumerator();

    /// <inheritdoc/>
    public void Dispose() => _disposeState.Dispose();

    /// <summary>
    /// Insert the given view models at the given index.
    /// </summary>
    /// <param name="index">The index to insert the view models at.</param>
    /// <param name="viewModelDescriptors">The view models to insert.</param>
    /// <returns>The newly created view models.</returns>
    private IViewModel[] InsertAt(
        int index,
        params IEnumerable<IViewModelDescriptor> viewModelDescriptors
    )
    {
        _disposeState.ThrowIfDisposed();

        var result = new List<IViewModel>();
        var currentIndex = index;
        foreach (var viewModelDescriptor in viewModelDescriptors)
        {
            var view = _viewProvider.ProvideFromDescriptor(viewModelDescriptor);
            Subscribe(view.ViewModel);
            if (IsActive)
                view.ViewModel.Activate();

            _list.Insert(currentIndex, view);
            result.Add(view.ViewModel);
            currentIndex++;
        }

        return result.ToArray();
    }

    /// <summary>
    /// Remove the view model at the given index.
    /// </summary>
    /// <param name="index">The index of the view model to remove.</param>
    private void RemoveAt(int index)
    {
        _disposeState.ThrowIfDisposed();

        var view = _list[index];
        _list.RemoveAt(index);
        Unsubscribe(view.ViewModel);
        view.ViewModel.Deactivate();
        view.ViewModel.Dispose();
    }

    /// <summary>
    /// Remove the given view model.
    /// </summary>
    /// <param name="viewModel">The view model to remove.</param>
    /// <returns>
    /// true if the view model was removed or false if the view model wasn't found.
    /// </returns>
    private bool RemoveFor(IViewModel viewModel)
    {
        _disposeState.ThrowIfDisposed();

        var index = _list.FindIndex(view => view.ViewModel == viewModel);
        if (index != -1)
        {
            RemoveAt(index);
            return true;
        }

        return false;
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
    public IEventObservable Changed => _changed.Observable;
    private readonly EventObservableSource _changed = new();

    #endregion
}
