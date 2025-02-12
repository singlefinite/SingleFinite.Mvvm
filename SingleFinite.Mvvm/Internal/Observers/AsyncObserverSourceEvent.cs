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
/// An observer that is the root of an observer chain and has a generic event
/// for a source.
/// </summary>
/// <typeparam name="TEventDelegate">The event delegate type.</typeparam>
internal class AsyncObserverSourceEvent<TEventDelegate> : IAsyncObserver
{
    #region Fields

    /// <summary>
    /// Set to true when this object has been disposed.
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// Action used to unregister event handler.
    /// </summary>
    private readonly Action<TEventDelegate> _unregister;

    /// <summary>
    /// Holds the event handler.
    /// </summary>
    private readonly TEventDelegate _handler;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="register">Action used to register event handler.</param>
    /// <param name="unregister">
    /// Action used to unregister event handler.
    /// </param>
    /// <param name="handler">
    /// Func used to get handler.  The action that raises the Next event
    /// of this observer is passed into the func.
    /// </param>
    public AsyncObserverSourceEvent(
        Action<TEventDelegate> register,
        Action<TEventDelegate> unregister,
        Func<Func<Task>, TEventDelegate> handler
    )
    {
        _unregister = unregister;
        _handler = handler(RaiseNextAsync);
        register(_handler);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Unregister event handler.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        _unregister(_handler);
    }

    /// <summary>
    /// Raise the Next event.
    /// </summary>
    /// <returns>The task from invoking the Next event.</returns>
    private Task RaiseNextAsync() => Next?.Invoke() ?? Task.CompletedTask;

    #endregion

    #region Events

    /// <summary>
    /// Raised when the underlying event is raised. 
    /// </summary>
    public event Func<Task>? Next;

    #endregion
}

/// <summary>
/// An observer that is the root of an observer chain and has a generic event
/// for a source.
/// </summary>
/// <typeparam name="TEventDelegate">The event delegate type.</typeparam>
/// <typeparam name="TArgs">The type of arguments for the observer.</typeparam>
internal class AsyncObserverSourceEvent<TEventDelegate, TArgs> : IAsyncObserver<TArgs>
{
    #region Fields

    /// <summary>
    /// Set to true when this object has been disposed.
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// Action used to unregister event handler.
    /// </summary>
    private readonly Action<TEventDelegate> _unregister;

    /// <summary>
    /// Holds the event handler.
    /// </summary>
    private readonly TEventDelegate _handler;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="register">Action used to register event handler.</param>
    /// <param name="unregister">
    /// Action used to unregister event handler.
    /// </param>
    /// <param name="handler">
    /// Func used to get handler.  The action that raises the Next event
    /// of this observer is passed into the func.
    /// </param>
    public AsyncObserverSourceEvent(
        Action<TEventDelegate> register,
        Action<TEventDelegate> unregister,
        Func<Func<TArgs, Task>, TEventDelegate> handler
    )
    {
        _unregister = unregister;
        _handler = handler(RaiseNextAsync);
        register(_handler);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Unregister event handler.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        _unregister(_handler);
    }

    /// <summary>
    /// Raise the Next event.
    /// </summary>
    /// <param name="args">Arguments passed with the event.</param>
    /// <returns>The task from invoking the Next event.</returns>
    private Task RaiseNextAsync(TArgs args) =>
        Next?.Invoke(args) ?? Task.CompletedTask;

    #endregion

    #region Events

    /// <summary>
    /// Raised when the underlying event is raised. 
    /// </summary>
    public event Func<TArgs, Task>? Next;

    #endregion
}
