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

namespace SingleFinite.Mvvm.Services;

/// <summary>
/// Service used to display dialogs.
/// </summary>
public interface IPresenterDialog
{
    /// <summary>
    /// If there are any open dialogs this property will be true.
    /// </summary>
    bool IsDialogOpen { get; }

    /// <summary>
    /// Display a dialog view.
    /// </summary>
    /// <param name="viewModelDescriptor">Describes the view to build.</param>
    /// <returns>The context for the dialog that is shown.</returns>
    IDialogContext Show(IViewModelDescriptor viewModelDescriptor);

    /// <summary>
    /// Display a dialog view.
    /// </summary>
    /// <typeparam name="TDialogViewModel">
    /// The type of view model to display a dialog view for.
    /// </typeparam>
    /// <returns>The context for the dialog that is shown.</returns>
    IDialogContext<TDialogViewModel> Show<TDialogViewModel>()
        where TDialogViewModel : IDialogViewModel;

    /// <summary>
    /// Display a dialog view.
    /// </summary>
    /// <typeparam name="TDialogViewModel">
    /// The type of view model to display a dialog view for.
    /// </typeparam>
    /// <typeparam name="TDialogViewModelContext">
    /// The type of context to provide to the view model.
    /// </typeparam>
    /// <param name="context">The context to provide to the dialog.</param>
    /// <returns>The context for the dialog that is shown.</returns>
    IDialogContext<TDialogViewModel> Show<TDialogViewModel, TDialogViewModelContext>(
        TDialogViewModelContext context
    )
        where TDialogViewModel : IDialogViewModel<TDialogViewModelContext>;

    /// <summary>
    /// Event that is raised when the IsDialogOpen property changes.
    /// </summary>
    EventToken<bool> IsDialogOpenChanged { get; }

    /// <summary>
    /// Event that is raised when a dialog is opened.
    /// </summary>
    EventToken<IView> DialogOpened { get; }
}
