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
/// Base class for common observer class behavior.
/// </summary>
internal abstract class ObserverBase : IObserver
{
    #region Fields

    /// <summary>
    /// Holds the parent to this observer.
    /// </summary>
    private readonly IObserver _parent;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    public ObserverBase(IObserver parent)
    {
        _parent = parent;
        _parent.Next += () =>
        {
            if (OnEvent())
                RaiseNextEvent();
        };
    }

    #endregion

    #region Methods

    /// <summary>
    /// This method is invoked when the parent event is raised.
    /// </summary>
    /// <returns>
    /// True if this observers event should be raised which would continue down
    /// the chain of observers. False if this observers event should not be
    /// raised which would stop the remaining chain of observers from seeing the
    /// event.
    /// </returns>
    protected abstract bool OnEvent();

    /// <summary>
    /// Raise the Event for this observer which moves execution down the chain.
    /// </summary>
    protected void RaiseNextEvent() => Next?.Invoke();

    /// <summary>
    /// Invoke the parent Dispose method.  The expectation is that the dispose
    /// will eventually reach the first observer in the chain which will
    /// unregister the source event handler.
    /// </summary>
    public virtual void Dispose() => _parent.Dispose();

    #endregion

    #region Events

    /// <summary>
    /// The event that is raised when handling of the parent event should
    /// continue the next observers down the chain of observers.
    /// </summary>
    public event Action? Next;

    #endregion
}

/// <summary>
/// Base class for observer classes.
/// </summary>
internal abstract class ObserverBase<TArgs> : IObserver<TArgs>
{
    #region Fields

    /// <summary>
    /// Holds the parent to this observer.
    /// </summary>
    private readonly IObserver<TArgs> _parent;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    public ObserverBase(IObserver<TArgs> parent)
    {
        _parent = parent;
        _parent.Next += args =>
        {
            if (OnEvent(args))
                RaiseNextEvent(args);
        };
    }

    #endregion

    #region Methods

    /// <summary>
    /// This method is invoked when the parent event is raised.
    /// </summary>
    /// <returns>
    /// True if this observers event should be raised which would continue down
    /// the chain of observers. False if this observers event should not be
    /// raised which would stop the remaining chain of observers from seeing the
    /// event.
    /// </returns>
    protected abstract bool OnEvent(TArgs args);

    /// <summary>
    /// Raise the Event for this observer which moves execution down the chain.
    /// </summary>
    /// <param name="args">The args to pass with the event.</param>
    protected void RaiseNextEvent(TArgs args) => Next?.Invoke(args);

    /// <summary>
    /// Invoke the parent Dispose method.  The expectation is that the dispose
    /// will eventually reach the first observer in the chain which will
    /// unregister the source event handler.
    /// </summary>
    public virtual void Dispose() => _parent.Dispose();

    #endregion

    #region Events

    /// <summary>
    /// The event that is raised when handling of the parent event should
    /// continue the next observers down the chain of observers.
    /// </summary>
    public event Action<TArgs>? Next;

    #endregion
}
