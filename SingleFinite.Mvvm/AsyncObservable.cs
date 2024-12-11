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

using SingleFinite.Mvvm.Internal.Observers;

namespace SingleFinite.Mvvm;

/// <summary>
/// This class wraps an event to make registering and unregistering callbacks 
/// for the event more convenient when working with dependency injection scopes.
/// Observable instances are created by instances of the 
/// <see cref="AsyncObservableSource"/> class.
/// </summary>
public sealed class AsyncObservable
{
    #region Constructors

    /// <summary>
    /// Instances of this class are created with
    /// <see cref="AsyncObservableSource"/> objects.
    /// </summary>
    /// <param name="source">
    /// The source that raises the observable event.
    /// </param>
    internal AsyncObservable(AsyncObservableSource source)
    {
        source.Event += RaiseEventAsync;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create an observer for this observable.
    /// </summary>
    /// <returns>
    /// An observer that runs when the event for this observable is raised.
    /// </returns>
    public IAsyncObserver Observe() => new AsyncObserverSource(this);

    /// <summary>
    /// Create an observer for this observable.
    /// This is a convience method that is equivalent to calling
    /// Observe().OnEach.
    /// </summary>
    /// <param name="callback">
    /// The callback to invoke whenever the event for this observable is raised.
    /// </param>
    /// <returns>
    /// An observer that runs when the event for this observable is raised.
    /// </returns>
    public IAsyncObserver Observe(Func<Task> callback) =>
        new AsyncObserverSource(this).OnEach(callback);

    /// <summary>
    /// Raise the event for this observable.
    /// </summary>
    /// <returns>The running task.</returns>
    private Task RaiseEventAsync() => Event?.Invoke() ?? Task.CompletedTask;

    #endregion

    #region Events

    /// <summary>
    /// The underlying event.
    /// </summary>
    public event Func<Task>? Event;

    #endregion
}

/// <summary>
/// This class wraps an event to make registering and unregistering callbacks 
/// for the event more convenient when working with dependency injection scopes.
/// Observable instances are created by instances of the 
/// <see cref="AsyncObservableSource{TArgs}"/> class.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with the event.
/// </typeparam>
public sealed class AsyncObservable<TArgs>
{
    #region Constructors

    /// <summary>
    /// Instances of this class are created with
    /// <see cref="AsyncObservableSource"/> objects.
    /// </summary>
    /// <param name="source">
    /// The source that raises the observable event.
    /// </param>
    internal AsyncObservable(AsyncObservableSource<TArgs> source)
    {
        source.Event += RaiseEventAsync;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create an observer for this observable.
    /// </summary>
    /// <returns>
    /// An observer that runs when the event for this observable is raised.
    /// </returns>
    public IAsyncObserver<TArgs> Observe() =>
        new AsyncObserverSource<TArgs>(this);

    /// <summary>
    /// Create an observer for this observable.
    /// This is a convience method that is equivalent to calling
    /// Observe().OnEach.
    /// </summary>
    /// <param name="callback">
    /// The callback to invoke whenever the event for this observable is raised.
    /// </param>
    /// <returns>
    /// An observer that runs when the event for this observable is raised.
    /// </returns>
    public IAsyncObserver<TArgs> Observe(Func<TArgs, Task> callback) =>
        new AsyncObserverSource<TArgs>(this).OnEach(callback);

    /// <summary>
    /// Raise the event for this observable.
    /// </summary>
    /// <param name="args">The args included with the event.</param>
    /// <returns>The running task.</returns>
    private Task RaiseEventAsync(TArgs args) =>
        Event?.Invoke(args) ?? Task.CompletedTask;

    #endregion

    #region Events

    /// <summary>
    /// The underlying event.
    /// </summary>
    public event Func<TArgs, Task>? Event;

    #endregion
}

/// <summary>
/// This class wraps an event to make registering and unregistering callbacks 
/// for the event more convenient when working with dependency injection scopes.
/// Observable instances are created by instances of the 
/// <see cref="AsyncObservableSource{TSender, TArgs}"/> class.
/// </summary>
/// <typeparam name="TSender">
/// The type of sender that raises the event.
/// </typeparam>
/// <typeparam name="TArgs">
/// The type of arguments passed with the event.
/// </typeparam>
public sealed class AsyncObservable<TSender, TArgs>
{
    #region Constructors

    /// <summary>
    /// Instances of this class are created with
    /// <see cref="AsyncObservableSource"/> objects.
    /// </summary>
    /// <param name="source">
    /// The source that raises the observable event.
    /// </param>
    internal AsyncObservable(AsyncObservableSource<TSender, TArgs> source)
    {
        source.Event += RaiseEventAsync;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create an observer for this observable.
    /// </summary>
    /// <returns>
    /// An observer that runs when the event for this observable is raised.
    /// </returns>
    public IAsyncObserver<TSender, TArgs> Observe() =>
        new AsyncObserverSource<TSender, TArgs>(this);

    /// <summary>
    /// Create an observer for this observable.
    /// This is a convience method that is equivalent to calling
    /// Observe().OnEach.
    /// </summary>
    /// <param name="callback">
    /// The callback to invoke whenever the event for this observable is raised.
    /// </param>
    /// <returns>
    /// An observer that runs when the event for this observable is raised.
    /// </returns>
    public IAsyncObserver<TSender, TArgs> Observe(
        Func<TSender, TArgs, Task> callback
    ) =>
        new AsyncObserverSource<TSender, TArgs>(this).OnEach(callback);

    /// <summary>
    /// Raise the event for this observable.
    /// </summary>
    /// <param name="sender">The object that is raising the event.</param>
    /// <param name="args">The args included with the event.</param>
    /// <returns>The running task.</returns>
    private Task RaiseEventAsync(TSender sender, TArgs args) =>
        Event?.Invoke(sender, args) ?? Task.CompletedTask;

    #endregion

    #region Events

    /// <summary>
    /// The underlying event.
    /// </summary>
    public event Func<TSender, TArgs, Task>? Event;

    #endregion
}
