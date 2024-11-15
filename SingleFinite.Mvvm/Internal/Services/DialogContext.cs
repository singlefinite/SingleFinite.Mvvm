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
/// Implementation for IDialogContext.
/// </summary>
/// <param name="view">The value for View property.</param>
/// <param name="closed">The value for Closed property.</param>
/// <param name="close">The action to invoke when Close is called.</param>
internal class DialogContext(
    IView view,
    Task closed,
    Action close
) : IDialogContext
{
    #region Fields

    /// <summary>
    /// Indicates if the dialog has been closed.
    /// </summary>
    private bool _isClosed = false;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public IView View { get; } = view;

    /// <inheritdoc/>
    public Task Closed { get; } = closed;

    /// <inheritdoc/>
    public void Close()
    {
        if (_isClosed)
            return;

        _isClosed = true;
        close();
    }

    #endregion
}
