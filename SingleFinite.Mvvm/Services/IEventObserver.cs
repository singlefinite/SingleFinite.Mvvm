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

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SingleFinite.Mvvm.Services;

/// <summary>
/// A service that manages event callbacks.  This service belongs to a dependency injection scope and when that scope is disposed
/// all callbacks that are managed by this service will be unregistered from their events.
/// </summary>
public interface IEventObserver
{
    /// <summary>
    /// Setup an event callback that will be unregistered when the dependency injection scope that
    /// this service belongs to is disposed.
    /// </summary>
    /// <param name="token">The event to register the callback to.</param>
    /// <param name="callback">The callback that will be registered to the event.</param>
    /// <param name="cancellationToken">Optional token that when cancelled will unregister the callback.</param>
    /// <exception cref="ObjectDisposedException">Thrown if the service has been disposed.</exception>
    /// <returns>A disposable object that when disposed will unregister the callback.</returns>
    IDisposable Observe(
        EventToken token,
        Action callback,
        CancellationToken? cancellationToken = null
    );

    /// <summary>
    /// Setup an event callback that will be unregistered when the dependency injection scope that
    /// this service belongs to is disposed.
    /// </summary>
    /// <param name="token">The event to register the callback to.</param>
    /// <param name="callback">
    /// The callback that will be registered to the event.
    /// A disposable is passed to the callback that when disposed will unregister the callback.
    /// </param>
    /// <param name="cancellationToken">Optional token that when cancelled will unregister the callback.</param>
    /// <exception cref="ObjectDisposedException">Thrown if the service has been disposed.</exception>
    /// <returns>A disposable object that when disposed will unregister the callback.</returns>
    IDisposable ObserveWithRegistration(
        EventToken token,
        Action<IDisposable> callback,
        CancellationToken? cancellationToken = null
    );

    /// <summary>
    /// Setup an event callback that will be unregistered when the dependency injection scope that
    /// this service belongs to is disposed.
    /// </summary>
    /// <typeparam name="TArgs">The type of arguments included with raised events.</typeparam>
    /// <param name="token">The event to register the callback to.</param>
    /// <param name="callback">The callback that will be registered to the event.</param>
    /// <param name="cancellationToken">Optional token that when cancelled will unregister the callback.</param>
    /// <exception cref="ObjectDisposedException">Thrown if the service has been disposed.</exception>
    /// <returns>A disposable object that when disposed will unregister the callback.</returns>
    IDisposable Observe<TArgs>(
        EventToken<TArgs> token,
        Action<TArgs> callback,
        CancellationToken? cancellationToken = null
    );

    /// <summary>
    /// Setup an event callback that will be unregistered when the dependency injection scope that
    /// this service belongs to is disposed.
    /// </summary>
    /// <typeparam name="TArgs">The type of arguments included with raised events.</typeparam>
    /// <typeparam name="TCallbackArgs">
    /// The type of arguments the callback expects.
    /// If an event is raised with arguments that can't be cast to this type the callback will not be invoked.
    /// </typeparam>
    /// <param name="token">The event to register the callback to.</param>
    /// <param name="callback">The callback that will be registered to the event.</param>
    /// <param name="cancellationToken">Optional token that when cancelled will unregister the callback.</param>
    /// <exception cref="ObjectDisposedException">Thrown if the service has been disposed.</exception>
    /// <returns>A disposable object that when disposed will unregister the callback.</returns>
    IDisposable Observe<TArgs, TCallbackArgs>(
        EventToken<TArgs> token,
        Action<TCallbackArgs> callback,
        CancellationToken? cancellationToken = null
    ) where TCallbackArgs : TArgs;

    /// <summary>
    /// Setup an event callback that will be unregistered when the dependency injection scope that
    /// this service belongs to is disposed.
    /// </summary>
    /// <typeparam name="TArgs">The type of arguments included with raised events.</typeparam>
    /// <param name="token">The event to register the callback to.</param>
    /// <param name="callback">
    /// The callback that will be registered to the event.
    /// A disposable is passed to the callback that when disposed will unregister the callback.
    /// </param>
    /// <param name="cancellationToken">Optional token that when cancelled will unregister the callback.</param>
    /// <exception cref="ObjectDisposedException">Thrown if the service has been disposed.</exception>
    /// <returns>A disposable object that when disposed will unregister the callback.</returns>
    IDisposable ObserveWithRegistration<TArgs>(
        EventToken<TArgs> token,
        Action<TArgs, IDisposable> callback,
        CancellationToken? cancellationToken = null
    );

