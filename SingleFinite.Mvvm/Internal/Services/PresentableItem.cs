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
/// Implementation of <see cref="IPresentableItem"/>.
/// </summary>
/// <param name="viewBuilder">Used to build view objects.</param>
internal sealed class PresentableItem(IViewBuilder viewBuilder) :
    IPresentableItem,
    IDisposable
{
    #region Fields

    /// <summary>
    /// Set to true when this object is disposed.
    /// </summary>
    private bool _isDisposed = false;

    /// <summary>
    /// Holds the current view.  A ViewStack is used here as it supports
    /// lifecycle management of the view.
    /// </summary>
    private readonly ViewStack _stack = new();

    #endregion

    #region Properties

    /// <inheritdoc/>
    public IView? Current => _stack.Current;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public IViewModel Set(IViewModelDescriptor viewModelDescriptor)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var view = viewBuilder.Build(viewModelDescriptor);
        _stack.Push(
            views: [view],
            popCount: 1
        );

        return view.ViewModel;
    }

    /// <inheritdoc/>
    public TViewModel Set<TViewModel>()
        where TViewModel : IViewModel =>
        (TViewModel)Set(new ViewModelDescriptor<TViewModel>());

    /// <inheritdoc/>
    public TViewModel Set<TViewModel, TViewModelContext>(
        TViewModelContext context
    ) where TViewModel : IViewModel<TViewModelContext> =>
        (TViewModel)Set(
            new ViewModelDescriptor<TViewModel, TViewModelContext>(context)
        );

    /// <inheritdoc/>
    public void Clear()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        _stack.Clear();
    }

    /// <summary>
    /// Clear the frame.
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
    public Observable<IPresentable.CurrentChangedEventArgs> CurrentChanged =>
        _stack.CurrentChanged;

    #endregion
}
