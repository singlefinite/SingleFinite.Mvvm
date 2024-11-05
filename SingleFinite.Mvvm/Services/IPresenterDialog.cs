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
    /// Display a simple alert message.
    /// </summary>
    /// <param name="title">The title for the alert.</param>
    /// <param name="message">The message for the alert.</param>
    /// <param name="closeButtonLabel">The label to display on the close button.</param>
    /// <param name="primaryButtonLabel">
    /// Optional label to display on the primary button.
    /// If this is null a primary button will not be displayed.
    /// </param>
    /// <param name="secondaryButtonLabel">
    /// Optional label to display on the secondary button.
    /// If this is null a primary button will not be displayed.
    /// </param>
    /// <returns>The resulting action selected by the user.</returns>
    Task<AlertResult> ShowAlertAsync(
        string title,
        string message,
        string closeButtonLabel,
        string? primaryButtonLabel = null,
        string? secondaryButtonLabel = null,
        AlertResult? defaultResult = null
    );

    /// <summary>
    /// Display a custom view as a dialog.
    /// This method will not return until the dialog has been closed.  After the dialog is closed the view model that was displayed
    /// is returned by this method so that any custom result the caller may wish to retrieve can be retrieved from the custom view model.
    /// Note, however that the view model returned will always be in the disposed state.
    /// </summary>
    /// <typeparam name="TViewModel">The type of view model to display a view for.</typeparam>
    /// <returns>The built view model after the dialog has been closed.</returns>
    Task<TViewModel> ShowAsync<TViewModel>()
        where TViewModel : IViewModel;

    /// <summary>
    /// Display a custom view as a dialog.
    /// This method will not return until the dialog has been closed.  After the dialog is closed the view model that was displayed
    /// is returned by this method so that any custom result the caller may wish to retrieve can be retrieved from the custom view model.
    /// Note, however that the view model returned will always be in the disposed state.
    /// </summary>
    /// <typeparam name="TViewModel">The type of view model to display a view for.</typeparam>
    /// <typeparam name="TViewModelContext">The type of context to provide to the view model.</typeparam>
    /// <param name="context">The context to provide to the view model.</param>
    /// <returns>The built view model after the dialog has been closed.</returns>
    Task<TViewModel> ShowAsync<TViewModel, TViewModelContext>(TViewModelContext context)
        where TViewModel : IViewModel<TViewModelContext>;

    /// <summary>
    /// Possible results from calling the <see cref="ShowAlertAsync(string, string, string, string?, string?)"/> method.
    /// </summary>
    public enum AlertResult
    {
        /// <summary>
        /// The user clicked the close button.
        /// </summary>
        Close,

        /// <summary>
        /// The user clicked the primary button.
        /// </summary>
        Primary,

        /// <summary>
        /// The user clicked the secondary button.
        /// </summary>
        Secondary
    }
}