    /// <summary>
    /// Setup an event callback that will be unregistered when the dependency injection scope that
    /// this service belongs to is disposed.
    /// </summary>
    /// <typeparam name="TArgs">The type of arguments included with raised events.</typeparam>
    /// <typeparam name="TCallbackArgs">
    /// The type of arguments the callback expects.
    /// If an event is raised with arguments that can't be cast to this type the callback will not be invoked.
    /// </typeparam>
    /// <param name="token">The event to register the callback to.</param>
    /// <param name="callback">
    /// The callback that will be registered to the event.
    /// A disposable is passed to the callback that when disposed will unregister the callback.
    /// </param>
    /// <param name="cancellationToken">Optional token that when cancelled will unregister the callback.</param>
    /// <exception cref="ObjectDisposedException">Thrown if the service has been disposed.</exception>
    /// <returns>A disposable object that when disposed will unregister the callback.</returns>
    IDisposable ObserveWithRegistration<TArgs, TCallbackArgs>(
        EventToken<TArgs> token,
        Action<TCallbackArgs, IDisposable> callback,
        CancellationToken? cancellationToken = null
    ) where TCallbackArgs : TArgs;

    /// <summary>
    /// Setup an event callback that will be unregistered when the dependency injection scope that
    /// this service belongs to is disposed.
    /// </summary>
    /// <typeparam name="TSender">The type of object that will raise events.</typeparam>
    /// <typeparam name="TArgs">The type of arguments included with raised events.</typeparam>
    /// <param name="token">The event to register the callback to.</param>
    /// <param name="callback">The callback that will be registered to the event.</param>
    /// <param name="cancellationToken">Optional token that when cancelled will unregister the callback.</param>
    /// <exception cref="ObjectDisposedException">Thrown if the service has been disposed.</exception>
    /// <returns>A disposable object that when disposed will unregister the callback.</returns>
    IDisposable Observe<TSender, TArgs>(
        EventToken<TSender, TArgs> token,
        Action<TSender, TArgs> callback,
        CancellationToken? cancellationToken = null
    );

    /// <summary>
    /// Setup an event callback that will be unregistered when the dependency injection scope that
    /// this service belongs to is disposed.
    /// </summary>
    /// <typeparam name="TSender">The type of object that will raise events.</typeparam>
    /// <typeparam name="TArgs">The type of arguments included with raised events.</typeparam>
    /// <typeparam name="TCallbackArgs">
    /// The type of arguments the callback expects.
    /// If an event is raised with arguments that can't be cast to this type the callback will not be invoked.
    /// </typeparam>
    /// <param name="token">The event to register the callback to.</param>
    /// <param name="callback">The callback that will be registered to the event.</param>
    /// <param name="cancellationToken">Optional token that when cancelled will unregister the callback.</param>
    /// <exception cref="ObjectDisposedException">Thrown if the service has been disposed.</exception>
    /// <returns>A disposable object that when disposed will unregister the callback.</returns>
    IDisposable Observe<TSender, TArgs, TCallbackArgs>(
        EventToken<TSender, TArgs> token,
        Action<TSender, TCallbackArgs> callback,
        CancellationToken? cancellationToken = null
    ) where TCallbackArgs : TArgs;

    /// <summary>
    /// Setup an event callback that will be unregistered when the dependency injection scope that
    /// this service belongs to is disposed or after the event has been raised once.
    /// </summary>
    /// <typeparam name="TSender">The type of object that will raise events.</typeparam>
    /// <typeparam name="TArgs">The type of arguments included with raised events.</typeparam>
    /// <param name="token">The event to register the callback to.</param>
    /// <param name="callback">
    /// The callback that will be registered to the event.
    /// A disposable is passed to the callback that when disposed will unregister the callback.
    /// </param>
    /// <param name="cancellationToken">Optional token that when cancelled will unregister the callback.</param>
    /// <exception cref="ObjectDisposedException">Thrown if the service has been disposed.</exception>
    /// <returns>A disposable object that when disposed will unregister the callback.</returns>
    IDisposable ObserveWithRegistration<TSender, TArgs>(
        EventToken<TSender, TArgs> token,
        Action<TSender, TArgs, IDisposable> callback,
        CancellationToken? cancellationToken = null
    );

    /// <summary>
    /// Setup an event callback that will be unregistered when the dependency injection scope that
    /// this service belongs to is disposed or after the event has been raised once.
    /// </summary>
    /// <typeparam name="TSender">The type of object that will raise events.</typeparam>
    /// <typeparam name="TArgs">The type of arguments included with raised events.</typeparam>
    /// <typeparam name="TCallbackArgs">
    /// The type of arguments the callback expects.
    /// If an event is raised with arguments that can't be cast to this type the callback will not be invoked.
    /// </typeparam>
    /// <param name="token">The event to register the callback to.</param>
    /// <param name="callback">
    /// The callback that will be registered to the event.
    /// A disposable is passed to the callback that when disposed will unregister the callback.
    /// </param>
    /// <param name="cancellationToken">Optional token that when cancelled will unregister the callback.</param>
    /// <exception cref="ObjectDisposedException">Thrown if the service has been disposed.</exception>
    /// <returns>A disposable object that when disposed will unregister the callback.</returns>
    IDisposable ObserveWithRegistration<TSender, TArgs, TCallbackArgs>(
        EventToken<TSender, TArgs> token,
        Action<TSender, TCallbackArgs, IDisposable> callback,
        CancellationToken? cancellationToken = null
    ) where TCallbackArgs : TArgs;

