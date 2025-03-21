// MIT License
// Copyright (c) 2024 Single Finite
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

namespace SingleFinite.Mvvm.Internal;

/// <summary>
/// A mutable collection of views that manages lifecycle events.
/// The behavior is similar to a stack however views can be added and removed
/// from anywhere and not just the top.
/// </summary>
internal class ViewStack
{
    #region Properties

    /// <summary>
    /// The views in the stack.  The top most view in the stack is at index 0.
    /// </summary>
    public IView[] Views { get; private set; } = [];
    private readonly List<IView> _views = [];

    /// <summary>
    /// The view models in the stack.  The top most view model is at index 0.
    /// </summary>
    public IViewModel[] ViewModels { get; private set; } = [];

    /// <summary>
    /// The number of views in the stack.
    /// </summary>
    public int Count => _views.Count;

    /// <summary>
    /// The current view is the top most view in the stack.
    /// </summary>
    public IView? Current { get; private set; }

    #endregion

    #region Methods

    /// <summary>
    /// Add the given views to the top of the stack.
    /// Views are pushed onto the stack in order so the last view in the
    /// enumerable will become the top most view.
    /// </summary>
    /// <param name="views">The views to add.</param>
    public void Push(params IEnumerable<IView> views) => Push(
        popCount: 0,
        views: views
    );

    /// <summary>
    /// Add the given views to the top of the stack.
    /// Views are pushed onto the stack in order so the last view in the
    /// enumerable will become the top most view.
    /// </summary>
    /// <param name="popCount">
    /// The number of views to pop off the top before adding the new views.
    /// </param>
    /// <param name="views">The views to add.</param>
    public void Push(int popCount, params IEnumerable<IView> views)
    {
        if (!views.Any())
            return;

        DeactivateTop();
        Remove(
            index: 0,
            count: popCount
        );
        Insert(
            index: 0,
            views: views
        );
        UpdateState(activateTop: true);
    }

    /// <summary>
    /// Add the given views to the stack at the given index.
    /// Views are added onto the stack in order so the last view in the
    /// enumerable will become the top most view.
    /// </summary>
    /// <param name="index">The index to add the views to.</param>
    /// <param name="views">The views to add.</param>
    public void Add(int index, params IEnumerable<IView> views)
    {
        if (index < 0 || index > Count)
            throw new IndexOutOfRangeException(nameof(index));

        if (index == 0)
        {
            Push(
                popCount: 0,
                views: views
            );
        }
        else
        {
            Insert(index, views);
            UpdateState();
        }
    }

    /// <summary>
    /// Pop the given number of views from the top.
    /// </summary>
    /// <param name="popCount">The number of views to pop.</param>
    /// <returns>true if the stack was modifed, false if it wasn't.</returns>
    public bool Pop(int popCount)
    {
        if (popCount <= 0)
            return false;

        DeactivateTop();
        Remove(
            index: 0,
            count: popCount
        );
        UpdateState(activateTop: true);

        return true;
    }

    /// <summary>
    /// Remove the views that have the given view models from the stack if
    /// present.
    /// </summary>
    /// <param name="viewModels">The view models to remove.</param>
    /// <returns>true if the stack was modifed, false if it wasn't.</returns>
    public bool Close(params IEnumerable<IViewModel> viewModels)
    {
        var indices = _views
            .Select((view, index) => viewModels.Contains(view.ViewModel) ? index : -1)
            .Where(index => index != -1)
            .OrderByDescending(index => index)
            .ToList();

        if (indices.Count == 0)
            return false;

        if (indices.Last() == 0)
        {
            DeactivateTop();
            Remove(indices);
            UpdateState(activateTop: true);
        }
        else
        {
            Remove(indices);
            UpdateState();
        }

        return true;
    }

    /// <summary>
    /// Remove all views from the stack.
    /// </summary>
    public void Clear()
    {
        DeactivateTop();
        Remove(
            index: 0,
            count: _views.Count
        );
        UpdateState();
    }

    /// <summary>
    /// Update properties for this object so the values are all in sync.
    /// </summary>
    /// <param name="activateTop">
    /// When true the top most view model will be activated after the state is
    /// updated but before the change events are raised.
    /// </param>
    private void UpdateState(bool activateTop = false)
    {
        var newTopView = _views.FirstOrDefault();
        var isNew = newTopView is not null && !Views.Contains(newTopView);

        Views = [.. _views];
        ViewModels = _views.Select(view => view.ViewModel).ToArray();

        if (Current != newTopView)
        {
            Current = _views.FirstOrDefault();

            if (activateTop)
                ActivateTop();

            _currentChanged.RaiseEvent(
                new(
                    view: Current,
                    isNew: isNew
                )
            );
        }
        else if (activateTop)
        {
            ActivateTop();
        }
    }

    /// <summary>
    /// Deactivate the top most view model in the stack.
    /// </summary>
    private void DeactivateTop() =>
        _views.FirstOrDefault()?.ViewModel?.Deactivate();

    /// <summary>
    /// Activate the top most view model in the stack.
    /// </summary>
    private void ActivateTop() =>
        _views.FirstOrDefault()?.ViewModel?.Activate();

    /// <summary>
    /// Subscribe to events for the view model.
    /// </summary>
    /// <param name="viewModel">The view model to subscribe to.</param>
    private void Subscribe(IViewModel viewModel)
    {
        if (viewModel is IClosable closable)
            closable.Closed.Event += OnClosed;
    }

    /// <summary>
    /// Unsubscribe from events for the view model.
    /// </summary>
    /// <param name="viewModel">The view model to unsubscribe from.</param>
    private void Unsubscribe(IViewModel viewModel)
    {
        if (viewModel is IClosable closable)
            closable.Closed.Event -= OnClosed;
    }

    /// <summary>
    /// Add the given views to the stack.
    /// </summary>
    /// <param name="index">The index to insert the views at.</param>
    /// <param name="views">The views to add.</param>
    private void Insert(int index, IEnumerable<IView> views)
    {
        foreach (var view in views)
            Subscribe(view.ViewModel);

        _views.InsertRange(index, views.Reverse());
    }

    /// <summary>
    /// Remove the given range of views from the stack and dispose of them.
    /// </summary>
    /// <param name="index">The index to start removing from.</param>
    /// <param name="count">The number of views to remove.</param>
    private void Remove(int index, int count)
    {
        if (count <= 0)
            return;

        var lastIndex = Math.Min(index + count, _views.Count);

        for (var i = index; i < lastIndex; i++)
        {
            var view = _views[i];
            Unsubscribe(view.ViewModel);
            view.ViewModel?.Dispose();
        }

        _views.RemoveRange(index, lastIndex - index);
    }

    /// <summary>
    /// Remove the views at the given indicies.
    /// </summary>
    /// <param name="indices">
    /// The indices of the views to remove.  They must be in descending order.
    /// </param>
    private void Remove(IEnumerable<int> indices)
    {
        foreach (var index in indices)
            Remove(index, 1);
    }

    /// <summary>
    /// Handle the Closed event raised by an IClosable object.
    /// </summary>
    /// <param name="closable">The IClosable that raised the event.</param>
    private void OnClosed(IClosable closable)
    {
        if (closable is IViewModel viewModel)
            Close(viewModel);
    }

    #endregion

    #region Events

    /// <summary>
    /// Event raised when the current view has been changed.
    /// </summary>
    public Observable<IPresentable.CurrentChangedEventArgs> CurrentChanged => _currentChanged.Observable;
    private readonly ObservableSource<IPresentable.CurrentChangedEventArgs> _currentChanged = new();

    #endregion
}
