// MIT License
// Copyright (c) 2024 Single Finite
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR 
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace SingleFinite.Mvvm;

/// <summary>
/// Base implementation of IDialog.
/// </summary>
public abstract class Dialog : ViewModel, IDialog
{
    #region Methods

    /// <summary>
    /// Close this dialog.
    /// </summary>
    protected void CloseDialog() => _dialogClosedSource.RaiseEvent();

    #endregion

    #region Events

    /// <inheritdoc/>
    public EventToken DialogClosed => _dialogClosedSource.Token;
    private readonly EventTokenSource _dialogClosedSource = new();

    #endregion
}

/// <summary>
/// Base implementation of IDialog with view model context.
/// </summary>
/// <typeparam name="TDialogContext">The type of context that must be provided to the dialog.</typeparam>
public abstract class Dialog<TDialogContext> : ViewModel, IDialog<TDialogContext>
{
    #region Methods

    /// <summary>
    /// Close this dialog.
    /// </summary>
    protected void CloseDialog() => _dialogClosedSource.RaiseEvent();

    #endregion

    #region Events

    /// <inheritdoc/>
    public EventToken DialogClosed => _dialogClosedSource.Token;
    private readonly EventTokenSource _dialogClosedSource = new();

    #endregion
}

/// <summary>
/// Base implementation of IDialogWithResult.
/// </summary>
/// <typeparam name="TResult">The type of result returned by the dialog.</typeparam>
public abstract class DialogWithResult<TResult> : ViewModel, IDialogWithResult<TResult>
{
    #region Methods

    /// <summary>
    /// Close this dialog.
    /// </summary>
    /// <param name="result">The result to close the dialog with.</param>
    protected void CloseDialog(TResult result) => _dialogClosedSource.RaiseEvent(result);

    #endregion

    #region Events

    /// <inheritdoc/>
    public EventToken<TResult> DialogClosed => _dialogClosedSource.Token;
    private readonly EventTokenSource<TResult> _dialogClosedSource = new();

    #endregion
}

/// <summary>
/// Base implementation of IDialogWithResult.
/// </summary>
/// <typeparam name="TDialogContext">The type of context that must be provided to the dialog.</typeparam>
/// <typeparam name="TResult">The type of result returned by the dialog.</typeparam>
public abstract class DialogWithResult<TDialogContext, TResult> : ViewModel<TDialogContext>, IDialogWithResult<TResult>
{
    #region Methods

    /// <summary>
    /// Close this dialog.
    /// </summary>
    /// <param name="result">The result to close the dialog with.</param>
    protected void CloseDialog(TResult result) => _dialogClosedSource.RaiseEvent(result);

    #endregion

    #region Events

    /// <inheritdoc/>
    public EventToken<TResult> DialogClosed => _dialogClosedSource.Token;
    private readonly EventTokenSource<TResult> _dialogClosedSource = new();

    #endregion
}