    /// <summary>
    /// Setup a PropertyChanging event callback that will be unregistered when the dependency injection scope that
    /// this service belongs to is disposed.
    /// </summary>
    /// <param name="owner">The object that owns the property to observe the changes on.</param>
    /// <param name="property">
    /// An expression in the form of `() => owner.property` that identifies the property to listen for changes on.
    /// </param>
    /// <param name="callback">The callback that will be registered to the event.</param>    
    /// <param name="cancellationToken">Optional token that when cancelled will unregister the callback.</param>
    /// <param name="propertyExpression">When left as null the compiler will set this from the property argument.</param>
    /// <returns>A disposable object that when disposed will unregister the callback.</returns>
    IDisposable ObservePropertyChanging(
        INotifyPropertyChanging owner,
        Func<object> property,
        Action callback,
        CancellationToken? cancellationToken = null,
        [CallerArgumentExpression(nameof(property))] string? propertyExpression = null
    );

    /// <summary>
    /// Setup a PropertyChanging event callback that will be unregistered when the dependency injection scope that
    /// this service belongs to is disposed.
    /// </summary>
    /// <param name="owner">The object that owns the property to observe the changes on.</param>
    /// <param name="property">
    /// An expression in the form of `() => owner.property` that identifies the property to listen for changes on.
    /// </param>
    /// <param name="callback">
    /// The callback that will be registered to the event.
    /// A disposable is passed to the callback that when disposed will unregister the callback.
    /// </param>    
    /// <param name="cancellationToken">Optional token that when cancelled will unregister the callback.</param>
    /// <param name="propertyExpression">When left as null the compiler will set this from the property argument.</param>
    /// <returns>A disposable object that when disposed will unregister the callback.</returns>
    IDisposable ObservePropertyChangingWithRegistration(
        INotifyPropertyChanging owner,
        Func<object> property,
        Action<IDisposable> callback,
        CancellationToken? cancellationToken = null,
        [CallerArgumentExpression(nameof(property))] string? propertyExpression = null
    );

    /// <summary>
    /// Setup a PropertyChanged event callback that will be unregistered when the dependency injection scope that
    /// this service belongs to is disposed.
    /// </summary>
    /// <param name="owner">The object that owns the property to observe the changes on.</param>
    /// <param name="property">
    /// An expression in the form of `() => owner.property` that identifies the property to listen for changes on.
    /// </param>
    /// <param name="callback">The callback that will be registered to the event.</param>    
    /// <param name="cancellationToken">Optional token that when cancelled will unregister the callback.</param>
    /// <param name="propertyExpression">When left as null the compiler will set this from the property argument.</param>
    /// <returns>A disposable object that when disposed will unregister the callback.</returns>
    IDisposable ObservePropertyChanged(
        INotifyPropertyChanged owner,
        Func<object> property,
        Action callback,
        CancellationToken? cancellationToken = null,
        [CallerArgumentExpression(nameof(property))] string? propertyExpression = null
    );

    /// <summary>
    /// Setup a PropertyChanged event callback that will be unregistered when the dependency injection scope that
    /// this service belongs to is disposed.
    /// </summary>
    /// <param name="owner">The object that owns the property to observe the changes on.</param>
    /// <param name="property">
    /// An expression in the form of `() => owner.property` that identifies the property to listen for changes on.
    /// </param>
    /// <param name="callback">
    /// The callback that will be registered to the event.
    /// A disposable is passed to the callback that when disposed will unregister the callback.
    /// </param>    
    /// <param name="cancellationToken">Optional token that when cancelled will unregister the callback.</param>
    /// <param name="propertyExpression">When left as null the compiler will set this from the property argument.</param>
    /// <returns>A disposable object that when disposed will unregister the callback.</returns>
    IDisposable ObservePropertyChangedWithRegistration(
        INotifyPropertyChanged owner,
        Func<object> property,
        Action<IDisposable> callback,
        CancellationToken? cancellationToken = null,
        [CallerArgumentExpression(nameof(property))] string? propertyExpression = null
    );

    /// <summary>
    /// Setup an event callback that will be unregistered when the dependency injection scope that
    /// this service belongs to is disposed.
    /// </summary>
    /// <typeparam name="TDelegate">The type of callback that will be registered.</typeparam>
    /// <param name="register">The action used to register the callback to the event.</param>
    /// <param name="unregister">The action used to unregister the callback from the event.</param>
    /// <param name="callback">The callback that will be registered to the event.</param>
    /// <param name="cancellationToken">Optional token that when cancelled will unregister the callback.</param>
    /// <exception cref="ObjectDisposedException">Thrown if the service has been disposed.</exception>
    /// <returns>A disposable object that when disposed will unregister the callback.</returns>
    IDisposable Observe<TDelegate>(
        Action<TDelegate> register,
        Action<TDelegate> unregister,
        TDelegate callback,
        CancellationToken? cancellationToken = null
    ) where TDelegate : Delegate;
}
