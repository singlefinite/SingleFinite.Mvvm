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

namespace SingleFinite.Mvvm.Internal;

/// <summary>
/// A class that is either in the open or closed state and will raise an event 
/// when moving from open to closed.
/// </summary>
internal class Transaction
{
    #region Fields

    /// <summary>
    /// The current number of disposable objects returned by the Start method 
    /// that have not been disposed.
    /// </summary>
    private int _pendingCount = 0;

    #endregion

    #region Properties

    /// <summary>
    /// If there are any undisposed objects returned by the Open method this 
    /// property will be true.
    /// </summary>
    public bool IsOpen
    {
        get;
        private set
        {
            if (field == value)
                return;

            field = value;

            _isOpenChangedSource.RaiseEvent(field);
            if (field)
                _openedSource.RaiseEvent();
            else
                _closedSource.RaiseEvent();
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Every time this method is invoked a new disposable object is created and
    /// returned.
    /// This transaction is considered open until all of the disposable objects 
    /// that have been returned by this method have been disposed.
    /// </summary>
    /// <returns>
    /// A disposable that keeps this transaction open until it and any other 
    /// disposable objects returned by this method are disposed.
    /// </returns>
    public IDisposable Start()
    {
        _pendingCount++;
        IsOpen = true;
        return new ActionOnDispose(Finish);
    }

    /// <summary>
    /// Decrement the open count.
    /// </summary>
    private void Finish()
    {
        _pendingCount--;

        if (_pendingCount < 0)
        {
            throw new InvalidOperationException(
                "The pending count is less than zero."
            );
        }

        if (_pendingCount == 0)
            IsOpen = false;
    }

    #endregion

    #region Events

    /// <summary>
    /// Event that is raised when the IsOpen property changes.
    /// </summary>
    public Observable<bool> IsOpenChanged => _isOpenChangedSource.Observable;
    private readonly ObservableSource<bool> _isOpenChangedSource = new();

    /// <summary>
    /// Event that is raised when the IsOpen property changes from false to 
    /// true.
    /// </summary>
    public Observable Opened => _openedSource.Observable;
    private readonly ObservableSource _openedSource = new();

    /// <summary>
    /// Event that is raised when the IsOpen property changes from true to 
    /// false.
    /// </summary>
    public Observable Closed => _closedSource.Observable;
    private readonly ObservableSource _closedSource = new();

    #endregion
}
