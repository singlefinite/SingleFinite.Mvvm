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
/// <see cref="ObservableSource"/> class.
/// </summary>
public sealed class Observable
{
    #region Constructors

    /// <summary>
    /// Instances of this class are created with <see cref="ObservableSource"/> 
    /// objects.
    /// </summary>
    /// <param name="source">
    /// The source that raises the observable event.
    /// </param>
    internal Observable(ObservableSource source)
    {
        source.Event += RaiseEvent;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create an observer for this observable.
    /// </summary>
    /// <returns>
    /// An observer that runs when the event for this observable is raised.
    /// </returns>
    public IObserver Observe() => new ObserverSource(this);

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
    public IObserver Observe(Action callback) =>
        new ObserverSource(this).OnEach(callback);

    /// <summary>
    /// Raise the event for this observable.
    /// </summary>
    private void RaiseEvent() => Event?.Invoke();

    #endregion

    #region Events

    /// <summary>
    /// The underlying event.
    /// </summary>
    public event Action? Event;

    #endregion
}

/// <summary>
/// This class wraps an event to make registering and unregistering callbacks 
/// for the event more convenient when working with dependency injection scopes.
/// Observable instances are created by instances of the 
/// <see cref="ObservableSource"/> class.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with the event.
/// </typeparam>
public sealed class Observable<TArgs>
{
    #region Constructors

    /// <summary>
    /// Instances of this class are created with <see cref="ObservableSource"/> 
    /// objects.
    /// </summary>
    /// <param name="source">
    /// The source that raises the observable event.
    /// </param>
    internal Observable(ObservableSource<TArgs> source)
    {
        source.Event += RaiseEvent;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create an observer for this observable.
    /// </summary>
    /// <returns>
    /// An observer that runs when the event for this observable is raised.
    /// </returns>
    public IObserver<TArgs> Observe() => new ObserverSource<TArgs>(this);

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
    public IObserver<TArgs> Observe(Action<TArgs> callback) =>
        new ObserverSource<TArgs>(this).OnEach(callback);

    /// <summary>
    /// Raise the event for this observable.
    /// </summary>
    /// <param name="args">The args included with the event.</param>
    private void RaiseEvent(TArgs args) => Event?.Invoke(args);

    #endregion

    #region Events

    /// <summary>
    /// The underlying event.
    /// </summary>
    public event Action<TArgs>? Event;

    #endregion
}

/// <summary>
/// This class wraps an event to make registering and unregistering callbacks 
/// for the event more convenient when working with dependency injection scopes.
/// Observable instances are created by instances of the 
/// <see cref="ObservableSource"/> class.
/// </summary>
/// <typeparam name="TSender">
/// The type of sender that raises the event.
/// </typeparam>
/// <typeparam name="TArgs">
/// The type of arguments passed with the event.
/// </typeparam>
public sealed class Observable<TSender, TArgs>
{
    #region Constructors

    /// <summary>
    /// Instances of this class are created with <see cref="ObservableSource"/> 
    /// objects.
    /// </summary>
    /// <param name="source">
    /// The source that raises the observable event.
    /// </param>
    internal Observable(ObservableSource<TSender, TArgs> source)
    {
        source.Event += RaiseEvent;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create an observer for this observable.
    /// </summary>
    /// <returns>
    /// An observer that runs when the event for this observable is raised.
    /// </returns>
    public IObserver<TSender, TArgs> Observe() =>
        new ObserverSource<TSender, TArgs>(this);

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
    public IObserver<TSender, TArgs> Observe(Action<TSender, TArgs> callback) =>
        new ObserverSource<TSender, TArgs>(this).OnEach(callback);

    /// <summary>
    /// Raise the event for this observable.
    /// </summary>
    /// <param name="sender">The object that is raising the event.</param>
    /// <param name="args">The args included with the event.</param>
    private void RaiseEvent(TSender sender, TArgs args) =>
        Event?.Invoke(sender, args);

    #endregion

    #region Events

    /// <summary>
    /// The underlying event.
    /// </summary>
    public event Action<TSender, TArgs>? Event;

    #endregion
}

