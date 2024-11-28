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

using System.ComponentModel;
using System.Runtime.CompilerServices;
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.Internal.Services;

/// <summary>
/// Implementation of <see cref="IEventObserver"/>.
/// </summary>
internal sealed class EventObserver : IEventObserver, IDisposable
{
    #region Fields

    /// <summary>
    /// Set to true when this service has been disposed.
    /// </summary>
    private bool _isDisposed = false;

    /// <summary>
    /// Holds disposable objects that will unregister callbacks when disposed.
    /// </summary>
    private readonly List<IDisposable> _observerList = [];

    #endregion

    #region Methods

    /// <inheritdoc/>
    public IDisposable Observe(
        EventToken token,
        Action callback,
        CancellationToken? cancellationToken = null
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        return Register(token.Register(callback), cancellationToken);
    }

    /// <inheritdoc/>
    public IDisposable ObserveWithUnregister(
        EventToken token,
        Action<IDisposable> callback,
        CancellationToken? cancellationToken = null
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        return Register(token.Register(callback), cancellationToken);
    }

    /// <inheritdoc/>
    public IDisposable Observe<TArgs>(
        EventToken<TArgs> token,
        Action<TArgs> callback,
        CancellationToken? cancellationToken = null
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        return Register(token.Register(callback), cancellationToken);
    }

    /// <inheritdoc/>
    public IDisposable Observe<TArgs>(
        EventToken<TArgs> token,
        Action callback,
        CancellationToken? cancellationToken = null
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        return Register(token.Register(callback), cancellationToken);
    }

    /// <inheritdoc/>
    public IDisposable Observe<TArgs, TCallbackArgs>(
        EventToken<TArgs> token,
        Action<TCallbackArgs> callback,
        CancellationToken? cancellationToken = null
    ) where TCallbackArgs : TArgs
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        void Handler(TArgs args)
        {
            if (args is TCallbackArgs callbackArgs)
                callback(callbackArgs);
        }

