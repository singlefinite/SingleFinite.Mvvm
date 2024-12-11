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
/// Observer that will dispose of the observer chain if a given condition is
/// met.
/// </summary>
/// <param name="parent">The parent to this observer.</param>
/// <param name="predicate">
/// The condition that when met will dispose of the oberver chain.
/// </param>
/// <param name="continueOnDispose">
/// If true the next observer in the observer chain will be invoked even when
/// predicate returns true.
/// </param>
internal class AsyncObserverDisposeIf(
    IAsyncObserver parent,
    Func<Task<bool>> predicate,
    bool continueOnDispose
) : AsyncObserverBase(parent)
{
    #region Methods

    /// <summary>
    /// Evaluate the dispose condition and dispose if the condition is met.
    /// </summary>
    /// <returns>
    /// True if the dispose condition is not met, false if it is.
    /// </returns>
    protected async override Task<bool> OnEventAsync()
    {
        var willDispose = await predicate();
        if (willDispose)
            Dispose();

        return continueOnDispose || !willDispose;
    }

    #endregion
}

/// <summary>
/// Observer that will dispose of the observer chain if a given condition is
/// met.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
/// <param name="predicate">
/// The condition that when met will dispose of the oberver chain.
/// </param>
/// <param name="continueOnDispose">
/// If true the next observer in the observer chain will be invoked even when
/// predicate returns true.
/// </param>
internal class AsyncObserverDisposeIf<TArgs>(
    IAsyncObserver<TArgs> parent,
    Func<TArgs, Task<bool>> predicate,
    bool continueOnDispose
) : AsyncObserverBase<TArgs>(parent)
{
    #region Methods

    /// <summary>
    /// Evaluate the dispose condition and dispose if the condition is met.
    /// </summary>
    /// <param name="args">Not used.</param>
    /// <returns>
    /// True if the dispose condition is not met, false if it is.
    /// </returns>
    protected async override Task<bool> OnEventAsync(TArgs args)
    {
        var willDispose = await predicate(args);
        if (willDispose)
            Dispose();

        return continueOnDispose || !willDispose;
    }

    #endregion
}

/// <summary>
/// Observer that will dispose of the observer chain if a given condition is
/// met.
/// </summary>
/// <typeparam name="TSender">
/// The type of sender passed with observed events.
/// </typeparam>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
/// <param name="predicate">
/// The condition that when met will dispose of the oberver chain.
/// </param>
/// <param name="continueOnDispose">
/// If true the next observer in the observer chain will be invoked even when
/// predicate returns true.
/// </param>
internal class AsyncObserverDisposeIf<TSender, TArgs>(
    IAsyncObserver<TSender, TArgs> parent,
    Func<TSender, TArgs, Task<bool>> predicate,
    bool continueOnDispose
) : AsyncObserverBase<TSender, TArgs>(parent)
{
    #region Methods

    /// <summary>
    /// Evaluate the dispose condition and dispose if the condition is met.
    /// </summary>
    /// <param name="sender">Not used.</param>
    /// <param name="args">Not used.</param>
    /// <returns>
    /// True if the dispose condition is not met, false if it is.
    /// </returns>
    protected async override Task<bool> OnEventAsync(TSender sender, TArgs args)
    {
        var willDispose = await predicate(sender, args);
        if (willDispose)
            Dispose();

        return continueOnDispose || !willDispose;
    }

    #endregion
}
