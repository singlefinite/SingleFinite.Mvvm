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
/// An observer that raises the next event in the observer chain as async
/// by using the provided dispatcher..
/// </summary>
/// <param name="parent">The parent to this observer.</param>
/// <param name="dispatcher">
/// The dispatcher the async events will be run with.
/// </param>
/// <param name="onError">
/// Optional action that is invoked if an exception is thrown from the async
/// observers.
/// </param>
internal class ObserverToAsync(
    IObserver parent,
    IDispatcher dispatcher,
    Action<ExceptionEventArgs>? onError
) : ObserverBase(parent), IAsyncObserver
{
    #region Methods

    /// <summary>
    /// Raise the next event with the dispatcher.
    /// </summary>
    /// <returns>This method always returns false.</returns>
    protected override bool OnEvent()
    {
        dispatcher.Run(
            func: async () =>
            {
                if (MappedNext is not null)
                    await MappedNext.Invoke();
            },
            onError: onError
        );
        return false;
    }

    #endregion

    #region Events

    /// <summary>
    /// This event is raised on the dispatcher for an observed event to pass
    /// down the observer chain.
    /// </summary>
    event Func<Task> IAsyncObserver.Next
    {
        add => MappedNext += value;
        remove => MappedNext -= value;
    }
    private event Func<Task>? MappedNext;

    #endregion
}

/// <summary>
/// An observer that invokes the given callback whenever an event is observed.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
/// <param name="dispatcher">
/// The dispatcher the async events will be run with.
/// </param>
/// <param name="onError">
/// Optional action that is invoked if an exception is thrown from the async
/// observers.
/// </param>
internal class ObserverToAsync<TArgs>(
    IObserver<TArgs> parent,
    IDispatcher dispatcher,
    Action<ExceptionEventArgs>? onError
) : ObserverBase<TArgs>(parent), IAsyncObserver<TArgs>
{
    #region Methods

    /// <summary>
    /// Raise the next event with the dispatcher.
    /// </summary>
    /// <param name="args">The arguments to pass with the event.</param>
    /// <returns>This method always returns false.</returns>
    protected override bool OnEvent(TArgs args)
    {
        dispatcher.Run(
            func: async () =>
            {
                if (MappedNext is not null)
                    await MappedNext.Invoke(args);
            },
            onError: onError
        );
        return false;
    }

    #endregion

    #region Events

    /// <summary>
    /// This event is raised on the dispatcher for an observed event to pass
    /// down the observer chain.
    /// </summary>
    event Func<TArgs, Task> IAsyncObserver<TArgs>.Next
    {
        add => MappedNext += value;
        remove => MappedNext -= value;
    }
    private event Func<TArgs, Task>? MappedNext;

    #endregion
}

/// <summary>
/// An observer that invokes the given callback whenever an event is observed.
/// </summary>
/// <typeparam name="TSender">
/// The type of sender passed with observed events.
/// </typeparam>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
/// <param name="dispatcher">
/// The dispatcher the async events will be run with.
/// </param>
/// <param name="onError">
/// Optional action that is invoked if an exception is thrown from the async
/// observers.
/// </param>
internal class ObserverToAsync<TSender, TArgs>(
    IObserver<TSender, TArgs> parent,
    IDispatcher dispatcher,
    Action<ExceptionEventArgs>? onError
) : ObserverBase<TSender, TArgs>(parent), IAsyncObserver<TSender, TArgs>
{
    #region Methods

    /// <summary>
    /// Raise the next event with the dispatcher.
    /// </summary>
    /// <param name="sender">The sender to pass with the event.</param>
    /// <param name="args">The arguments to pass with the event.</param>
    /// <returns>This method always returns false.</returns>
    protected override bool OnEvent(TSender sender, TArgs args)
    {
        dispatcher.Run(
            func: async () =>
            {
                if (MappedNext is not null)
                    await MappedNext.Invoke(sender, args);
            },
            onError: onError
        );
        return false;
    }

    #endregion

    #region Events

    /// <summary>
    /// This event is raised on the dispatcher for an observed event to pass
    /// down the observer chain.
    /// </summary>
    event Func<TSender, TArgs, Task> IAsyncObserver<TSender, TArgs>.Next
    {
        add => MappedNext += value;
        remove => MappedNext -= value;
    }
    private event Func<TSender, TArgs, Task>? MappedNext;

    #endregion
}
