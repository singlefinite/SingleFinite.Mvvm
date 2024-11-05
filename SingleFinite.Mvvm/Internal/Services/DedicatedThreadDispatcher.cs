// MIT License
// Copyright (c) 2024 SingleFinite
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

using System.Collections.Concurrent;
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.Internal.Services;

/// <summary>
/// Default implementation of <see cref="IAppMainDispatcher"/> that queues execution of functions and actions on to a dedicated thread.
/// Normally this service should be replaced with a platform specific service that will dispatch to the UI thread for that platform.
/// However, this service is useful for unit testing when there is no UI thread provided by the unit testing framework.
/// </summary>
internal sealed class DedicatedThreadDispatcher : DispatcherBase, IAppMainDispatcher, IDisposable
{
    #region Fields

    /// <summary>
    /// The single thread that dispatched functions and actions will be run on.
    /// </summary>
    private readonly Thread _thread;

    /// <summary>
    /// The blocking queue used to dispatch functions and actions through.
    /// </summary>
    private readonly BlockingCollection<Action> _queue = [];

    /// <summary>
    /// Set to true when this service has been disposed.
    /// </summary>
    private bool _isDisposed = false;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public DedicatedThreadDispatcher()
    {
        _thread = new(ThreadStart)
        {
            Name = "SingleFinite.Mvvm.Internal.Services.DedicatedThreadDispatcher"
        };
        _thread.Start();
    }

    #endregion

    #region Methods

    /// <summary>
    /// This method is run on the dedicated thread and will execute functions and actions from the queue
    /// until the queue has been marked completed.
    /// </summary>
    private void ThreadStart()
    {
        var test = SynchronizationContext.Current;
        try
        {
            while (!_queue.IsCompleted)
            {
                var action = _queue.Take();
                action();
            }
        }
        catch (InvalidOperationException)
        {
            // Ignore exception thrown when action queue has been marked complete.
        }
    }

    /// <summary>
    /// Implements <see cref="IDispatcher"/> by dispatching the function execution to the dedicated thread.
    /// If this method is called from the dedicated thread the function will be executed right away isntead
    /// of being queued.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>A task that runs until the function has completed.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if this object has been disposed.</exception>
    public override Task<TResult> RunAsync<TResult>(Func<Task<TResult>> func)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (Thread.CurrentThread == _thread)
            return func();

        var taskCompletionSource = new TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously);

        _queue.Add(async () =>
        {
            try
            {
                var result = await func();
                taskCompletionSource.SetResult(result);
            }
            catch (Exception ex)
            {
                taskCompletionSource.SetException(ex);
            }
        });

        return taskCompletionSource.Task;
    }

    /// <summary>
    /// Mark the queue as complete when disposed and wait for the dedicated thread to stop.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        _queue.CompleteAdding();
        _thread.Join();

        _isDisposed = true;
    }

    #endregion
}