        return Register(token.Register(Handler), cancellationToken);
    }

    /// <inheritdoc/>
    public IDisposable ObserveWithUnregister<TArgs>(
        EventToken<TArgs> token,
        Action<TArgs, IDisposable> callback,
        CancellationToken? cancellationToken = null
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        return Register(token.Register(callback), cancellationToken);
    }

    /// <inheritdoc/>
    public IDisposable ObserveWithUnregister<TArgs, TCallbackArgs>(
        EventToken<TArgs> token,
        Action<TCallbackArgs, IDisposable> callback,
        CancellationToken? cancellationToken = null
    ) where TCallbackArgs : TArgs
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        void Handler(TArgs args, IDisposable observer)
        {
            if (args is TCallbackArgs callbackArgs)
                callback(callbackArgs, observer);
        }

        return Register(token.Register(Handler), cancellationToken);
    }

    /// <inheritdoc/>
    public IDisposable Observe<TSender, TArgs>(
        EventToken<TSender, TArgs> token,
        Action<TSender, TArgs> callback,
        CancellationToken? cancellationToken = null
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        return Register(token.Register(callback), cancellationToken);
    }

    /// <inheritdoc/>
    public IDisposable Observe<TSender, TArgs>(
        EventToken<TSender, TArgs> token,
        Action callback,
        CancellationToken? cancellationToken = null
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        return Register(token.Register(callback), cancellationToken);
    }

    /// <inheritdoc/>
    public IDisposable Observe<TSender, TArgs, TCallbackArgs>(
        EventToken<TSender, TArgs> token,
        Action<TSender, TCallbackArgs> callback,
        CancellationToken? cancellationToken = null
    ) where TCallbackArgs : TArgs
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        void Handler(TSender sender, TArgs args)
        {
            if (args is TCallbackArgs callbackArgs)
                callback(sender, callbackArgs);
        }

        return Register(token.Register(Handler), cancellationToken);
    }

    /// <inheritdoc/>
    public IDisposable ObserveWithUnregister<TSender, TArgs>(
        EventToken<TSender, TArgs> token,
        Action<TSender, TArgs, IDisposable> callback,
        CancellationToken? cancellationToken = null
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        return Register(token.Register(callback), cancellationToken);
    }

    /// <inheritdoc/>
    public IDisposable ObserveWithUnregister<TSender, TArgs, TCallbackArgs>(
        EventToken<TSender, TArgs> token,
        Action<TSender, TCallbackArgs, IDisposable> callback,
        CancellationToken? cancellationToken = null
    ) where TCallbackArgs : TArgs
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        void Handler(TSender sender, TArgs args, IDisposable observer)
        {
            if (args is TCallbackArgs callbackArgs)
                callback(sender, callbackArgs, observer);
        }

        return Register(token.Register(Handler), cancellationToken);
    }

    /// <inheritdoc/>
    public IDisposable ObservePropertyChanging(
        INotifyPropertyChanging owner,
        Func<object> property,
        Action callback,
        CancellationToken? cancellationToken = null,
        [CallerArgumentExpression(nameof(property))]
        string? propertyExpression = null
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        void Handler(object? sender, PropertyChangingEventArgs e)
        {
            callback();
        }

        var propertyName = ParsePropertyName(propertyExpression);
        owner.PropertyChanging += Handler;
        return Register(
            new ActionOnDispose(() => owner.PropertyChanging -= Handler),
            cancellationToken
        );
    }

    /// <inheritdoc/>
    public IDisposable ObservePropertyChangingWithUnregister(
        INotifyPropertyChanging owner,
        Func<object> property,
        Action<IDisposable> callback,
        CancellationToken? cancellationToken = null,
        [CallerArgumentExpression(nameof(property))]
        string? propertyExpression = null
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        IDisposable? observer = null;
        void Handler(object? sender, PropertyChangingEventArgs e)
        {
            callback(observer.Require());
        }

        var propertyName = ParsePropertyName(propertyExpression);
        owner.PropertyChanging += Handler;
        observer = Register(
            new ActionOnDispose(() => owner.PropertyChanging -= Handler),
            cancellationToken
        );

        return observer;
    }

    /// <inheritdoc/>
    public IDisposable ObservePropertyChanged(
        INotifyPropertyChanged owner,
        Func<object> property,
        Action callback,
        CancellationToken? cancellationToken = null,
        [CallerArgumentExpression(nameof(property))]
        string? propertyExpression = null
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        void Handler(object? sender, PropertyChangedEventArgs e)
        {
            callback();
        }

        var propertyName = ParsePropertyName(propertyExpression);
        owner.PropertyChanged += Handler;
        return Register(
            new ActionOnDispose(() => owner.PropertyChanged -= Handler),
            cancellationToken
        );
    }

    public IDisposable ObservePropertyChangedWithUnregister(
        INotifyPropertyChanged owner,
        Func<object> property,
        Action<IDisposable> callback,
        CancellationToken? cancellationToken = null,
        [CallerArgumentExpression(nameof(property))]
        string? propertyExpression = null
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        IDisposable? observer = null;
        void Handler(object? sender, PropertyChangedEventArgs e)
        {
            callback(observer.Require());
        }

        var propertyName = ParsePropertyName(propertyExpression);
        owner.PropertyChanged += Handler;
        observer = Register(
            new ActionOnDispose(() => owner.PropertyChanged -= Handler),
            cancellationToken
        );

        return observer;
    }

    /// <inheritdoc/>
    public IDisposable Observe<TDelegate>(
        Action<TDelegate> register,
        Action<TDelegate> unregister,
        TDelegate handler,
        CancellationToken? cancellationToken = null
    ) where TDelegate : Delegate
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        register(handler);
        return Register(
            new ActionOnDispose(() => unregister(handler)),
            cancellationToken
        );
    }

    /// <summary>
    /// Unregister all callbacks.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        var observers = _observerList.ToArray();
        foreach (var observer in observers)
            observer.Dispose();

        _observerList.Clear();

        _isDisposed = true;
    }

    /// <summary>
    /// Add the given observer to the observer list.
    /// </summary>
    /// <param name="observer">
    /// The observer to to add to the observer list.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional token that when cancelled will dispose of the observer.
    /// </param>
    /// <returns>
    /// A disposable that when is disposed will remove the observer from the
    /// list and dispose of the observer.
    /// </returns>
    private ActionOnDispose Register(
        IDisposable observer,
        CancellationToken? cancellationToken
    )
    {
        ActionOnDispose? removeObserver = null;
        removeObserver = new ActionOnDispose(() =>
        {
            if (removeObserver != null)
                _observerList.Remove(removeObserver);
            observer.Dispose();
        });

        _observerList.Add(removeObserver);

        cancellationToken?.Register(() => removeObserver.Dispose());

        return removeObserver;
    }

    /// <summary>
    /// Parse the property name from the given expression.
    /// </summary>
    /// <param name="propertyExpression">
    /// The expression to parse the property name from.
    /// The expected format for the expression is '() => owner.property'.
    /// </param>
    /// <returns>The property name parsed from the expression.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the expression is not in the expected format.
    /// </exception>
    private static string ParsePropertyName(string? propertyExpression)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(
            propertyExpression,
            nameof(propertyExpression)
        );

        var dotIndex = propertyExpression.IndexOf('.');
        if (dotIndex == -1)
            throw new ArgumentException(
                message: "expression must be in the form of 'owner.property'",
                paramName: nameof(propertyExpression)
            );

        return propertyExpression[(dotIndex + 1)..];
    }

    #endregion
}
