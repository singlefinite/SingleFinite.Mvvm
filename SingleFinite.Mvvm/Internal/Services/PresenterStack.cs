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

using System.ComponentModel;
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.Internal.Services;

/// <summary>
/// Implementation of <see cref="IPresenterStack"/>.
/// </summary>
internal sealed class PresenterStack :
    IPresenterStack,
    IDisposable
{
    #region Fields

    /// <summary>
    /// Used to build view objects.
    /// </summary>
    private readonly IViewBuilder _viewBuilder;

    /// <summary>
    /// Set to true when this object is disposed.
    /// </summary>
    private bool _isDisposed = false;

    /// <summary>
    /// Holds the underlying stack.
    /// </summary>
    private readonly ViewStack _stack = new();

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="viewBuilder">Used to build view objects.</param>
    public PresenterStack(IViewBuilder viewBuilder)
    {
        _viewBuilder = viewBuilder;

        _stack.TopViewChanged.Register(
            () => PropertyChanged?.Invoke(
                sender: this,
                e: new(nameof(Current))
            )
        );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public IView? Current => _stack.TopView;

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
        PopOptions? popOptions = null
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var view = _viewBuilder.Build(viewModelDescriptor);
        _stack.Push(
            views: [view],
            popCount: GetPopCount(popOptions)
        );

        return view.ViewModel;
    }

    /// <inheritdoc/>
    public TViewModel Push<TViewModel>(PopOptions? popOptions = null)
        where TViewModel : IViewModel =>
        (TViewModel)Push(
            new ViewModelDescriptor<TViewModel>(), popOptions
        );

    /// <inheritdoc/>
    public TViewModel Push<TViewModel, TViewModelContext>(
        TViewModelContext context,
        PopOptions? popOptions = null
    )
        where TViewModel : IViewModel<TViewModelContext> =>
        (TViewModel)Push(
            new ViewModelDescriptor<TViewModel, TViewModelContext>(context),
            popOptions
        );

    /// <inheritdoc/>
    public IViewModel[] PushAll(
        IEnumerable<IViewModelDescriptor> viewModelDescriptors,
        PopOptions? popOptions = null
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var viewList = viewModelDescriptors
            .Select(_viewBuilder.Build)
            .ToList();

        if (viewList.Count == 0)
            return [];

        _stack.Push(viewList, GetPopCount(popOptions));

        return viewList.Select(view => view.ViewModel).ToArray();
    }

    /// <inheritdoc/>
    public bool Pop()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        return _stack.Pop(1);
    }

    /// <inheritdoc/>
    public bool PopTo<TViewModel>(
        bool fromTop = true,
        bool inclusive = false
    ) where TViewModel : IViewModel
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

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
        ObjectDisposedException.ThrowIf(_isDisposed, this);

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
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        _stack.Clear();
    }

    /// <inheritdoc/>
    public void Close(params IEnumerable<IViewModel> viewModels)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        _stack.Close(viewModels);
    }

    /// <summary>
    /// Clear the stack.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        Clear();

        _isDisposed = true;
    }

    #endregion

    #region Events

    /// <inheritdoc/>
    public EventToken<IView?> CurrentChanged => _stack.TopViewChanged;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion
}
