﻿// MIT License
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

namespace SingleFinite.Mvvm;

/// <summary>
/// The source for an <see cref="Observable"/>.  Instances of this class can be 
/// kept private within its owner while the observable is shared publicly 
/// outside of the owner.  Since events are only raised through this class it 
/// prevents events from being raised outside of the owning class.
/// </summary>
public sealed class ObservableSource
{
    #region Properties

    /// <summary>
    /// The observable provided by this class.
    /// </summary>
    public Observable Observable { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public ObservableSource()
    {
        Observable = new(this);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Raise the event for the observable.
    /// </summary>
    public void RaiseEvent() => Event?.Invoke();

    #endregion

    #region Events

    /// <summary>
    /// Event that is raised when the RaiseEvent method is invoked.
    /// </summary>
    internal event Action? Event;

    #endregion
}

/// <summary>
/// The source for an <see cref="Observable"/>.  Instances of this class can be 
/// kept private within its owner while the observable is shared publicly 
/// outside of the owner.  Since events are only raised through this class it 
/// prevents events from being raised outside of the owning class.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with the event.
/// </typeparam>
public sealed class ObservableSource<TArgs>
{
    #region Properties

    /// <summary>
    /// The observable provided by this class.
    /// </summary>
    public Observable<TArgs> Observable { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public ObservableSource()
    {
        Observable = new(this);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Raise the event for the observable.
    /// </summary>
    /// <param name="args">The arguments to pass with the event.</param>
    public void RaiseEvent(TArgs args) => Event?.Invoke(args);

    #endregion

    #region Events

    /// <summary>
    /// Event that is raised when the RaiseEvent method is invoked.
    /// </summary>
    internal event Action<TArgs>? Event;

    #endregion
}

/// <summary>
/// The source for an <see cref="Observable"/>.  Instances of this class can be 
/// kept private within its owner while the observable is shared publicly 
/// outside of the owner.  Since events are only raised through this class it 
/// prevents events from being raised outside of the owning class.
/// </summary>
/// <typeparam name="TSender">
/// The type of sender that raises the event.
/// </typeparam>
/// <typeparam name="TArgs">
/// The type of arguments passed with the event.
/// </typeparam>
public sealed class ObservableSource<TSender, TArgs>
{
    #region Properties

    /// <summary>
    /// The observable provided by this class.
    /// </summary>
    public Observable<TSender, TArgs> Observable { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public ObservableSource()
    {
        Observable = new(this);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Raise the event for the observable.
    /// </summary>
    /// <param name="sender">The object that is raising the event.</param>
    /// <param name="args">The arguments to pass with the event.</param>
    public void RaiseEvent(TSender sender, TArgs args) =>
        Event?.Invoke(sender, args);

    #endregion

    #region Events

    /// <summary>
    /// Event that is raised when the RaiseEvent method is invoked.
    /// </summary>
    internal event Action<TSender, TArgs>? Event;

    #endregion
}