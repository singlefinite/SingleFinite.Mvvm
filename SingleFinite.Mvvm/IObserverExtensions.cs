﻿// MIT License
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
    /// Invoke the given callback whenever an exception thrown below this
    /// observer in the chain is thrown.  Caught exceptions will not move past
    /// this observer.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="callback">
    /// The callback to invoke when an exception is caught.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver Catch(
        this IObserver observer,
        Action<Exception> callback
    ) => new ObserverCatch(
        observer,
        ex =>
        {
            callback(ex);
            return true;
        }
    );

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
    /// The callback to invoke when an exception is caught.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver<TArgs> Catch<TArgs>(
        this IObserver<TArgs> observer,
        Action<TArgs, Exception> callback
    ) => new ObserverCatch<TArgs>(
        observer,
        (args, ex) =>
        {
            callback(args, ex);
            return true;
        }
    );

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
    /// The callback to invoke when an exception is caught.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver<TSender, TArgs> Catch<TSender, TArgs>(
        this IObserver<TSender, TArgs> observer,
        Action<TSender, TArgs, Exception> callback
    ) => new ObserverCatch<TSender, TArgs>(
        observer,
        (sender, args, ex) =>
        {
            callback(sender, args, ex);
            return true;
        }
    );

    /// <summary>
    /// Invoke the given callback whenever an exception thrown below this
    /// observer in the chain is thrown.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="callback">
    /// The callback to invoke when an exception is caught.  If the exception
    /// is handled by this callback it should return true which will prevent
    /// the exception from moving further up the observer chain.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver Catch(
        this IObserver observer,
        Func<Exception, bool> callback
    ) => new ObserverCatch(observer, callback);

    /// <summary>
    /// Invoke the given callback whenever an exception thrown below this
    /// observer in the chain is thrown.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="callback">
    /// The callback to invoke when an exception is caught.  If the exception
    /// is handled by this callback it should return true which will prevent
    /// the exception from moving further up the observer chain.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver<TArgs> Catch<TArgs>(
        this IObserver<TArgs> observer,
        Func<TArgs, Exception, bool> callback
    ) => new ObserverCatch<TArgs>(observer, callback);

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
    /// The callback to invoke when an exception is caught.  If the exception
    /// is handled by this callback it should return true which will prevent
    /// the exception from moving further up the observer chain.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver<TSender, TArgs> Catch<TSender, TArgs>(
        this IObserver<TSender, TArgs> observer,
        Func<TSender, TArgs, Exception, bool> callback
    ) => new ObserverCatch<TSender, TArgs>(observer, callback);
}
