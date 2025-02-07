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

namespace SingleFinite.Mvvm.Internal.Services;

/// <summary>
/// Implementation of the IDebouncer service that uses a timer.
/// </summary>
internal class Debouncer : IDebouncer, IDisposable
{
    #region Fields

    /// <summary>
    /// Set to true when this object has been disposed.
    /// </summary>
    private bool _isDisposed = false;

    /// <summary>
    /// Used to synchronize access to the timer.
    /// </summary>
    private readonly Lock _timerLock = new();

    /// <summary>
    /// The timer used to wait for the debounce delay to pass.
    /// </summary>
    private Timer? _timer = null;

    /// <summary>
    /// The debounce info used when the debounce delay has passed.
    /// </summary>
    private DebounceInfo? _debounceInfo = null;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public void Debounce(
        Action action,
        TimeSpan delay,
        IDispatcher dispatcher,
        Action<ExceptionEventArgs>? onError
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        lock (_timerLock)
        {
            _timer?.Dispose();

            _debounceInfo = new DebounceInfo
            {
                Action = () =>
                {
                    dispatcher.Run(
                        action: action,
                        onError: onError
                    );
                }
            };

            _timer = new(
                callback: OnTimeout,
                state: _debounceInfo,
                dueTime: delay,
                period: Timeout.InfiniteTimeSpan
            );
        }
    }

    /// <inheritdoc/>
    public void Debounce(
        Func<Task> func,
        TimeSpan delay,
        IDispatcher dispatcher,
        Action<ExceptionEventArgs>? onError = null
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        lock (_timerLock)
        {
            _timer?.Dispose();

            _debounceInfo = new DebounceInfo
            {
                Action = () =>
                {
                    dispatcher.Run(
                        func: func,
                        onError: onError
                    );
                }
            };

            _timer = new(
                callback: OnTimeout,
                state: _debounceInfo,
                dueTime: delay,
                period: Timeout.InfiniteTimeSpan
            );
        }
    }

    /// <summary>
    /// The method invoked when a debounce delay has passed.
    /// </summary>
    /// <param name="state">The debounce info to process.</param>
    private void OnTimeout(object? state)
    {
        if (state is not DebounceInfo debounceInfo)
            return;

        lock (_timerLock)
        {
            if (debounceInfo != _debounceInfo)
                return;

            _timer?.Dispose();
            _timer = null;
            _debounceInfo = null;
        }

        debounceInfo.Action();
    }

    /// <summary>
    /// Cancel any pending debounce and dispose of this object.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;

        lock (_timerLock)
        {
            _timer?.Dispose();
            _timer = null;
        }
    }

    #endregion

    #region Types

    /// <summary>
    /// Holds info for a debounce.
    /// </summary>
    private class DebounceInfo
    {
        /// <summary>
        /// The action to invoke after a debounce has passed.
        /// </summary>
        public required Action Action { get; init; }
    }

    #endregion
}
