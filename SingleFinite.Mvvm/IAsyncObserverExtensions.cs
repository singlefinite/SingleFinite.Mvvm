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
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm;

/// <summary>
/// Methods that modify how an observable event is handled.  Observers are
/// chained together to create different handler logic.
/// </summary>
public static class IAsyncObserverExtensions
{
    /// <summary>
    /// Invoke the given callback whenever the observable event is raised.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="callback">
    /// The callback to invoke when the observable event is raised.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver OnEach(
        this IAsyncObserver observer,
        Func<Task> callback
    ) =>
        new AsyncObserverForEach(observer, callback);

    /// <summary>
    /// Invoke the given callback whenever the observable event is raised.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="callback">
    /// The callback to invoke when the observable event is raised.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver OnEach(
        this IAsyncObserver observer,
        Action callback
    ) =>
        new AsyncObserverForEach(
            observer,
            () =>
            {
                callback();
                return Task.CompletedTask;
            }
        );

    /// <summary>
    /// Invoke the given callback whenever the observable event is raised.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="callback">
    /// The callback to invoke when the observable event is raised.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> OnEach<TArgs>(
        this IAsyncObserver<TArgs> observer,
        Func<TArgs, Task> callback
    ) => new AsyncObserverForEach<TArgs>(observer, callback);

    /// <summary>
    /// Invoke the given callback whenever the observable event is raised.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="callback">
    /// The callback to invoke when the observable event is raised.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> OnEach<TArgs>(
        this IAsyncObserver<TArgs> observer,
        Action<TArgs> callback
    ) => new AsyncObserverForEach<TArgs>(
        observer,
        args =>
        {
            callback(args);
            return Task.CompletedTask;
        }
    );

    /// <summary>
    /// Invoke the given callback whenever the observable event is raised.
    /// </summary>
    /// <typeparam name="TSender">
    /// The type of sender passed into the observer.
    /// </typeparam>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="callback">
    /// The callback to invoke when the observable event is raised.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TSender, TArgs> OnEach<TSender, TArgs>(
        this IAsyncObserver<TSender, TArgs> observer,
        Func<TSender, TArgs, Task> callback
    ) => new AsyncObserverForEach<TSender, TArgs>(observer, callback);

    /// <summary>
    /// Invoke the given callback whenever the observable event is raised.
    /// </summary>
    /// <typeparam name="TSender">
    /// The type of sender passed into the observer.
    /// </typeparam>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="callback">
    /// The callback to invoke when the observable event is raised.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TSender, TArgs> OnEach<TSender, TArgs>(
        this IAsyncObserver<TSender, TArgs> observer,
        Action<TSender, TArgs> callback
    ) => new AsyncObserverForEach<TSender, TArgs>(
        observer,
        (sender, args) =>
        {
            callback(sender, args);
            return Task.CompletedTask;
        }
    );

    /// <summary>
    /// Select a value to pass to chained observers.
    /// </summary>
    /// <typeparam name="TArgsOut">
    /// The type of arguments that will be passed to chained observers.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="selector">
    /// The callback to invoke to select the arguments to pass to chained
    /// observers.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgsOut> Select<TArgsOut>(
        this IAsyncObserver observer,
        Func<Task<TArgsOut>> selector
    ) => new AsyncObserverSelect<TArgsOut>(observer, selector);

    /// <summary>
    /// Select a value to pass to chained observers.
    /// </summary>
    /// <typeparam name="TArgsOut">
    /// The type of arguments that will be passed to chained observers.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="selector">
    /// The callback to invoke to select the arguments to pass to chained
    /// observers.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgsOut> Select<TArgsOut>(
        this IAsyncObserver observer,
        Func<TArgsOut> selector
    ) => new AsyncObserverSelect<TArgsOut>(
        observer,
        () => Task.FromResult(selector())
    );

    /// <summary>
    /// Select a value to pass to chained observers.
    /// </summary>
    /// <typeparam name="TArgsIn">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <typeparam name="TArgsOut">
    /// The type of arguments that will be passed to chained observers.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="selector">
    /// The callback to invoke to select the arguments to pass to chained
    /// observers.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgsOut> Select<TArgsIn, TArgsOut>(
        this IAsyncObserver<TArgsIn> observer,
        Func<TArgsIn, Task<TArgsOut>> selector
    ) => new AsyncObserverSelect<TArgsIn, TArgsOut>(observer, selector);

    /// <summary>
    /// Select a value to pass to chained observers.
    /// </summary>
    /// <typeparam name="TArgsIn">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <typeparam name="TArgsOut">
    /// The type of arguments that will be passed to chained observers.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="selector">
    /// The callback to invoke to select the arguments to pass to chained
    /// observers.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgsOut> Select<TArgsIn, TArgsOut>(
        this IAsyncObserver<TArgsIn> observer,
        Func<TArgsIn, TArgsOut> selector
    ) => new AsyncObserverSelect<TArgsIn, TArgsOut>(
        observer,
        (args) => Task.FromResult(selector(args))
    );

    /// <summary>
    /// Select a value to pass to chained observers.
    /// </summary>
    /// <typeparam name="TSender">
    /// The type of sender passed into the observer.
    /// </typeparam>
    /// <typeparam name="TArgsIn">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <typeparam name="TArgsOut">
    /// The type of arguments that will be passed to chained observers.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="selector">
    /// The callback to invoke to select the arguments to pass to chained
    /// observers.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TSender, TArgsOut> Select<TSender, TArgsIn, TArgsOut>(
        this IAsyncObserver<TSender, TArgsIn> observer,
        Func<TSender, TArgsIn, Task<TArgsOut>> selector
    ) => new AsyncObserverSelect<TSender, TArgsIn, TArgsOut>(observer, selector);

    /// <summary>
    /// Select a value to pass to chained observers.
    /// </summary>
    /// <typeparam name="TSender">
    /// The type of sender passed into the observer.
    /// </typeparam>
    /// <typeparam name="TArgsIn">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <typeparam name="TArgsOut">
    /// The type of arguments that will be passed to chained observers.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="selector">
    /// The callback to invoke to select the arguments to pass to chained
    /// observers.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TSender, TArgsOut> Select<TSender, TArgsIn, TArgsOut>(
        this IAsyncObserver<TSender, TArgsIn> observer,
        Func<TSender, TArgsIn, TArgsOut> selector
    ) => new AsyncObserverSelect<TSender, TArgsIn, TArgsOut>(
        observer,
        (sender, args) => Task.FromResult(selector(sender, args))
    );

    /// <summary>
    /// Filter out observable events that don't match the predicate and prevent
    /// them from being passed down the observer chain.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="predicate">
    /// The callback invoked to determine if the observable event should be
    /// filtered out.  If the callback returns false the observable event will
    /// be filtered out.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver Where(
        this IAsyncObserver observer,
        Func<Task<bool>> predicate
    ) => new AsyncObserverWhere(observer, predicate);

    /// <summary>
    /// Filter out observable events that don't match the predicate and prevent
    /// them from being passed down the observer chain.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="predicate">
    /// The callback invoked to determine if the observable event should be
    /// filtered out.  If the callback returns false the observable event will
    /// be filtered out.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver Where(
        this IAsyncObserver observer,
        Func<bool> predicate
    ) => new AsyncObserverWhere(
        observer,
        () => Task.FromResult(predicate())
    );

    /// <summary>
    /// Filter out observable events that don't match the predicate and prevent
    /// them from being passed down the observer chain.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="predicate">
    /// The callback invoked to determine if the observable event should be
    /// filtered out.  If the callback returns false the observable event will
    /// be filtered out.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> Where<TArgs>(
        this IAsyncObserver<TArgs> observer,
        Func<TArgs, Task<bool>> predicate
    ) => new AsyncObserverWhere<TArgs>(observer, predicate);

    /// <summary>
    /// Filter out observable events that don't match the predicate and prevent
    /// them from being passed down the observer chain.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="predicate">
    /// The callback invoked to determine if the observable event should be
    /// filtered out.  If the callback returns false the observable event will
    /// be filtered out.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> Where<TArgs>(
        this IAsyncObserver<TArgs> observer,
        Func<TArgs, bool> predicate
    ) => new AsyncObserverWhere<TArgs>(
        observer,
        args => Task.FromResult(predicate(args))
    );

    /// <summary>
    /// Filter out observable events that don't match the predicate and prevent
    /// them from being passed down the observer chain.
    /// </summary>
    /// <typeparam name="TSender">
    /// The type of sender passed into the observer.
    /// </typeparam>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="predicate">
    /// The callback invoked to determine if the observable event should be
    /// filtered out.  If the callback returns false the observable event will
    /// be filtered out.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TSender, TArgs> Where<TSender, TArgs>(
        this IAsyncObserver<TSender, TArgs> observer,
        Func<TSender, TArgs, Task<bool>> predicate
    ) => new AsyncObserverWhere<TSender, TArgs>(observer, predicate);

    /// <summary>
    /// Filter out observable events that don't match the predicate and prevent
    /// them from being passed down the observer chain.
    /// </summary>
    /// <typeparam name="TSender">
    /// The type of sender passed into the observer.
    /// </typeparam>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="predicate">
    /// The callback invoked to determine if the observable event should be
    /// filtered out.  If the callback returns false the observable event will
    /// be filtered out.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TSender, TArgs> Where<TSender, TArgs>(
        this IAsyncObserver<TSender, TArgs> observer,
        Func<TSender, TArgs, bool> predicate
    ) => new AsyncObserverWhere<TSender, TArgs>(
        observer,
        (sender, args) => Task.FromResult(predicate(sender, args))
    );

    /// <summary>
    /// Dispose of the observer chain if the predicate is matched.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="predicate">
    /// The callback invoked to determine if the observer chain should be
    /// disposed.  If the callback returns true the observer chain is disposed.
    /// </param>
    /// <param name="continueOnDispose">
    /// When set to true the next observer in the observer chain will be invoked
    /// even when predicate returns true and this observer chain will be
    /// disposed.  Default is false.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver Until(
        this IAsyncObserver observer,
        Func<Task<bool>> predicate,
        bool continueOnDispose = false
    ) => new AsyncObserverUntil(observer, predicate, continueOnDispose);

    /// <summary>
    /// Dispose of the observer chain if the predicate is matched.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="predicate">
    /// The callback invoked to determine if the observer chain should be
    /// disposed.  If the callback returns true the observer chain is disposed.
    /// </param>
    /// <param name="continueOnDispose">
    /// When set to true the next observer in the observer chain will be invoked
    /// even when predicate returns true and this observer chain will be
    /// disposed.  Default is false.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver Until(
        this IAsyncObserver observer,
        Func<bool> predicate,
        bool continueOnDispose = false
    ) => new AsyncObserverUntil(
        observer,
        () => Task.FromResult(predicate()),
        continueOnDispose
    );

    /// <summary>
    /// Dispose of the observer chain if the predicate is matched.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="predicate">
    /// The callback invoked to determine if the observer chain should be
    /// disposed.  If the callback returns true the observer chain is disposed.
    /// </param>
    /// <param name="continueOnDispose">
    /// When set to true the next observer in the observer chain will be invoked
    /// even when predicate returns true and this observer chain will be
    /// disposed.  Default is false.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> Until<TArgs>(
        this IAsyncObserver<TArgs> observer,
        Func<TArgs, Task<bool>> predicate,
        bool continueOnDispose = false
    ) => new AsyncObserverUntil<TArgs>(
        observer,
        predicate,
        continueOnDispose
    );

    /// <summary>
    /// Dispose of the observer chain if the predicate is matched.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="predicate">
    /// The callback invoked to determine if the observer chain should be
    /// disposed.  If the callback returns true the observer chain is disposed.
    /// </param>
    /// <param name="continueOnDispose">
    /// When set to true the next observer in the observer chain will be invoked
    /// even when predicate returns true and this observer chain will be
    /// disposed.  Default is false.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> Until<TArgs>(
        this IAsyncObserver<TArgs> observer,
        Func<TArgs, bool> predicate,
        bool continueOnDispose = false
    ) => new AsyncObserverUntil<TArgs>(
        observer,
        args => Task.FromResult(predicate(args)),
        continueOnDispose
    );

    /// <summary>
    /// Dispose of the observer chain if the predicate is matched.
    /// </summary>
    /// <typeparam name="TSender">
    /// The type of sender passed into the observer.
    /// </typeparam>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="predicate">
    /// The callback invoked to determine if the observer chain should be
    /// disposed.  If the callback returns true the observer chain is disposed.
    /// </param>
    /// <param name="continueOnDispose">
    /// When set to true the next observer in the observer chain will be invoked
    /// even when predicate returns true and this observer chain will be
    /// disposed.  Default is false.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TSender, TArgs> Until<TSender, TArgs>(
        this IAsyncObserver<TSender, TArgs> observer,
        Func<TSender, TArgs, Task<bool>> predicate,
        bool continueOnDispose = false
    ) => new AsyncObserverUntil<TSender, TArgs>(
        observer,
        predicate,
        continueOnDispose
    );

    /// <summary>
    /// Dispose of the observer chain if the predicate is matched.
    /// </summary>
    /// <typeparam name="TSender">
    /// The type of sender passed into the observer.
    /// </typeparam>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="predicate">
    /// The callback invoked to determine if the observer chain should be
    /// disposed.  If the callback returns true the observer chain is disposed.
    /// </param>
    /// <param name="continueOnDispose">
    /// When set to true the next observer in the observer chain will be invoked
    /// even when predicate returns true and this observer chain will be
    /// disposed.  Default is false.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TSender, TArgs> Until<TSender, TArgs>(
        this IAsyncObserver<TSender, TArgs> observer,
        Func<TSender, TArgs, bool> predicate,
        bool continueOnDispose = false
    ) => new AsyncObserverUntil<TSender, TArgs>(
        observer,
        (sender, args) => Task.FromResult(predicate(sender, args)),
        continueOnDispose
    );

    /// <summary>
    /// Dispose of the observer chain when the first event is observed.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver Once(
        this IAsyncObserver observer
    ) => Until(
        observer,
        predicate: () => Task.FromResult(true),
        continueOnDispose: true
    );

    /// <summary>
    /// Dispose of the observer chain when the first event is observed.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with the observed event.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> Once<TArgs>(
        this IAsyncObserver<TArgs> observer
    ) => Until(
        observer,
        predicate: _ => Task.FromResult(true),
        continueOnDispose: true
    );

    /// <summary>
    /// Dispose of the observer chain when the first event is observed.
    /// </summary>
    /// <typeparam name="TSender">
    /// The type of sender passed with the observed event.
    /// </typeparam>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with the observed event.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TSender, TArgs> Once<TSender, TArgs>(
        this IAsyncObserver<TSender, TArgs> observer
    ) => Until(
        observer,
        predicate: (_, _) => Task.FromResult(true),
        continueOnDispose: true
    );

    /// <summary>
    /// Dispose of the observer chain when the given lifecycle object is
    /// disposed.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="lifecycle">
    /// The lifecycle that that when disposed will dispose of this observer.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver On(
        this IAsyncObserver observer,
        ILifecycle lifecycle
    ) => new AsyncObserverOn(
        observer,
        lifecycle
    );

    /// <summary>
    /// Dispose of the observer chain when the given lifecycle object is
    /// disposed.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with the observed event.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="lifecycle">
    /// The lifecycle that that when disposed will dispose of this observer.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> On<TArgs>(
        this IAsyncObserver<TArgs> observer,
        ILifecycle lifecycle
    ) => new AsyncObserverOn<TArgs>(
        observer,
        lifecycle
    );

    /// <summary>
    /// Dispose of the observer chain when the given lifecycle object is
    /// disposed.
    /// </summary>
    /// <typeparam name="TSender">
    /// The type of sender passed with the observed event.
    /// </typeparam>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with the observed event.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="lifecycle">
    /// The lifecycle that that when disposed will dispose of this observer.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TSender, TArgs> On<TSender, TArgs>(
        this IAsyncObserver<TSender, TArgs> observer,
        ILifecycle lifecycle
    ) => new AsyncObserverOn<TSender, TArgs>(
        observer,
        lifecycle
    );

    /// <summary>
    /// Dispose of the observer chain when the given cancellation token is
    /// cancelled.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="cancellationToken">
    /// The cancellation token that when cancelled will dispose of this
    /// observer.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver On(
        this IAsyncObserver observer,
        CancellationToken cancellationToken
    ) => new AsyncObserverOn(
        observer,
        cancellationToken
    );

    /// <summary>
    /// Dispose of the observer chain when the given cancellation token is
    /// cancelled.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with the observed event.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="cancellationToken">
    /// The cancellation token that that cancelled will dispose of this
    /// observer.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> On<TArgs>(
        this IAsyncObserver<TArgs> observer,
        CancellationToken cancellationToken
    ) => new AsyncObserverOn<TArgs>(
        observer,
        cancellationToken
    );

    /// <summary>
    /// Dispose of the observer chain when the given cancellation token is
    /// cancelled.
    /// </summary>
    /// <typeparam name="TSender">
    /// The type of sender passed with the observed event.
    /// </typeparam>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with the observed event.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="cancellationToken">
    /// The cancellation token that that cancelled will dispose of this
    /// observer.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TSender, TArgs> On<TSender, TArgs>(
        this IAsyncObserver<TSender, TArgs> observer,
        CancellationToken cancellationToken
    ) => new AsyncObserverOn<TSender, TArgs>(
        observer,
        cancellationToken
    );

    /// <summary>
    /// Observer that debounces events.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="delay">The delay period for debouncing.</param>
    /// <param name="dispatcher">
    /// The dispatcher to run on after the delay has passed.
    /// </param>
    /// <param name="debouncer">
    /// The debouncer to use for debouncing.  If not specifed a default debouncer
    /// will be used.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver Debounce(
        this IAsyncObserver observer,
        TimeSpan delay,
        IDispatcher dispatcher,
        IDebouncer? debouncer = null
    ) => new AsyncObserverDebounce(
        observer,
        delay,
        dispatcher,
        debouncer
    );

    /// <summary>
    /// Observer that debounces events.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with observed events.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="delay">The delay period for debouncing.</param>
    /// <param name="dispatcher">
    /// The dispatcher to run on after the delay has passed.
    /// </param>
    /// <param name="debouncer">
    /// The debouncer to use for debouncing.  If not specifed a default debouncer
    /// will be used.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> Debounce<TArgs>(
        this IAsyncObserver<TArgs> observer,
        TimeSpan delay,
        IDispatcher dispatcher,
        IDebouncer? debouncer = null
    ) => new AsyncObserverDebounce<TArgs>(
        observer,
        delay,
        dispatcher,
        debouncer
    );

    /// <summary>
    /// Observer that debounces events.
    /// </summary>
    /// <typeparam name="TSender">
    /// The type of sender passed with observed events.
    /// </typeparam>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with observed events.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="delay">The delay period for debouncing.</param>
    /// <param name="dispatcher">
    /// The dispatcher to run on after the delay has passed.
    /// </param>
    /// <param name="debouncer">
    /// The debouncer to use for debouncing.  If not specifed a default debouncer
    /// will be used.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TSender, TArgs> Debounce<TSender, TArgs>(
        this IAsyncObserver<TSender, TArgs> observer,
        TimeSpan delay,
        IDispatcher dispatcher,
        IDebouncer? debouncer = null
    ) => new AsyncObserverDebounce<TSender, TArgs>(
        observer,
        delay,
        dispatcher,
        debouncer
    );

    /// <summary>
    /// Invoke the given callback whenever an exception thrown below this
    /// observer in the chain is thrown.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="callback">
    /// The callback to invoke whenever an exception is caught.  If the
    /// exception is not handled it will be rethrown and continue up the chain.
    /// By default the exception is considered handled unless the IsHandled
    /// property of the ExceptionEventArgs is changed to false.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver Catch(
        this IAsyncObserver observer,
        Func<ExceptionEventArgs, Task> callback
    ) => new AsyncObserverCatch(observer, callback);

    /// <summary>
    /// Invoke the given callback whenever an exception thrown below this
    /// observer in the chain is thrown.  Caught exceptions will not move past
    /// this observer.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="callback">
    /// The callback to invoke whenever an exception is caught.  If the
    /// exception is not handled it will be rethrown and continue up the chain.
    /// By default the exception is considered handled unless the IsHandled
    /// property of the ExceptionEventArgs is changed to false.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver Catch(
        this IAsyncObserver observer,
        Action<ExceptionEventArgs> callback
    ) => new AsyncObserverCatch(
        observer,
        ex =>
        {
            callback(ex);
            return Task.CompletedTask;
        }
    );

    /// <summary>
    /// Invoke the given callback whenever an exception thrown below this
    /// observer in the chain is thrown.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="callback">
    /// The callback to invoke whenever an exception is caught.  If the
    /// exception is not handled it will be rethrown and continue up the chain.
    /// By default the exception is considered handled unless the IsHandled
    /// property of the ExceptionEventArgs is changed to false.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> Catch<TArgs>(
        this IAsyncObserver<TArgs> observer,
        Func<TArgs, ExceptionEventArgs, Task> callback
    ) => new AsyncObserverCatch<TArgs>(observer, callback);

    /// <summary>
    /// Invoke the given callback whenever an exception thrown below this
    /// observer in the chain is thrown.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="callback">
    /// The callback to invoke whenever an exception is caught.  If the
    /// exception is not handled it will be rethrown and continue up the chain.
    /// By default the exception is considered handled unless the IsHandled
    /// property of the ExceptionEventArgs is changed to false.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> Catch<TArgs>(
        this IAsyncObserver<TArgs> observer,
        Action<TArgs, ExceptionEventArgs> callback
    ) => new AsyncObserverCatch<TArgs>(
        observer,
        (args, ex) =>
        {
            callback(args, ex);
            return Task.CompletedTask;
        }
    );

    /// <summary>
    /// Invoke the given callback whenever an exception thrown below this
    /// observer in the chain is thrown.
    /// </summary>
    /// <typeparam name="TSender">
    /// The type of sender passed into the observer.
    /// </typeparam>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="callback">
    /// The callback to invoke whenever an exception is caught.  If the
    /// exception is not handled it will be rethrown and continue up the chain.
    /// By default the exception is considered handled unless the IsHandled
    /// property of the ExceptionEventArgs is changed to false.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TSender, TArgs> Catch<TSender, TArgs>(
        this IAsyncObserver<TSender, TArgs> observer,
        Func<TSender, TArgs, ExceptionEventArgs, Task> callback
    ) => new AsyncObserverCatch<TSender, TArgs>(observer, callback);

    /// <summary>
    /// Invoke the given callback whenever an exception thrown below this
    /// observer in the chain is thrown.
    /// </summary>
    /// <typeparam name="TSender">
    /// The type of sender passed into the observer.
    /// </typeparam>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="callback">
    /// The callback to invoke whenever an exception is caught.  If the
    /// exception is not handled it will be rethrown and continue up the chain.
    /// By default the exception is considered handled unless the IsHandled
    /// property of the ExceptionEventArgs is changed to false.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TSender, TArgs> Catch<TSender, TArgs>(
        this IAsyncObserver<TSender, TArgs> observer,
        Action<TSender, TArgs, ExceptionEventArgs> callback
    ) => new AsyncObserverCatch<TSender, TArgs>(
        observer,
        (sender, args, ex) =>
        {
            callback(sender, args, ex);
            return Task.CompletedTask;
        }
    );

    /// <summary>
    /// Limit the number of observers that can be executing at the same time.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="maxConcurrent">
    /// The max number of observers allowed to be executing at the same time.
    /// If a new event occurs and there are already max number of observers
    /// executing the event will be buffered until enough executing observers
    /// complete to allow the new event to be passed on.  Must be zero or
    /// greater or an exception will be thrown.
    /// </param>
    /// <param name="maxBuffer">
    /// The max number of events that will be buffered if there are already max
    /// number of concurrent observers executing.  If a new event occurs and there
    /// are already max number of events buffered the event will be ignored.  If
    /// this is set to -1 there is no limit.  Default is -1.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if maxConcurrent is less than zero.
    /// </exception>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver Limit(
        this IAsyncObserver observer,
        int maxConcurrent,
        int maxBuffer = -1
    ) => new AsyncObserverLimit(
        observer,
        maxConcurrent,
        maxBuffer
    );

    /// <summary>
    /// Limit the number of observers that can be executing at the same time.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="maxConcurrent">
    /// The max number of observers allowed to be executing at the same time.
    /// If a new event occurs and there are already max number of observers
    /// executing the event will be buffered until enough executing observers
    /// complete to allow the new event to be passed on.  Must be zero or
    /// greater or an exception will be thrown.
    /// </param>
    /// <param name="maxBuffer">
    /// The max number of events that will be buffered if there are already max
    /// number of concurrent observers executing.  If a new event occurs and there
    /// are already max number of events buffered the event will be ignored.  If
    /// this is set to -1 there is no limit.  Default is -1.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if maxConcurrent is less than zero.
    /// </exception>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> Limit<TArgs>(
        this IAsyncObserver<TArgs> observer,
        int maxConcurrent,
        int maxBuffer = -1
    ) => new AsyncObserverLimit<TArgs>(
        observer,
        maxConcurrent,
        maxBuffer
    );

    /// <summary>
    /// Limit the number of observers that can be executing at the same time.
    /// </summary>
    /// <typeparam name="TSender">
    /// The type of sender passed into the observer.
    /// </typeparam>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="maxConcurrent">
    /// The max number of observers allowed to be executing at the same time.
    /// If a new event occurs and there are already max number of observers
    /// executing the event will be buffered until enough executing observers
    /// complete to allow the new event to be passed on.  Must be zero or
    /// greater or an exception will be thrown.
    /// </param>
    /// <param name="maxBuffer">
    /// The max number of events that will be buffered if there are already max
    /// number of concurrent observers executing.  If a new event occurs and there
    /// are already max number of events buffered the event will be ignored.  If
    /// this is set to -1 there is no limit.  Default is -1.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if maxConcurrent is less than zero.
    /// </exception>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TSender, TArgs> Limit<TSender, TArgs>(
        this IAsyncObserver<TSender, TArgs> observer,
        int maxConcurrent,
        int maxBuffer = -1
    ) => new AsyncObserverLimit<TSender, TArgs>(
        observer,
        maxConcurrent,
        maxBuffer
    );
}
