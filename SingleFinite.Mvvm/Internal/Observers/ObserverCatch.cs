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
/// An observer that catches exceptions thrown in the observer chain.
/// </summary>
/// <param name="parent">The parent to this observer.</param>
/// <param name="callback">
/// The callback to invoke whenever an exception is caught.
/// If the callback returns false the exception will continue up the observer
/// chain.  If the callback returns true the exception will not continue up
/// the observer chain.
/// </param>
internal class ObserverCatch(
    IObserver parent,
    Func<Exception, bool> callback
) : ObserverBase(parent)
{
    #region Methods

    /// <summary>
    /// Catch any exceptions that are thrown below this observer in the chain
    /// and pass them to the callback function.
    /// </summary>
    /// <returns>This method always returns false.</returns>
    protected override bool OnEvent()
    {
        try
        {
            RaiseNextEvent();
        }
        catch (Exception ex)
        {
            if (!callback(ex))
                throw;
        }

        return false;
    }

    #endregion
}

/// <summary>
/// An observer that catches exceptions thrown in the observer chain.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
/// <param name="callback">
/// The callback to invoke whenever an exception is caught.
/// If the callback returns false the exception will continue up the observer
/// chain.  If the callback returns true the exception will not continue up
/// the observer chain.
/// </param>
internal class ObserverCatch<TArgs>(
    IObserver<TArgs> parent,
    Func<TArgs, Exception, bool> callback
) : ObserverBase<TArgs>(parent)
{
    #region Methods

    /// <summary>
    /// Catch any exceptions that are thrown below this observer in the chain
    /// and pass them to the callback function.
    /// </summary>
    /// <param name="args">
    /// Arguments passed with the observed event that are passed to the
    /// callback.
    /// </param>
    /// <returns>This method always returns false.</returns>
    protected override bool OnEvent(TArgs args)
    {
        try
        {
            RaiseNextEvent(args);
        }
        catch (Exception ex)
        {
            if (!callback(args, ex))
                throw;
        }

        return false;
    }

    #endregion
}

/// <summary>
/// An observer that catches exceptions thrown in the observer chain.
/// </summary>
/// <typeparam name="TSender">
/// The type of sender passed with observed events.
/// </typeparam>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
/// <param name="callback">
/// The callback to invoke whenever an exception is caught.
/// If the callback returns false the exception will continue up the observer
/// chain.  If the callback returns true the exception will not continue up
/// the observer chain.
/// </param>
internal class ObserverCatch<TSender, TArgs>(
    IObserver<TSender, TArgs> parent,
    Func<TSender, TArgs, Exception, bool> callback
) : ObserverBase<TSender, TArgs>(parent)
{
    #region Methods

    /// <summary>
    /// Catch any exceptions that are thrown below this observer in the chain
    /// and pass them to the callback function.
    /// </summary>
    /// <param name="sender">
    /// Sender passed with the observed event that is passed to the callback.
    /// </param>
    /// <param name="args">
    /// Arguments passed with the observed event that are passed to the
    /// callback.
    /// </param>
    /// <returns>This method always returns false.</returns>
    protected override bool OnEvent(TSender sender, TArgs args)
    {
        try
        {
            RaiseNextEvent(sender, args);
        }
        catch (Exception ex)
        {
            if (!callback(sender, args, ex))
                throw;
        }

        return false;
    }

    #endregion
}
