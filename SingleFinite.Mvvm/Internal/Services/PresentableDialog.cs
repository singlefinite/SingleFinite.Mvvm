﻿// MIT License
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
/// Implementation of <see cref="IPresentableDialog"/>.
/// </summary>
/// <param name="viewBuilder">Used to build view objects.</param>
internal class PresentableDialog(IViewBuilder viewBuilder) :
    IPresentableDialog,
    IDisposable
{
    #region Fields

    /// <summary>
    /// Set to true when this object has been disposed.
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// Holds the stack of dialog views.
    /// </summary>
    private readonly ViewStack _stack = new();

    #endregion

    #region Properties

    /// <inheritdoc/>
    public IView? Current => _stack.Current;

    /// <inheritdoc/>
    public IViewModel[] ViewModels => _stack.ViewModels;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public IViewModel Show(IViewModelDescriptor viewModelDescriptor)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var view = viewBuilder.Build(viewModelDescriptor);
        _stack.Push(
            views: [view],
            popCount: 0
        );

        return view.ViewModel;
    }

    /// <inheritdoc/>
    public TViewModel Show<TViewModel>(params object[] parameters)
        where TViewModel : IViewModel =>
        (TViewModel)Show(new ViewModelDescriptor<TViewModel>(parameters));

    /// <inheritdoc/>
    public Task<IViewModel> ShowAsync(IViewModelDescriptor viewModelDescriptor)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var taskSource = new TaskCompletionSource<IViewModel>();
        var viewModel = Show(viewModelDescriptor);
        viewModel.Disposed
            .Observe()
            .OnEach(() => taskSource.TrySetResult(viewModel))
            .Once();
        return taskSource.Task;
    }

    /// <inheritdoc/>
    public async Task<TViewModel> ShowAsync<TViewModel>(
        params object[] parameters
    )
        where TViewModel : IViewModel =>
        (TViewModel)await ShowAsync(
            new ViewModelDescriptor<TViewModel>(parameters)
        );

    /// <inheritdoc/>
    public void Close(IViewModel viewModel)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        _stack.Close(viewModel);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        _stack.Clear();
    }

    /// <summary>
    /// Dispose of this object.
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
