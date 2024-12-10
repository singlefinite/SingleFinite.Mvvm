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
internal class ObserverDisposeIf(
    IObserver parent,
    Func<bool> predicate,
    bool continueOnDispose
) : ObserverBase(parent)
{
    #region Methods

    /// <summary>
    /// Evaluate the dispose condition and dispose if the condition is met.
    /// </summary>
    /// <returns>
    /// True if the dispose condition is not met, false if it is.
    /// </returns>
    protected override bool OnEvent()
    {
        var willDispose = predicate();
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
internal class ObserverDisposeIf<TArgs>(
    IObserver<TArgs> parent,
    Func<TArgs, bool> predicate,
    bool continueOnDispose
) : ObserverBase<TArgs>(parent)
{
    #region Methods

    /// <summary>
    /// Evaluate the dispose condition and dispose if the condition is met.
    /// </summary>
    /// <param name="args">Not used.</param>
    /// <returns>
    /// True if the dispose condition is not met, false if it is.
    /// </returns>
    protected override bool OnEvent(TArgs args)
    {
        var willDispose = predicate(args);
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
internal class ObserverDisposeIf<TSender, TArgs>(
    IObserver<TSender, TArgs> parent,
    Func<TSender, TArgs, bool> predicate,
    bool continueOnDispose
) : ObserverBase<TSender, TArgs>(parent)
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
    protected override bool OnEvent(TSender sender, TArgs args)
    {
        var willDispose = predicate(sender, args);
        if (willDispose)
            Dispose();

        return continueOnDispose || !willDispose;
    }

    #endregion
}
