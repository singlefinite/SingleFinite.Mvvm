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

namespace SingleFinite.Mvvm.Services;

/// <summary>
/// Service used to display dialogs.  This library doesn't provide an implementation for this service as the display of dialogs
/// is specific to the platform and UI framework in use.  Platform specific assemblies should provide the implementation for
/// this service if supported.
/// </summary>
public interface IPresenterDialog
{
    /// <summary>
    /// If there are any open dialogs this property will be true.
    /// </summary>
    bool IsDialogOpen { get; }

    /// <summary>
    /// Display a dialog view that doesn't return a result.
    /// </summary>
    /// <typeparam name="TDialog">The type of dialog to display a view for.</typeparam>
    /// <returns>A task that completes when the dialog has been closed.</returns>
    Task ShowAsync<TDialog>()
        where TDialog : IDialog;

    /// <summary>
    /// Display a dialog view that doesn't return a result.
    /// </summary>
    /// <typeparam name="TDialog">The type of dialog to display a view for.</typeparam>
    /// <typeparam name="TDialogContext">The type of context to provide to the dialog.</typeparam>
    /// <param name="context">The context to provide to the dialog.</param>
    /// <returns>A task that completes when the dialog has been closed.</returns>
    Task ShowAsync<TDialog, TDialogContext>(TDialogContext context)
        where TDialog : IDialog<TDialogContext>;

    /// <summary>
    /// Display a dialog view that returns a result.
    /// </summary>
    /// <typeparam name="TDialog">The type of dialog to display a view for.</typeparam>
    /// <typeparam name="TResult">The type of result that will be returned by the dialog.</typeparam>
    /// <returns>A task that completes with the result when the dialog has been closed.</returns>
    Task<TResult> ShowAsync<TDialog, TResult>()
        where TDialog : IDialogWithResult<TResult>;

    /// <summary>
    /// Display a dialog view that returns a result.
    /// </summary>
    /// <typeparam name="TDialog">The type of dialog to display a view for.</typeparam>
    /// <typeparam name="TDialogContext">The type of context to provide to the dialog.</typeparam>
    /// <typeparam name="TResult">The type of result that will be returned by the dialog.</typeparam>
    /// <param name="context">The context to provide to the dialog.</param>
    /// <returns>A task that completes when the dialog has been closed.</returns>
    Task<TResult> ShowAsync<TDialog, TDialogContext, TResult>(TDialogContext context)
        where TDialog : IDialogWithResult<TDialogContext, TResult>;

    /// <summary>
    /// Event that is raised when the IsDialogOpen property changes.
    /// </summary>
    EventToken<bool> IsDialogOpenChanged { get; }
}
