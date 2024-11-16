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

namespace SingleFinite.Mvvm;

/// <summary>
/// The context for a dialog that is displaying a view.
/// </summary>
public interface IDialogContext
{
    /// <summary>
    /// The view displayed in the dialog.
    /// </summary>
    IView View { get; }

    /// <summary>
    /// A task that completes when the dialog has been closed.
    /// </summary>
    Task Closed { get; }

    /// <summary>
    /// Close the dialog.
    /// </summary>
    void Close();
}

/// <summary>
/// The context for a dialog that is displaying a view.
/// </summary>
/// <typeparam name="TDialogViewModel">
/// The type of view model the view is displaying.
/// </typeparam>
public interface IDialogContext<TDialogViewModel> : IDialogContext
    where TDialogViewModel : IDialogViewModel
{
    /// <inheritdoc/>
    IView IDialogContext.View => View;

    /// <summary>
    /// The view displayed in the dialog.
    /// </summary>
    new IView<TDialogViewModel> View { get; }
}
