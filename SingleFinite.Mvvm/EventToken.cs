// MIT License
// Copyright (c) 2024 Single Finite
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR 
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using SingleFinite.Mvvm.Internal;

namespace SingleFinite.Mvvm;

/// <summary>
/// This class wraps an event to make registering and unregistering callbacks for the event more convenient when working with
/// dependency injection scopes.  EventToken instances are created by instances of the <see cref="EventTokenSource"/> class.
/// </summary>
public sealed class EventToken
{
    #region Constructors

    /// <summary>
    /// Instances of this class are created with <see cref="EventTokenSource"/> objects.
    /// </summary>
    internal EventToken()
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Register the action to call when this event is raised.
    /// </summary>
    /// <param name="callback">The callback to register.</param>
    /// <returns>A registration object that when disposed will unregister the callback.</returns>
    public IDisposable Register(Action callback)
    {
        ActionEvent += callback;
        return new ActionOnDispose(() => ActionEvent -= callback);
    }

    /// <summary>
    /// Register the action to call when this event is raised.
    /// A disposable is passed to the callback that when disposed will unregister the callback.
    /// </summary>
    /// <param name="callback">
    /// The action that will be called when the event has been raised.
    /// </param>
    /// <returns>A registration object that when disposed will unregister the callback.</returns>
    public IDisposable Register(Action<IDisposable> callback)
    {
        IDisposable? registration = null;

        void Handler()
        {
            callback(
                registration ?? throw new NullReferenceException("registration is null")
            );
        }

        registration = new ActionOnDispose(() => ActionEvent -= Handler);

        ActionEvent += Handler;

        return registration;
    }

    /// <summary>
    /// Raise the event for this token.
    /// </summary>
    internal void RaiseEvent() => ActionEvent?.Invoke();

    #endregion

    #region Events

    /// <summary>
    /// The underlying event.
    /// </summary>
    public event Action? ActionEvent;

    #endregion
}

/// <summary>
/// This class wraps an event to make registering and unregistering callbacks for the event more convenient when working with
/// dependency injection scopes.  EventToken instances are created by instances of the <see cref="EventTokenSource"/> class.
/// </summary>
/// <typeparam name="TArgs">The type of arguments passed with the event.</typeparam>
public sealed class EventToken<TArgs>
{
    #region Constructors

    /// <summary>
    /// Instances of this class are created with <see cref="EventTokenSource"/> objects.
    /// </summary>
    internal EventToken()
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Register the action to call when this event is raised.
    /// </summary>
    /// <param name="callback">The callback to register.</param>
    /// <returns>A registration object that when disposed will unregister the callback.</returns>
    public IDisposable Register(Action<TArgs> callback)
    {
        ActionEvent += callback;
        return new ActionOnDispose(() => ActionEvent -= callback);
    }

    /// <summary>
    /// Register the action to call when this event is raised.
    /// A disposable is passed to the callback that when disposed will unregister the callback.
    /// </summary>
    /// <param name="callback">The callback to register.</param>
    /// <returns>A registration object that when disposed will unregister the callback.</returns>
    public IDisposable Register(Action<TArgs, IDisposable> callback)
    {
        IDisposable? registration = null;

        void Handler(TArgs args)
        {
            callback(
                args,
                registration ?? throw new NullReferenceException("registration is null")
            );
        }

        registration = new ActionOnDispose(() => ActionEvent -= Handler);

        ActionEvent += Handler;

        return registration;
    }

    /// <summary>
    /// Raise the event for this token.
    /// </summary>
    /// <param name="args">The args included with the event.</param>
    internal void RaiseEvent(TArgs args) => ActionEvent?.Invoke(args);

    #endregion

    #region Events

    /// <summary>
    /// The underlying event.
    /// </summary>
    public event Action<TArgs>? ActionEvent;

    #endregion
}

/// <summary>
/// This class wraps an event to make registering and unregistering callbacks for the event more convenient when working with
/// dependency injection scopes.  EventToken instances are created by instances of the <see cref="EventTokenSource"/> class.
/// </summary>
/// <typeparam name="TSender">The type of sender that raises the event.</typeparam>
/// <typeparam name="TArgs">The type of arguments passed with the event.</typeparam>
public sealed class EventToken<TSender, TArgs>
{
    #region Constructors

    /// <summary>
    /// Instances of this class are created with <see cref="EventTokenSource"/> objects.
    /// </summary>
    internal EventToken()
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Register the action to call when this event is raised.
    /// </summary>
    /// <param name="callback">The callback to register.</param>
    /// <returns>A registration object that when disposed will unregister the callback.</returns>
    public IDisposable Register(Action<TSender, TArgs> callback)
    {
        ActionEvent += callback;
        return new ActionOnDispose(() => ActionEvent -= callback);
    }

    /// <summary>
    /// Register the action to call when this event is raised.
    /// A disposable is passed to the callback that when disposed will unregister the callback.
    /// </summary>
    /// <param name="callback">
    /// The action that will be called when the event has been raised.
    /// </param>
    /// <returns>A registration object that when disposed will unregister the callback.</returns>
    public IDisposable Register(Action<TSender, TArgs, IDisposable> callback)
    {
        IDisposable? registration = null;

        void Handler(TSender sender, TArgs args)
        {
            callback(
                sender,
                args,
                registration ?? throw new NullReferenceException("registration is null")
            );
        }

        registration = new ActionOnDispose(() => ActionEvent -= Handler);

        ActionEvent += Handler;

        return registration;
    }

    /// <summary>
    /// Raise the event for this token.
    /// </summary>
    /// <param name="sender">The object that is raising the event.</param>
    /// <param name="args">The args included with the event.</param>
    internal void RaiseEvent(TSender sender, TArgs args) => ActionEvent?.Invoke(sender, args);

    #endregion

    #region Events

    /// <summary>
    /// The underlying event.
    /// </summary>
    public event Action<TSender, TArgs>? ActionEvent;

    #endregion
}
