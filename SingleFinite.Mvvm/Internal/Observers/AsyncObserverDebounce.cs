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

using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.Internal.Observers;

/// <summary>
/// Observer that debounces events.
/// </summary>
/// <param name="parent">The parent to this observer.</param>
/// <param name="dispatcher">The dispatcher to use for debouncing.</param>
/// <param name="delay">The delay period for debouncing.</param>
internal class AsyncObserverDebounce(
    IAsyncObserver parent,
    IDispatcherWithCancellation dispatcher,
    TimeSpan delay
) : AsyncObserverBase(parent), IAsyncObserver
{
    #region Fields

    /// <summary>
    /// Cancellation token source used to cancel any pending event.
    /// </summary>
    private CancellationTokenSource? _cancellationTokenSource = null;

    #endregion

    #region Methods

    /// <summary>
    /// Wait for the configured delay and if no new events have been raised
    /// when the delay period has elapsed pass the event onto the next observer.
    /// </summary>
    /// <returns>
    /// This method always returns false since the next event is only raised
    /// after the delay has elapsed.
    /// </returns>
    protected override Task<bool> OnEventAsync()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new();

        dispatcher.Run(
            func: async cancellationToken =>
            {
                try
                {
                    await Task.Delay(
                        delay: delay,
                        cancellationToken: cancellationToken
                    );
                }
                catch (TaskCanceledException) { /* ignore canceled tasks */ }

                if (!cancellationToken.IsCancellationRequested)
                    MappedEvent?.Invoke();
            },
            _cancellationTokenSource.Token
        );

        return Task.FromResult(false);
    }

    #endregion

    #region Events

    /// <summary>
    /// This event is raised when an event has been debounced.
    /// </summary>
    event Func<Task> IAsyncObserver.Event
    {
        add => MappedEvent += value;
        remove => MappedEvent -= value;
    }
    private event Func<Task>? MappedEvent;

    #endregion
}

/// <summary>
/// Observer that debounces events.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
/// <param name="dispatcher">The dispatcher to use for debouncing.</param>
/// <param name="delay">The delay period for debouncing.</param>
internal class AsyncObserverDebounce<TArgs>(
    IAsyncObserver<TArgs> parent,
    IDispatcherWithCancellation dispatcher,
    TimeSpan delay
) : AsyncObserverBase<TArgs>(parent), IAsyncObserver<TArgs>
{
    #region Fields

    /// <summary>
    /// Cancellation token source used to cancel any pending event.
    /// </summary>
    private CancellationTokenSource? _cancellationTokenSource = null;

    #endregion

    #region Methods

    /// <summary>
    /// Wait for the configured delay and if no new events have been raised
    /// when the delay period has elapsed pass the event onto the next observer.
    /// </summary>
    /// <param name="args">Arguments passed with the observed event.</param>
    /// <returns>
    /// This method always returns false since the next event is only raised
    /// after the delay has elapsed.
    /// </returns>
    protected override Task<bool> OnEventAsync(TArgs args)
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new();

        dispatcher.Run(
            func: async cancellationToken =>
            {
                try
                {
                    await Task.Delay(
                        delay: delay,
                        cancellationToken: cancellationToken
                    );
                }
                catch (TaskCanceledException) { /* ignore canceled tasks */ }

                if (!cancellationToken.IsCancellationRequested)
                    MappedEvent?.Invoke(args);
            },
            _cancellationTokenSource.Token
        );

        return Task.FromResult(false);
    }

    #endregion

    #region Events

    /// <summary>
    /// This event is raised when an event has been debounced.
    /// </summary>
    event Func<TArgs, Task> IAsyncObserver<TArgs>.Event
    {
        add => MappedEvent += value;
        remove => MappedEvent -= value;
    }
    private event Func<TArgs, Task>? MappedEvent;

    #endregion
}

/// <summary>
/// Observer that debounces events.
/// </summary>
/// <typeparam name="TSender">
/// The type of sender passed with observed events.
/// </typeparam>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
/// <param name="dispatcher">The dispatcher to use for debouncing.</param>
/// <param name="delay">The delay period for debouncing.</param>
internal class AsyncObserverDebounce<TSender, TArgs>(
    IAsyncObserver<TSender, TArgs> parent,
    IDispatcherWithCancellation dispatcher,
    TimeSpan delay
) : AsyncObserverBase<TSender, TArgs>(parent), IAsyncObserver<TSender, TArgs>
{
    #region Fields

    /// <summary>
    /// Cancellation token source used to cancel any pending event.
    /// </summary>
    private CancellationTokenSource? _cancellationTokenSource = null;

    #endregion

    #region Methods

    /// <summary>
    /// Wait for the configured delay and if no new events have been raised
    /// when the delay period has elapsed pass the event onto the next observer.
    /// </summary>
    /// <param name="sender">Sender passed with the observed event.</param>
    /// <param name="args">Arguments passed with the observed event.</param>
    /// <returns>
    /// This method always returns false since the next event is only raised
    /// after the delay has elapsed.
    /// </returns>
    protected override Task<bool> OnEventAsync(TSender sender, TArgs args)
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new();

        dispatcher.Run(
            func: async cancellationToken =>
            {
                try
                {
                    await Task.Delay(
                        delay: delay,
                        cancellationToken: cancellationToken
                    );
                }
                catch (TaskCanceledException) { /* ignore canceled tasks */ }

                if (!cancellationToken.IsCancellationRequested)
                    MappedEvent?.Invoke(sender, args);
            },
            _cancellationTokenSource.Token
        );

        return Task.FromResult(false);
    }

    #endregion

    #region Events

    /// <summary>
    /// This event is raised when an event has been debounced.
    /// </summary>
    event Func<TSender, TArgs, Task> IAsyncObserver<TSender, TArgs>.Event
    {
        add => MappedEvent += value;
        remove => MappedEvent -= value;
    }
    private event Func<TSender, TArgs, Task>? MappedEvent;

    #endregion
}
