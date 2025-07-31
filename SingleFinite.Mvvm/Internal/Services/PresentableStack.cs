// MIT License
// Copyright (c) 2025 Single Finite
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

namespace SingleFinite.Mvvm.Internal.Services;

/// <summary>
/// Implementation of <see cref="IPresentableStack"/>.
/// </summary>
internal sealed class PresentableStack :
    IPresentableStack,
    IDisposable,
    IDisposeObservable
{
    #region Fields

    /// <summary>
    /// Holds the dispose state for this object.
    /// </summary>
    private readonly DisposeState _disposeState;

    /// <summary>
    /// Holds the underlying stack.
    /// </summary>
    private readonly ViewStack _stack = new();

    /// <summary>
    /// Holds view builder used to build objects.
    /// </summary>
    private readonly IViewBuilder _viewBuilder;

    #endregion

    #region Constructors

    public PresentableStack(IViewBuilder viewBuilder)
    {
        _viewBuilder = viewBuilder;
        _disposeState = new(
            owner: this,
            onDispose: Clear
        );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public bool IsDisposed => _disposeState.IsDisposed;

    /// <inheritdoc/>
    public IView? Current => _stack.Current;

    /// <inheritdoc/>
    public IViewModel[] ViewModels => _stack.ViewModels;

    #endregion

    #region Methods

    /// <summary>
    /// Get the number of view models that should be popped off of the stack so
    /// that the desired view model is left on top.
    /// </summary>
    /// <param name="predicate">
    /// The function used to identify a view model.
    /// </param>
    /// <param name="fromTop">
    /// When true, iterate from the top of the stack to the bottom when 
    /// searching for the view model.
    /// When false, iterate from the bottom of the stack to the top when 
    /// searching for the view mdoel.
    /// </param>
    /// <param name="inclusive">
    /// If true the pop count should remove the identified view from the stack 
    /// and everything above it.
    /// If false the pop count should leave the identified view on the stack but
    /// remove everything above it.
    /// </param>
    /// <returns>The number of view models to pop off the stack in order to
    /// leave the desired view model on top.
    /// </returns>
    private int GetPopCount(
        Func<IViewModel, bool> predicate,
        bool fromTop,
        bool inclusive
    )
    {
        var query = _stack.ViewModels
            .Select((viewModel, index) => predicate(viewModel) ? index : -1)
            .Where(index => index != -1);

        var index = fromTop ?
            query.FirstOrDefault(-1) :
            query.LastOrDefault(-1);

        if (index == -1)
            return 0;

        return index + (inclusive ? 1 : 0);
    }

    /// <summary>
    /// Get the number of view models that should be popped off of the stack
    /// based on the pop options.
    /// </summary>
    /// <param name="popOptions">
    /// The options that determine the number of view models to pop off the
    /// stack.
    /// </param>
    /// <returns>
    /// The number of view models that should be popped off the stack.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if an unknown pop options type is provided.
    /// </exception>
    private int GetPopCount(PopOptions? popOptions) => popOptions switch
    {
        null => 0,
        PopOptions.PopOptionsRemoveAll => _stack.Count,
        PopOptions.PopOptionsCount count => Math.Min(
            count.Count,
            _stack.Count
        ),
        PopOptions.PopOptionsQuery query => GetPopCount(
            predicate: query.Predicate,
            fromTop: query.FromTop,
            inclusive: query.Inclusive
        ),
        _ => throw new InvalidOperationException(
            $"Unexpected PopOptions type: {popOptions.GetType().FullName}"
        )
    };

    /// <inheritdoc/>
    public IViewModel Push(
        IViewModelDescriptor viewModelDescriptor,
        PopOptions? popOptions = default
    )
    {
        _disposeState.ThrowIfDisposed();

        var view = _viewBuilder.Build(viewModelDescriptor);
        _stack.Push(
            views: [view],
            popCount: GetPopCount(popOptions)
        );

        return view.ViewModel;
    }

    /// <inheritdoc/>
    public TViewModel Push<TViewModel>(
        params object[] parameters
    )
        where TViewModel : IViewModel =>
        Push<TViewModel>(
            popOptions: null,
            parameters
        );

    /// <inheritdoc/>
    public TViewModel Push<TViewModel>(
        PopOptions? popOptions = default,
        params object[] parameters
    )
        where TViewModel : IViewModel =>
        (TViewModel)Push(
            new ViewModelDescriptor<TViewModel>(parameters),
            popOptions
        );

    /// <inheritdoc/>
    public IViewModel[] PushAll(
        params IEnumerable<IViewModelDescriptor> viewModelDescriptors
    ) => PushAll(
        popOptions: null,
        viewModelDescriptors
    );

    /// <inheritdoc/>
    public IViewModel[] PushAll(
        PopOptions? popOptions = default,
        params IEnumerable<IViewModelDescriptor> viewModelDescriptors
    )
    {
        _disposeState.ThrowIfDisposed();

        var viewList = viewModelDescriptors
            .Select(_viewBuilder.Build)
            .ToList();

        if (viewList.Count == 0)
            return [];

        _stack.Push(GetPopCount(popOptions), viewList);

        return viewList.Select(view => view.ViewModel).ToArray();
    }

    /// <inheritdoc/>
    public IViewModel Add(
        int index,
        IViewModelDescriptor viewModelDescriptor,
        PopOptions? popOptions = default
    )
    {
        _disposeState.ThrowIfDisposed();

        var view = _viewBuilder.Build(viewModelDescriptor);
        _stack.Add(
            index: index,
            views: [view]
        );

        return view.ViewModel;
    }

    /// <inheritdoc/>
    public TViewModel Add<TViewModel>(
        int index,
        params object[] parameters
    )
        where TViewModel : IViewModel =>
        Add<TViewModel>(
            index: index,
            popOptions: null,
            parameters
        );

    /// <inheritdoc/>
    public TViewModel Add<TViewModel>(
        int index,
        PopOptions? popOptions = default,
        params object[] parameters
    )
        where TViewModel : IViewModel =>
        (TViewModel)Add(
            index,
            new ViewModelDescriptor<TViewModel>(parameters),
            popOptions
        );

    /// <inheritdoc/>
    public IViewModel[] AddAll(
        int index,
        params IEnumerable<IViewModelDescriptor> viewModelDescriptors
    ) => AddAll(
        index: index,
        popOptions: null,
        viewModelDescriptors
    );

    /// <inheritdoc/>
    public IViewModel[] AddAll(
        int index,
        PopOptions? popOptions = default,
        params IEnumerable<IViewModelDescriptor> viewModelDescriptors
    )
    {
        _disposeState.ThrowIfDisposed();

        var viewList = viewModelDescriptors
            .Select(_viewBuilder.Build)
            .ToList();

        if (viewList.Count == 0)
            return [];

        _stack.Add(index, viewList);

        return viewList.Select(view => view.ViewModel).ToArray();
    }

    /// <inheritdoc/>
    public bool Pop()
    {
        _disposeState.ThrowIfDisposed();
        return _stack.Pop(1);
    }

    /// <inheritdoc/>
    public bool PopTo<TViewModel>(
        bool fromTop = true,
        bool inclusive = false
    ) where TViewModel : IViewModel
    {
        _disposeState.ThrowIfDisposed();

        return _stack.Pop(
            GetPopCount(
                PopOptions.PopTo<TViewModel>(
                    fromTop: fromTop,
                    inclusive: inclusive
                )
            )
        );
    }

    /// <inheritdoc/>
    public bool PopTo(
        Func<IViewModel, bool> predicate,
        bool fromTop = true,
        bool inclusive = false
    )
    {
        _disposeState.ThrowIfDisposed();

        return _stack.Pop(
            GetPopCount(
                PopOptions.PopTo(
                    predicate: predicate,
                    fromTop: fromTop,
                    inclusive: inclusive
                )
            )
        );
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _stack.Clear();
    }

    /// <inheritdoc/>
    public void Close(params IEnumerable<IViewModel> viewModels)
    {
        _disposeState.ThrowIfDisposed();
        _stack.Close(viewModels);
    }

    /// <inheritdoc/>
    public void Dispose() => _disposeState.Dispose();

    #endregion

    #region Events

    /// <inheritdoc/>
    public Observable<IPresentable.CurrentChangedEventArgs> CurrentChanged =>
        _stack.CurrentChanged;

    /// <inheritdoc/>
    public Observable Disposed => _disposeState.Disposed;

    #endregion
}
