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
/// An observer that prevents observed events from continuing down the observer
/// chain unless a given condition is met.
/// </summary>
/// <param name="parent">The parent to this observer.</param>
/// <param name="predicate">
/// The condition that must be met to allow observed events to continue down the
/// observer chain.
/// </param>
internal class AsyncObserverWhere(
    IAsyncObserver parent,
    Func<Task<bool>> predicate
) : AsyncObserverBase(parent)
{
    #region Methods

    /// <summary>
    /// Check if the condition is met for the observed event.
    /// </summary>
    /// <returns>
    /// True if the provided condition is met, false if it isn't.
    /// </returns>
    protected async override Task<bool> OnEventAsync() => await predicate();

    #endregion
}

/// <summary>
/// An observer that prevents observed events from continuing down the observer
/// chain unless a given condition is met.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
/// <param name="predicate">
/// The condition that must be met to allow observed events to continue down the
/// observer chain.
/// </param>
internal class AsyncObserverWhere<TArgs>(
    IAsyncObserver<TArgs> parent,
    Func<TArgs, Task<bool>> predicate
) : AsyncObserverBase<TArgs>(parent)
{
    #region Methods

    /// <summary>
    /// Check if the condition is met for the observed event.
    /// </summary>
    /// <param name="args">
    /// Arguments passed with the observed event that are passed to the
    /// predicate.
    /// </param>
    /// <returns>
    /// True if the provided condition is met, false if it isn't.
    /// </returns>
    protected async override Task<bool> OnEventAsync(TArgs args) =>
        await predicate(args);

    #endregion
}

/// <summary>
/// An observer that prevents observed events from continuing down the observer
/// chain unless a given condition is met.
/// </summary>
/// <typeparam name="TSender">
/// The type of sender passed with observed events.
/// </typeparam>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
/// <param name="predicate">
/// The condition that must be met to allow observed events to continue down the
/// observer chain.
/// </param>
internal class AsyncObserverWhere<TSender, TArgs>(
    IAsyncObserver<TSender, TArgs> parent,
    Func<TSender, TArgs, Task<bool>> predicate
) : AsyncObserverBase<TSender, TArgs>(parent)
{
    #region Methods

    /// <summary>
    /// Check if the condition is met for the observed event.
    /// </summary>
    /// <param name="sender">
    /// Sender passed with the observed event that is passed to the predicate.
    /// </param>
    /// <param name="args">
    /// Arguments passed with the observed event that are passed to the
    /// predicate.
    /// </param>
    /// <returns>
    /// True if the provided condition is met, false if it isn't.
    /// </returns>
    protected async override Task<bool> OnEventAsync(
        TSender sender,
        TArgs args
    ) =>
        await predicate(sender, args);

    #endregion
}
