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
/// A view model that should be displayed as a dialog.
/// </summary>
public interface IDialog : IViewModel
{
    /// <summary>
    /// Event that is raised when the dialog has requested to be closed.
    /// </summary>
    public EventToken DialogClosed { get; }
}

/// <summary>
/// A view model that should be displayed as a dialog and accepts a view model context.
/// </summary>
/// <typeparam name="TDialogContext">The type of context that must be provided to the dialog.</typeparam>
public interface IDialog<TDialogContext> : IViewModel<TDialogContext>
{
    /// <summary>
    /// Event that is raised when the dialog has requested to be closed.
    /// </summary>
    public EventToken DialogClosed { get; }
}

/// <summary>
/// A view model that should be displayed as a dialog.
/// </summary>
/// <typeparam name="TResult">The type of result that will be returned from the dialog.</typeparam>
public interface IDialogWithResult<TResult> : IViewModel
{
    /// <summary>
    /// Event that is raised when the dialog has requested to be closed.
    /// </summary>
    public EventToken<TResult> DialogClosed { get; }
}

/// <summary>
/// A view model that should be displayed as a dialog and accepts a view model context.
/// </summary>
/// <typeparam name="TDialogContext">The type of context that must be provided to the dialog.</typeparam>
/// <typeparam name="TResult">The type of result that will be returned from the dialog.</typeparam>
public interface IDialogWithResult<TDialogContext, TResult> : IViewModel<TDialogContext>
{
    /// <summary>
    /// Event that is raised when the dialog has requested to be closed.
    /// </summary>
    public EventToken DialogClosed { get; }
}
