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
public static class IObserverExtensions
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
    public static IObserver OnEach(this IObserver observer, Action callback) =>
        new ObserverForEach(observer, callback);

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
    public static IObserver<TArgs> OnEach<TArgs>(
        this IObserver<TArgs> observer,
        Action<TArgs> callback
    ) => new ObserverForEach<TArgs>(observer, callback);

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
    public static IObserver<TSender, TArgs> OnEach<TSender, TArgs>(
        this IObserver<TSender, TArgs> observer,
        Action<TSender, TArgs> callback
    ) => new ObserverForEach<TSender, TArgs>(observer, callback);

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
    public static IObserver<TArgsOut> Select<TArgsOut>(
        this IObserver observer,
        Func<TArgsOut> selector
    ) => new ObserverSelect<TArgsOut>(observer, selector);

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
    public static IObserver<TArgsOut> Select<TArgsIn, TArgsOut>(
        this IObserver<TArgsIn> observer,
        Func<TArgsIn, TArgsOut> selector
    ) => new ObserverSelect<TArgsIn, TArgsOut>(observer, selector);

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
    public static IObserver<TSender, TArgsOut> Select<TSender, TArgsIn, TArgsOut>(
        this IObserver<TSender, TArgsIn> observer,
        Func<TSender, TArgsIn, TArgsOut> selector
    ) => new ObserverSelect<TSender, TArgsIn, TArgsOut>(observer, selector);

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
    public static IObserver Where(
        this IObserver observer,
        Func<bool> predicate
    ) => new ObserverWhere(observer, predicate);

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
    public static IObserver<TArgs> Where<TArgs>(
        this IObserver<TArgs> observer,
        Func<TArgs, bool> predicate
    ) => new ObserverWhere<TArgs>(observer, predicate);

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
    public static IObserver<TSender, TArgs> Where<TSender, TArgs>(
        this IObserver<TSender, TArgs> observer,
        Func<TSender, TArgs, bool> predicate
    ) => new ObserverWhere<TSender, TArgs>(observer, predicate);

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
    public static IObserver DisposeIf(
        this IObserver observer,
        Func<bool> predicate,
        bool continueOnDispose = false
    ) => new ObserverDisposeIf(observer, predicate, continueOnDispose);

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
    public static IObserver<TArgs> DisposeIf<TArgs>(
        this IObserver<TArgs> observer,
        Func<TArgs, bool> predicate,
        bool continueOnDispose = false
    ) => new ObserverDisposeIf<TArgs>(observer, predicate, continueOnDispose);

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
    public static IObserver<TSender, TArgs> DisposeIf<TSender, TArgs>(
        this IObserver<TSender, TArgs> observer,
        Func<TSender, TArgs, bool> predicate,
        bool continueOnDispose = false
    ) => new ObserverDisposeIf<TSender, TArgs>(observer, predicate, continueOnDispose);

    /// <summary>
    /// Dispose of the observer chain when the first event is observed.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver Once(
        this IObserver observer
    ) => DisposeIf(
        observer,
        predicate: () => true,
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
    public static IObserver<TArgs> Once<TArgs>(
        this IObserver<TArgs> observer
    ) => DisposeIf(
        observer,
        predicate: _ => true,
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
    public static IObserver<TSender, TArgs> Once<TSender, TArgs>(
        this IObserver<TSender, TArgs> observer
    ) => DisposeIf(
        observer,
        predicate: (_, _) => true,
        continueOnDispose: true
    );

    /// <summary>
    /// Observer that debounces events.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="dispatcher">The dispatcher to use for debouncing.</param>
    /// <param name="delay">The delay period for debouncing.</param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver Debounce(
        this IObserver observer,
        IDispatcherWithCancellation dispatcher,
        TimeSpan delay
    ) => new ObserverDebounce(
        observer,
        dispatcher,
        delay
    );

    /// <summary>
    /// Observer that debounces events.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with observed events.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="dispatcher">The dispatcher to use for debouncing.</param>
    /// <param name="delay">The delay period for debouncing.</param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver<TArgs> Debounce<TArgs>(
        this IObserver<TArgs> observer,
        IDispatcherWithCancellation dispatcher,
        TimeSpan delay
    ) => new ObserverDebounce<TArgs>(
        observer,
        dispatcher,
        delay
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
    /// <param name="dispatcher">The dispatcher to use for debouncing.</param>
    /// <param name="delay">The delay period for debouncing.</param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver<TSender, TArgs> Debounce<TSender, TArgs>(
        this IObserver<TSender, TArgs> observer,
        IDispatcherWithCancellation dispatcher,
        TimeSpan delay
    ) => new ObserverDebounce<TSender, TArgs>(
        observer,
        dispatcher,
        delay
    );

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
    public static IObserver Catch(
        this IObserver observer,
        Action<ExceptionEventArgs> callback
    ) => new ObserverCatch(observer, callback);

    /// <summary>
    /// Invoke the given callback whenever an exception thrown below this
    /// observer in the chain is thrown.  Caught exceptions will not move past
    /// this observer.
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
    public static IObserver<TArgs> Catch<TArgs>(
        this IObserver<TArgs> observer,
        Action<TArgs, ExceptionEventArgs> callback
    ) => new ObserverCatch<TArgs>(observer, callback);

    /// <summary>
    /// Invoke the given callback whenever an exception thrown below this
    /// observer in the chain is thrown.  Caught exceptions will not move past
    /// this observer.
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
    public static IObserver<TSender, TArgs> Catch<TSender, TArgs>(
        this IObserver<TSender, TArgs> observer,
        Action<TSender, TArgs, ExceptionEventArgs> callback
    ) => new ObserverCatch<TSender, TArgs>(observer, callback);

    /// <summary>
    /// Create an observer that raises the next event in the observer chain as
    /// async by using the provided dispatcher..
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="dispatcher">
    /// The dispatcher the async events will be run with.
    /// </param>
    /// <param name="onError">
    /// Optional action that is invoked if an exception is thrown from the async
    /// observers.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver ToAsync(
        this IObserver observer,
        IDispatcher dispatcher,
        Action<ExceptionEventArgs>? onError = default
    ) => new ObserverToAsync(observer, dispatcher, onError);

    /// <summary>
    /// Create an observer that raises the next event in the observer chain as
    /// async by using the provided dispatcher..
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with observed events.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="dispatcher">
    /// The dispatcher the async events will be run with.
    /// </param>
    /// <param name="onError">
    /// Optional action that is invoked if an exception is thrown from the async
    /// observers.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> ToAsync<TArgs>(
        this IObserver<TArgs> observer,
        IDispatcher dispatcher,
        Action<ExceptionEventArgs>? onError = default
    ) => new ObserverToAsync<TArgs>(observer, dispatcher, onError);

    /// <summary>
    /// Create an observer that raises the next event in the observer chain as
    /// async by using the provided dispatcher..
    /// </summary>
    /// <typeparam name="TSender">
    /// The type of sender passed with observed events.
    /// </typeparam>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with observed events.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="dispatcher">
    /// The dispatcher the async events will be run with.
    /// </param>
    /// <param name="onError">
    /// Optional action that is invoked if an exception is thrown from the async
    /// observers.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TSender, TArgs> ToAsync<TSender, TArgs>(
        this IObserver<TSender, TArgs> observer,
        IDispatcher dispatcher,
        Action<ExceptionEventArgs>? onError = default
    ) => new ObserverToAsync<TSender, TArgs>(observer, dispatcher, onError);
}
