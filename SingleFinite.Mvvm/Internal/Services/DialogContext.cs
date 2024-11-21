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

namespace SingleFinite.Mvvm.Internal.Services;

/// <summary>
/// Implementation for <see cref="IDialogContext"/>.
/// </summary>
internal class DialogContext : IDialogContext
{
    #region Fields

    /// <summary>
    /// Source for the task.
    /// </summary>
    private readonly TaskCompletionSource _taskSource = new();

    /// <summary>
    /// Set to true when the dialog has been closed.
    /// </summary>
    private bool _isClosed;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="view">The view for the context.</param>
    /// <param name="isModal">Indicates if the view is modal.</param>
    public DialogContext(IView view, bool isModal)
    {
        View = view;
        IsModal = isModal;

        var viewModel = View.ViewModel as IDialogViewModel ??
            throw new InvalidOperationException(
                "An IDialogViewModel view model is required."
            );

        viewModel.Closed.Register(Close);
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public IView View { get; }

    /// <inheritdoc/>
    public bool IsModal { get; }

    /// <inheritdoc/>
    public Task Task => _taskSource.Task;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public void Close()
    {
        if (_isClosed)
            return;

        _isClosed = true;

        View.ViewModel.Deactivate();
        View.ViewModel.Dispose();
        _closedSource.RaiseEvent();
        _taskSource.TrySetResult();
    }

    #endregion

    #region Events

    /// <inheritdoc/>
    public EventToken Closed => _closedSource.Token;
    private readonly EventTokenSource _closedSource = new();

    #endregion
}

/// <summary>
/// Implementation for <see cref="IDialogContext"/>.
/// </summary>
/// <param name="view">The value for View property.</param>
/// <param name="isModal">Indicates if the view is modal.</param>
/// <typeparam name="TDialogViewModel">
/// The type of view model displayed in the dialog.
/// </typeparam>
internal class DialogContext<TDialogViewModel>(
    IView<TDialogViewModel> view,
    bool isModal
) :
    DialogContext(view, isModal),
    IDialogContext<TDialogViewModel>
    where TDialogViewModel : IDialogViewModel
{
    /// <inheritdoc/>
    IView<TDialogViewModel> IDialogContext<TDialogViewModel>.View =>
        (IView<TDialogViewModel>)View;
}
