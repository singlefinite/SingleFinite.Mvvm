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

using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.Internal.Services;

/// <summary>
/// Implementation of <see cref="IPresenterStack"/>.
/// </summary>
/// <param name="viewBuilder">Used to build view objects.</param>
internal sealed class PresenterStack(IViewBuilder viewBuilder) :
    IPresenterStack,
    IDisposable
{
    #region Fields

    /// <summary>
    /// Set to true when this object is disposed.
    /// </summary>
    private bool _isDisposed = false;

    /// <summary>
    /// Holds the underlying stack.
    /// </summary>
    private readonly Stack<IView> _stack = [];

    #endregion

    #region Properties

    /// <inheritdoc/>
    public IView? Current { get; private set; }

    /// <inheritdoc/>
    public IView[] Stack { get; private set; } = [];

    #endregion

    #region Methods

    /// <summary>
    /// Update the public stack properties so they match the private writable
    /// stack.
    /// </summary>
    private void UpdateCurrent()
    {
        var newCurrent = _stack.FirstOrDefault();
        var isCurrentChanged = newCurrent != Current;

        Current = newCurrent;
        Stack = [.. _stack];

        if (isCurrentChanged)
        {
            Current?.ViewModel?.Activate();
            CurrentChanged.RaiseEvent(newCurrent);
        }
    }

    /// <summary>
    /// Get the number of views that should be popped off of the stack so that 
    /// the desired view is left on top.
    /// </summary>
    /// <param name="predicate">The function used to identify a view.</param>
    /// <param name="fromTop">
    /// When true, iterate from the top of the stack to the bottom when 
    /// searching for a view.
    /// When false, iterate from the bottom of the stack to the top when 
    /// searching for a view.
    /// </param>
    /// <param name="inclusive">
    /// If true the pop count should remove the identified view from the stack 
    /// and everything above it.
    /// If false the pop count should leave the identified view on the stack but
    /// remove everything above it.
    /// </param>
    /// <returns>The number of views to pop off the stack in order to leave the 
    /// desired view on top.</returns>
    private int FindPopCount(
        Func<IView, bool> predicate,
        bool fromTop,
        bool inclusive
    )
    {
        var query = _stack
            .Select((view, index) => predicate(view) ? index : -1)
            .Where(index => index != -1);

        var index = fromTop ?
            query.FirstOrDefault(-1) :
            query.LastOrDefault(-1);

        if (index == -1)
            return 0;

        return index + (inclusive ? 1 : 0);
    }

    /// <summary>
    /// Remove views from the stack based on the given options.
    /// </summary>
    /// <param name="popOptions">The push options to process.</param>
    /// <param name="alwaysDeactivateTop">
    /// When true the top view in the stack will be deactivated even if the 
    /// stack is not changed
    /// by this method.
    /// </param>
    /// <returns>true if the stack was modified, false if it wasn't.</returns>
    private bool PopTo(
        PopOptions? popOptions,
        bool alwaysDeactivateTop
    )
    {
        var popCount = popOptions switch
        {
            null => 0,
            PopOptions.PopOptionsRemoveAll => _stack.Count,
            PopOptions.PopOptionsCount count => Math.Min(
                count.Count,
                _stack.Count
            ),
            PopOptions.PopOptionsQuery query => FindPopCount(
                predicate: query.Predicate,
                fromTop: query.FromTop,
                inclusive: query.Inclusive
            ),
            _ => throw new InvalidOperationException(
                $"Unexpected PopOptions type: {popOptions.GetType().FullName}"
            )
        };

        if (popCount == 0)
        {
            if (alwaysDeactivateTop && _stack.Count > 0)
                _stack.Peek().ViewModel.Deactivate();

            return false;
        }

        _stack.Peek().ViewModel.Deactivate();

        for (var popped = 0; popped < popCount; popped++)
            _stack.Pop().ViewModel.Dispose();

        return true;
    }

    /// <inheritdoc/>
    public IView Push(
        IViewModelDescriptor viewModelDescriptor,
        PopOptions? popOptions = null
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        PopTo(
            popOptions: popOptions,
            alwaysDeactivateTop: true
        );

        var view = viewBuilder.Build(viewModelDescriptor);
        _stack.Push(view);

        UpdateCurrent();

        return view;
    }

    /// <inheritdoc/>
    public IView<TViewModel> Push<TViewModel>(PopOptions? popOptions = null)
        where TViewModel : IViewModel =>
        (IView<TViewModel>)Push(
            new ViewModelDescriptor<TViewModel>(), popOptions
        );

    /// <inheritdoc/>
    public IView<TViewModel> Push<TViewModel, TViewModelContext>(
        TViewModelContext context,
        PopOptions? popOptions = null
    )
        where TViewModel : IViewModel<TViewModelContext> =>
        (IView<TViewModel>)Push(
            new ViewModelDescriptor<TViewModel, TViewModelContext>(context),
            popOptions
        );

    /// <inheritdoc/>
    public IView[] PushAll(
        IEnumerable<IViewModelDescriptor> viewModelDescriptors,
        PopOptions? popOptions = null
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (!viewModelDescriptors.Any())
            return [];

        PopTo(
            popOptions: popOptions,
            alwaysDeactivateTop: true
        );

        var viewList = viewModelDescriptors
            .Select(viewBuilder.Build)
            .ToList();

        foreach (var view in viewList)
            _stack.Push(view);

        UpdateCurrent();

        return [.. viewList];
    }

    /// <inheritdoc/>
    public bool Pop()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var didPop = PopTo(
            popOptions: PopOptions.PopCount(1),
            alwaysDeactivateTop: false
        );

        UpdateCurrent();

        return didPop;
    }

    /// <inheritdoc/>
    public bool PopTo<TViewModel>(
        bool fromTop = true,
        bool inclusive = false
    ) where TViewModel : IViewModel
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var didPop = PopTo(
            popOptions: PopOptions.PopTo<TViewModel>(
                fromTop: fromTop,
                inclusive: inclusive
            ),
            alwaysDeactivateTop: false
        );

        UpdateCurrent();

        return didPop;
    }

    /// <inheritdoc/>
    public bool PopTo(
        Func<IView, bool> predicate,
        bool fromTop = true,
        bool inclusive = false
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var didPop = PopTo(
            popOptions: PopOptions.PopTo(
                predicate: predicate,
                fromTop: fromTop,
                inclusive: inclusive
            ),
            alwaysDeactivateTop: false
        );

        UpdateCurrent();

        return didPop;
    }

    /// <inheritdoc/>
    public void PopAll()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        PopTo(
            popOptions: PopOptions.PopAll(),
            alwaysDeactivateTop: false
        );

        UpdateCurrent();
    }

    /// <summary>
    /// Clear the stack.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        PopAll();

        _isDisposed = true;
    }

    #endregion

    #region Events

    /// <inheritdoc/>
    public EventToken<IView?> CurrentChanged => _currentChangedSource.Token;
    private readonly EventTokenSource<IView?> _currentChangedSource = new();

    #endregion
}
