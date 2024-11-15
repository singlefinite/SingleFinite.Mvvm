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
/// A dialog view model.
/// </summary>
public abstract class DialogViewModel : ViewModel, IDialogViewModel
{
    #region Methods

    /// <summary>
    /// Raises the Closed event.
    /// </summary>
    protected void Close() => _closedSource.RaiseEvent();

    #endregion

    #region Events

    /// <inheritdoc/>
    public EventToken Closed => _closedSource.Token;
    private readonly EventTokenSource _closedSource = new();

    #endregion
}

/// <summary>
/// A dialog view model.
/// </summary>
/// <typeparam name="TContext">The context for the view model.</typeparam>
public abstract class DialogViewModel<TContext> :
    DialogViewModel,
    IViewModel<TContext>,
    IDialogViewModel<TContext>
{
}
