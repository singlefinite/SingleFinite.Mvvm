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
/// Observer that listens for when the observer is disposed.
/// This class will only listen for dispose from observers further down the
/// chain of observers and will not listen for dispose from observers that
/// preceed it in the chain of observers.
/// </summary>
/// <param name="parent">The parent to this observer.</param>
internal class ObserverDisposeListener(
    IObserver parent
) : ObserverBase(parent)
{
    #region Methods

    /// <summary>
    /// This method doesn't have any logic.
    /// </summary>
    /// <returns>This method always returns true.</returns>
    protected override bool OnEvent() => true;

    /// <summary>
    /// Invoke the onDispose method that was provided.
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
        Disposed?.Invoke();
    }

    #endregion

    #region Events

    /// <summary>
    /// Event that is raised when the observer has been disposed.
    /// </summary>
    public event Action? Disposed;

    #endregion
}

/// <summary>
/// Observer that listens for when the observer is disposed.
/// This class will only listen for dispose from observers further down the
/// chain of observers and will not listen for dispose from observers that
/// preceed it in the chain of observers.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
internal class ObserverDisposeListener<TArgs>(
    IObserver<TArgs> parent
) : ObserverBase<TArgs>(parent)
{
    #region Methods

    /// <summary>
    /// This method doesn't have any logic.
    /// </summary>
    /// <returns>This method always returns true.</returns>
    protected override bool OnEvent(TArgs args) => true;

    /// <summary>
    /// Invoke the onDispose method that was provided.
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
        Disposed?.Invoke();
    }

    #endregion

    #region Events

    /// <summary>
    /// Event that is raised when the observer has been disposed.
    /// </summary>
    public event Action? Disposed;

    #endregion
}
