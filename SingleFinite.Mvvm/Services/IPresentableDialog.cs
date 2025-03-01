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
public interface IPresentableDialog : IPresentable
{
    /// <summary>
    /// All currently open dialogs in order from top to bottom with the top
    /// most dialog at index 0.
    /// </summary>
    IViewModel[] ViewModels { get; }

    /// <summary>
    /// Display a dialog.
    /// </summary>
    /// <param name="viewModelDescriptor">
    /// Describes the view model to build.
    /// </param>
    /// <returns>The newly created view model.</returns>
    IViewModel Show(IViewModelDescriptor viewModelDescriptor);

    /// <summary>
    /// Display a dialog.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of view model to build.
    /// </typeparam>
    /// <param name="parameters">
    /// The parameters that will be provided to the view model.
    /// </param>
    /// <returns>The newly created view model.</returns>
    TViewModel Show<TViewModel>(
        params object[] parameters
    )
        where TViewModel : IViewModel;

    /// <summary>
    /// Display a dialog and wait until the dialog is closed.
    /// </summary>
    /// <param name="viewModelDescriptor">
    /// Describes the view model to build.
    /// </param>
    /// <returns>The newly created view model.</returns>
    Task<IViewModel> ShowAsync(IViewModelDescriptor viewModelDescriptor);

    /// <summary>
    /// Display a dialog and wait until the dialog is closed.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of view model to build.
    /// </typeparam>
    /// <param name="parameters">
    /// The parameters that will be provided to the view model.
    /// </param>
    /// <returns>The newly created view model.</returns>
    Task<TViewModel> ShowAsync<TViewModel>(
        params object[] parameters
    )
        where TViewModel : IViewModel;

    /// <summary>
    /// Close the given view model.  If the view model isn't open this method
    /// will have no effect.
    /// </summary>
    /// <param name="viewModel">The view model to close.</param>
    void Close(IViewModel viewModel);

    /// <summary>
    /// Close all currently open view models.
    /// </summary>
    void Clear();
}
