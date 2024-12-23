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
using SingleFinite.Mvvm.Internal.Observers;
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
    public IObserver Observe(
        Observable observable,
        CancellationToken? cancellationToken = default
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var observer = new ObserverDisposeListener(
            parent: observable.Observe()
        );

        void DisposedHandler()
        {
            _observerList.Remove(observer);
            observer.Disposed -= DisposedHandler;
        }

        observer.Disposed += DisposedHandler;

        _observerList.Add(observer);
        cancellationToken?.Register(observer.Dispose);

        return observer;
    }

    /// <inheritdoc/>
    public IObserver Observe(
        Observable observable,
        Action callback,
        CancellationToken? cancellationToken = default
    ) => Observe(observable, cancellationToken).OnEach(callback);

    /// <inheritdoc/>
    public IObserver<TArgs> Observe<TArgs>(
        Observable<TArgs> observable,
        CancellationToken? cancellationToken = default
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var observer = new ObserverDisposeListener<TArgs>(
            parent: observable.Observe()
        );

        void DisposedHandler()
        {
            _observerList.Remove(observer);
            observer.Disposed -= DisposedHandler;
        }

        observer.Disposed += DisposedHandler;

        _observerList.Add(observer);
        cancellationToken?.Register(observer.Dispose);

        return observer;
    }

    /// <inheritdoc/>
    public IObserver<TArgs> Observe<TArgs>(
        Observable<TArgs> observable,
        Action<TArgs> callback,
        CancellationToken? cancellationToken = default
    ) => Observe<TArgs>(observable, cancellationToken).OnEach(callback);

    /// <inheritdoc/>
    public IObserver<TSender, TArgs> Observe<TSender, TArgs>(
        Observable<TSender, TArgs> observable,
        CancellationToken? cancellationToken = default
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var observer = new ObserverDisposeListener<TSender, TArgs>(
            parent: observable.Observe()
        );

        void DisposedHandler()
        {
            _observerList.Remove(observer);
            observer.Disposed -= DisposedHandler;
        }

        observer.Disposed += DisposedHandler;

        _observerList.Add(observer);
        cancellationToken?.Register(observer.Dispose);

        return observer;
    }

    /// <inheritdoc/>
    public IObserver<TSender, TArgs> Observe<TSender, TArgs>(
        Observable<TSender, TArgs> observable,
        Action<TSender, TArgs> callback,
        CancellationToken? cancellationToken = default
    ) => Observe(observable, cancellationToken).OnEach(callback);

    /// <inheritdoc/>
    public IAsyncObserver Observe(
        AsyncObservable observable,
        CancellationToken? cancellationToken = default
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var observer = new AsyncObserverDisposeListener(
            parent: observable.Observe()
        );

        void DisposedHandler()
        {
            _observerList.Remove(observer);
            observer.Disposed -= DisposedHandler;
        }

        observer.Disposed += DisposedHandler;

        _observerList.Add(observer);
        cancellationToken?.Register(observer.Dispose);

        return observer;
    }

    /// <inheritdoc/>
    public IAsyncObserver Observe(
        AsyncObservable observable,
        Func<Task> callback,
        CancellationToken? cancellationToken = default
    ) => Observe(observable, cancellationToken).OnEach(callback);

    /// <inheritdoc/>
    public IAsyncObserver<TArgs> Observe<TArgs>(
        AsyncObservable<TArgs> observable,
        CancellationToken? cancellationToken = default
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var observer = new AsyncObserverDisposeListener<TArgs>(
            parent: observable.Observe()
        );

        void DisposedHandler()
        {
            _observerList.Remove(observer);
            observer.Disposed -= DisposedHandler;
        }

        observer.Disposed += DisposedHandler;

        _observerList.Add(observer);
        cancellationToken?.Register(observer.Dispose);

        return observer;
    }

    /// <inheritdoc/>
    public IAsyncObserver<TArgs> Observe<TArgs>(
        AsyncObservable<TArgs> observable,
        Func<TArgs, Task> callback,
        CancellationToken? cancellationToken = default
    ) => Observe<TArgs>(observable, cancellationToken).OnEach(callback);

    /// <inheritdoc/>
    public IAsyncObserver<TSender, TArgs> Observe<TSender, TArgs>(
        AsyncObservable<TSender, TArgs> observable,
        CancellationToken? cancellationToken = default
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var observer = new AsyncObserverDisposeListener<TSender, TArgs>(
            parent: observable.Observe()
        );

        void DisposedHandler()
        {
            _observerList.Remove(observer);
            observer.Disposed -= DisposedHandler;
        }

        observer.Disposed += DisposedHandler;

        _observerList.Add(observer);
        cancellationToken?.Register(observer.Dispose);

        return observer;
    }

    /// <inheritdoc/>
    public IAsyncObserver<TSender, TArgs> Observe<TSender, TArgs>(
        AsyncObservable<TSender, TArgs> observable,
        Func<TSender, TArgs, Task> callback,
        CancellationToken? cancellationToken = default
    ) => Observe(observable, cancellationToken).OnEach(callback);

    /// <inheritdoc/>
    public IObserver<string?> ObservePropertyChanging(
        INotifyPropertyChanging owner,
        Action<string?> callback,
        CancellationToken? cancellationToken = default
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var observableSource = new ObservableSource<string?>();

        void Handler(object? sender, PropertyChangingEventArgs e)
        {
            observableSource.RaiseEvent(e.PropertyName);
        }

        owner.PropertyChanging += Handler;

        var observer = new ObserverDisposeListener<string?>(
            parent: new ObserverSource<string?>(
                observable: observableSource.Observable
            )
        );

        void DisposedHandler()
        {
            _observerList.Remove(observer);
            owner.PropertyChanging -= Handler;
            observer.Disposed -= DisposedHandler;
        }

        observer.Disposed += DisposedHandler;

        _observerList.Add(observer);
        cancellationToken?.Register(observer.Dispose);

        return observer.OnEach(callback);
    }

    /// <inheritdoc/>
    public IObserver ObservePropertyChanging(
        INotifyPropertyChanging owner,
        Func<object?> property,
        Action callback,
        CancellationToken? cancellationToken = default,
        [CallerArgumentExpression(nameof(property))]
        string? propertyExpression = default
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var observableSource = new ObservableSource();

        void Handler(object? sender, PropertyChangingEventArgs e)
        {
            observableSource.RaiseEvent();
        }

        var propertyName = Component.ParsePropertyName(propertyExpression);
        owner.PropertyChanging += Handler;

        var observer = new ObserverDisposeListener(
            parent: new ObserverSource(observable: observableSource.Observable)
        );

        void DisposedHandler()
        {
            _observerList.Remove(observer);
            owner.PropertyChanging -= Handler;
            observer.Disposed -= DisposedHandler;
        }

        observer.Disposed += DisposedHandler;

        _observerList.Add(observer);
        cancellationToken?.Register(observer.Dispose);

        return observer.OnEach(callback);
    }

    /// <inheritdoc/>
    public IObserver<string?> ObservePropertyChanged(
        INotifyPropertyChanged owner,
        Action<string?> callback,
        CancellationToken? cancellationToken = default
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var observableSource = new ObservableSource<string?>();

        void Handler(object? sender, PropertyChangedEventArgs e)
        {
            observableSource.RaiseEvent(e.PropertyName);
        }

        owner.PropertyChanged += Handler;

        var observer = new ObserverDisposeListener<string?>(
            parent: new ObserverSource<string?>(
                observable: observableSource.Observable
            )
        );

        void DisposedHandler()
        {
            _observerList.Remove(observer);
            owner.PropertyChanged -= Handler;
            observer.Disposed -= DisposedHandler;
        }

        observer.Disposed += DisposedHandler;

        _observerList.Add(observer);
        cancellationToken?.Register(observer.Dispose);

        return observer.OnEach(callback);
    }

    /// <inheritdoc/>
    public IObserver ObservePropertyChanged(
        INotifyPropertyChanged owner,
        Func<object?> property,
        Action callback,
        CancellationToken? cancellationToken = default,
        [CallerArgumentExpression(nameof(property))]
        string? propertyExpression = default
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var observableSource = new ObservableSource();

        void Handler(object? sender, PropertyChangedEventArgs e)
        {
            observableSource.RaiseEvent();
        }

        var propertyName = Component.ParsePropertyName(propertyExpression);
        owner.PropertyChanged += Handler;

        var observer = new ObserverDisposeListener(
            parent: new ObserverSource(observable: observableSource.Observable)
        );

        void DisposedHandler()
        {
            _observerList.Remove(observer);
            owner.PropertyChanged -= Handler;
            observer.Disposed -= DisposedHandler;
        }

        observer.Disposed += DisposedHandler;

        _observerList.Add(observer);
        cancellationToken?.Register(observer.Dispose);

        return observer.OnEach(callback);
    }

    /// <inheritdoc/>
    public IDisposable Observe<TDelegate>(
        Action<TDelegate> register,
        Action<TDelegate> unregister,
        TDelegate handler,
        CancellationToken? cancellationToken = default
    ) where TDelegate : Delegate
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        register(handler);

        IDisposable? disposable = null;
        disposable = new ActionOnDispose(() =>
        {
            _observerList.Remove(disposable.Require());
            unregister(handler);
        });

        _observerList.Add(disposable);
        cancellationToken?.Register(disposable.Dispose);

        return disposable;
    }

    /// <summary>
    /// Dispose of all observers created through this service.
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

    #endregion
}
