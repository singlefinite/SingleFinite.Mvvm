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

namespace SingleFinite.Mvvm.Internal.Observers;

/// <summary>
/// An observer that limits the number of observers that can be executing at the
/// same time.
/// </summary>
internal class AsyncObserverLimit : AsyncObserverBase
{
    #region Fields

    /// <summary>
    /// Used to buffer events.
    /// </summary>
    private readonly SemaphoreSlim _semaphore;

    /// <summary>
    /// The max number of events that can be buffered.
    /// </summary>
    private readonly int _maxBuffer;

    /// <summary>
    /// Holds the number of events currently being buffered.
    /// </summary>
    private int _bufferCount = 0;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    /// <param name="maxConcurrent">
    /// The max number of observers allowed to be executing at the same time.
    /// If a new event occurs and there are already max number of observers
    /// executing the event will be buffered until enough executing observers
    /// complete to allow the new event to be passed on.  Must be zero or
    /// greater or an exception will be thrown.
    /// </param>
    /// <param name="maxBuffer">
    /// The max number of events that will be buffered if there are already max
    /// number of concurrent observers executing.  If a new event occurs and
    /// there are already max number of events buffered the event will be
    /// ignored.  If this is set to -1 there is no limit.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if maxConcurrent is less than zero or maxBuffer is less than -1.
    /// </exception>
    public AsyncObserverLimit(
        IAsyncObserver parent,
        int maxConcurrent,
        int maxBuffer
    ) : base(parent)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxConcurrent);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxBuffer, -1);

        _maxBuffer = maxBuffer;

        _semaphore = new(
            initialCount: maxConcurrent,
            maxCount: maxConcurrent
        );
    }

    #endregion

    #region Methods

    /// <summary>
    /// Only pass the event on if the limit hasn't been reached.
    /// </summary>
    /// <returns>This method always returns false.</returns>
    protected async override Task<bool> OnEventAsync()
    {
        var isBuffered = _maxBuffer != -1 && _semaphore.CurrentCount == 0;
        if (isBuffered)
        {
            if (_bufferCount == _maxBuffer)
                return false;

            _bufferCount++;
        }

        try
        {
            await _semaphore.WaitAsync();

            if (isBuffered)
                _bufferCount--;

            await RaiseNextEventAsync();
        }
        finally
        {
            _semaphore.Release();
        }

        return false;
    }

    #endregion
}

/// <summary>
/// An observer that limits the number of observers that can be executing at the
/// same time.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
internal class AsyncObserverLimit<TArgs> : AsyncObserverBase<TArgs>
{
    #region Fields

    /// <summary>
    /// Used to buffer events.
    /// </summary>
    private readonly SemaphoreSlim _semaphore;

    /// <summary>
    /// The max number of events that can be buffered.
    /// </summary>
    private readonly int _maxBuffer;

    /// <summary>
    /// Holds the number of events currently being buffered.
    /// </summary>
    private int _bufferCount = 0;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    /// <param name="maxConcurrent">
    /// The max number of observers allowed to be executing at the same time.
    /// If a new event occurs and there are already max number of observers
    /// executing the event will be buffered until enough executing observers
    /// complete to allow the new event to be passed on.  Must be zero or
    /// greater or an exception will be thrown.
    /// </param>
    /// <param name="maxBuffer">
    /// The max number of events that will be buffered if there are already max
    /// number of concurrent observers executing.  If a new event occurs and
    /// there are already max number of events buffered the event will be
    /// ignored.  If this is set to -1 there is no limit.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if maxConcurrent is less than zero or maxBuffer is less than -1.
    /// </exception>
    public AsyncObserverLimit(
        IAsyncObserver<TArgs> parent,
        int maxConcurrent,
        int maxBuffer
    ) : base(parent)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxConcurrent);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxBuffer, -1);

        _maxBuffer = maxBuffer;

        _semaphore = new(
            initialCount: maxConcurrent,
            maxCount: maxConcurrent
        );
    }

    #endregion

    #region Methods

    /// <summary>
    /// Only pass the event on if the limit hasn't been reached.
    /// </summary>
    /// <param name="args">
    /// Arguments passed with the observed event that are passed to the next
    /// observer.
    /// </param>
    /// <returns>This method always returns false.</returns>
    protected async override Task<bool> OnEventAsync(TArgs args)
    {
        var isBuffered = _maxBuffer != -1 && _semaphore.CurrentCount == 0;
        if (isBuffered)
        {
            if (_bufferCount == _maxBuffer)
                return false;

            _bufferCount++;
        }

        try
        {
            await _semaphore.WaitAsync();

            if (isBuffered)
                _bufferCount--;

            await RaiseNextEventAsync(args);
        }
        finally
        {
            _semaphore.Release();
        }

        return false;
    }

    #endregion
}
