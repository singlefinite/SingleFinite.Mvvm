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

using SingleFinite.Mvvm.Internal.Services;
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.Internal.Observers;

/// <summary>
/// Observer that debounces events.
/// </summary>
/// <param name="parent">The parent to this observer.</param>
/// <param name="delay">The delay period for debouncing.</param>
/// <param name="dispatcher">
/// The dispatcher to run on after the delay has passed.
/// </param>
/// <param name="debouncer">
/// The debouncer to use for debouncing.  If not specifed a default debouncer
/// will be used.
/// </param>
internal class ObserverDebounce(
    IObserver parent,
    TimeSpan delay,
    IDispatcher dispatcher,
    IDebouncer? debouncer = null
) : ObserverBase(parent), IObserver
{
    #region Fields

    /// <summary>
    /// Debouncer used to debounce.
    /// </summary>
    private readonly IDebouncer _debouncer = debouncer ?? new Debouncer();

    #endregion

    #region Methods

    /// <summary>
    /// Wait for the configured delay and if no new events have been raised
    /// when the delay period has passed pass the event onto the next observer.
    /// </summary>
    /// <returns>
    /// This method always returns false since the next event is only raised
    /// after the delay has passed.
    /// </returns>
    protected override bool OnEvent()
    {
        _debouncer.Debounce(
            action: () => MappedNext?.Invoke(),
            delay: delay,
            dispatcher: dispatcher
        );

        return false;
    }

    #endregion

    #region Events

    /// <summary>
    /// This event is raised when an event has been debounced.
    /// </summary>
    event Action IObserver.Next
    {
        add => MappedNext += value;
        remove => MappedNext -= value;
    }
    private event Action? MappedNext;

    #endregion
}

/// <summary>
/// Observer that debounces events.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
/// <param name="delay">The delay period for debouncing.</param>
/// <param name="dispatcher">The dispatcher to use for debouncing.</param>
/// <param name="debouncer">
/// The debouncer to use for debouncing.  If not specifed a default debouncer
/// will be used.
/// </param>
internal class ObserverDebounce<TArgs>(
    IObserver<TArgs> parent,
    TimeSpan delay,
    IDispatcher dispatcher,
    IDebouncer? debouncer = null
) : ObserverBase<TArgs>(parent), IObserver<TArgs>
{
    #region Fields

    /// <summary>
    /// Debouncer used to debounce.
    /// </summary>
    private readonly IDebouncer _debouncer = debouncer ?? new Debouncer();

    #endregion

    #region Methods

    /// <summary>
    /// Wait for the configured delay and if no new events have been raised
    /// when the delay period has passed pass the event onto the next observer.
    /// </summary>
    /// <param name="args">Arguments passed with the observed event.</param>
    /// <returns>
    /// This method always returns false since the next event is only raised
    /// after the delay has passed.
    /// </returns>
    protected override bool OnEvent(TArgs args)
    {
        _debouncer.Debounce(
            action: () => MappedNext?.Invoke(args),
            delay: delay,
            dispatcher: dispatcher
        );

        return false;
    }

    #endregion

    #region Events

    /// <summary>
    /// This event is raised when an event has been debounced.
    /// </summary>
    event Action<TArgs> IObserver<TArgs>.Next
    {
        add => MappedNext += value;
        remove => MappedNext -= value;
    }
    private event Action<TArgs>? MappedNext;

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
/// <param name="delay">The delay period for debouncing.</param>
/// <param name="dispatcher">The dispatcher to use for debouncing.</param>
/// <param name="debouncer">
/// The debouncer to use for debouncing.  If not specifed a default debouncer
/// will be used.
/// </param>
internal class ObserverDebounce<TSender, TArgs>(
    IObserver<TSender, TArgs> parent,
    TimeSpan delay,
    IDispatcher dispatcher,
    IDebouncer? debouncer = null
) : ObserverBase<TSender, TArgs>(parent), IObserver<TSender, TArgs>
{
    #region Fields

    /// <summary>
    /// Debouncer used to debounce.
    /// </summary>
    private readonly IDebouncer _debouncer = debouncer ?? new Debouncer();

    #endregion

    #region Methods

    /// <summary>
    /// Wait for the configured delay and if no new events have been raised
    /// when the delay period has passed pass the event onto the next observer.
    /// </summary>
    /// <param name="sender">Sender passed with the observed event.</param>
    /// <param name="args">Arguments passed with the observed event.</param>
    /// <returns>
    /// This method always returns false since the next event is only raised
    /// after the delay has passed.
    /// </returns>
    protected override bool OnEvent(TSender sender, TArgs args)
    {
        _debouncer.Debounce(
            action: () => MappedNext?.Invoke(sender, args),
            delay: delay,
            dispatcher: dispatcher
        );

        return false;
    }

    #endregion

    #region Events

    /// <summary>
    /// This event is raised when an event has been debounced.
    /// </summary>
    event Action<TSender, TArgs> IObserver<TSender, TArgs>.Next
    {
        add => MappedNext += value;
        remove => MappedNext -= value;
    }
    private event Action<TSender, TArgs>? MappedNext;

    #endregion
}
