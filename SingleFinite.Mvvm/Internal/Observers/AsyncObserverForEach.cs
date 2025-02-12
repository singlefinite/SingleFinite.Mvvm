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

namespace SingleFinite.Mvvm.Internal.Observers;

/// <summary>
/// An observer that invokes the given callback whenever an event is observed.
/// </summary>
/// <param name="parent">The parent to this observer.</param>
/// <param name="callback">
/// The callback to invoke whenever an event is observed.
/// </param>
internal class AsyncObserverForEach(
    IAsyncObserver parent,
    Func<Task> callback
) : AsyncObserverBase(parent)
{
    #region Methods

    /// <summary>
    /// Invoke the provided callback.
    /// </summary>
    /// <returns>This method always returns true.</returns>
    protected async override Task<bool> OnEventAsync()
    {
        await callback();
        return true;
    }

    #endregion
}

/// <summary>
/// An observer that invokes the given callback whenever an event is observed.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
/// <param name="callback">
/// The callback to invoke whenever an event is observed.
/// </param>
internal class AsyncObserverForEach<TArgs>(
    IAsyncObserver<TArgs> parent,
    Func<TArgs, Task> callback
) : AsyncObserverBase<TArgs>(parent)
{
    #region Methods

    /// <summary>
    /// Invoke the provided callback.
    /// </summary>
    /// <param name="args">
    /// Arguments passed with the observed event that are passed to the
    /// callback.
    /// </param>
    /// <returns>This method always returns true.</returns>
    protected async override Task<bool> OnEventAsync(TArgs args)
    {
        await callback(args);
        return true;
    }

    #endregion
}
