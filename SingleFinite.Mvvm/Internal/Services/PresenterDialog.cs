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
/// Implementation of <see cref="IPresenterDialog"/>.
/// </summary>
/// <param name="viewBuilder">Used to build views.</param>
internal class PresenterDialog(
    IViewBuilder viewBuilder
) :
    IPresenterDialog,
    IDisposable
{
    #region Fields

    /// <summary>
    /// Set to true when this object has been disposed.
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// Holds the stack of dialog views with the view at index 0 considered
    /// to be on top of the stack.
    /// </summary>
    private readonly List<IView> _dialogStack = [];

    #endregion

    #region Properties

    /// <inheritdoc/>
    public IView? Current
    {
        get;
        private set
        {
            if (field == value)
                return;

            field = value;
            _currentChangedSource.RaiseEvent(value);
        }
    }

    /// <inheritdoc/>
    public IViewModel[] Dialogs { get; private set; } = [];

    #endregion

    #region Methods

    /// <summary>
    /// Update the Dialogs list from the current list dialog views.
    /// </summary>
    private void UpdateDialogs() =>
        Dialogs = _dialogStack.Select(view => view.ViewModel).ToArray();

    /// <inheritdoc/>
    public IViewModel Show(IViewModelDescriptor viewModelDescriptor)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (_dialogStack.Count > 0)
            _dialogStack[0].ViewModel.Deactivate();

        var view = viewBuilder.Build(viewModelDescriptor);

        _dialogStack.Insert(0, view);
        UpdateDialogs();

        view.ViewModel.Activate();
        Current = view;
        return view.ViewModel;
    }

    /// <inheritdoc/>
    public TViewModel Show<TViewModel>()
        where TViewModel : IViewModel =>
        (TViewModel)Show(new ViewModelDescriptor<TViewModel>());

    /// <inheritdoc/>
    public TViewModel Show<TViewModel, TViewModelContext>(TViewModelContext context)
        where TViewModel : IViewModel<TViewModelContext> =>
        (TViewModel)Show(new ViewModelDescriptor<TViewModel, TViewModelContext>(context));

    /// <inheritdoc/>
    public Task<IViewModel> ShowAsync(IViewModelDescriptor viewModelDescriptor)
    {
        var taskSource = new TaskCompletionSource<IViewModel>();
        var viewModel = Show(viewModelDescriptor);
        viewModel.Disposed.Register(
            observer =>
            {
                observer.Dispose();
                taskSource.TrySetResult(viewModel);
            }
        );
        return taskSource.Task;
    }

    /// <inheritdoc/>
    public async Task<TViewModel> ShowAsync<TViewModel>()
        where TViewModel : IViewModel =>
        (TViewModel)await ShowAsync(new ViewModelDescriptor<TViewModel>());

    /// <inheritdoc/>
    public async Task<TViewModel> ShowAsync<TViewModel, TViewModelContext>(
        TViewModelContext context
    )
        where TViewModel : IViewModel<TViewModelContext> =>
        (TViewModel)await ShowAsync(
            new ViewModelDescriptor<TViewModel, TViewModelContext>(context)
        );

    /// <inheritdoc/>
    public void Close(IViewModel viewModel)
    {
        var index = _dialogStack.FindIndex(view => view.ViewModel == viewModel);
        if (index == -1)
            return;

        if (index == 0)
        {
            // Remove view from top of stack and make the next dialog
            // active.
            //
            viewModel.Deactivate();
            viewModel.Dispose();

            _dialogStack.RemoveAt(0);
            UpdateDialogs();

            var top = _dialogStack.FirstOrDefault();
            top?.ViewModel?.Activate();
            Current = top;
        }
        else
        {
            // Remove view from within the stack and leave Current unchanged.
            //
            viewModel.Dispose();

            _dialogStack.RemoveAt(index);
            UpdateDialogs();
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _dialogStack.FirstOrDefault()?.ViewModel?.Deactivate();
        _dialogStack.ForEach(view => view.ViewModel.Dispose());
        _dialogStack.Clear();
        UpdateDialogs();
        Current = null;
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
    public EventToken<IView?> CurrentChanged => _currentChangedSource.Token;
    private readonly EventTokenSource<IView?> _currentChangedSource = new();

    #endregion
}
