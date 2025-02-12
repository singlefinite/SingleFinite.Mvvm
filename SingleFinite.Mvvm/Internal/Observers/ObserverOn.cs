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

namespace SingleFinite.Mvvm.Internal.Observers;

/// <summary>
/// An observer that will observe events until the passed in lifecycle is
/// disposed or the passed in cancellation token is cancelled.
/// </summary>
internal class ObserverOn : ObserverBase
{
    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    /// <param name="lifecycle">
    /// The lifecycle that when disposed will dispose of this observer.
    /// </param>
    public ObserverOn(
        IObserver parent,
        ILifecycle lifecycle
    ) : base(parent)
    {
        if (lifecycle.IsDisposed)
        {
            Dispose();
            return;
        }

        lifecycle.Disposed
            .Observe(Dispose)
            .Once();
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    /// <param name="cancellationToken">
    /// The cancellation token that when cancelled will dispose of this
    /// observer.
    /// </param>
    public ObserverOn(
        IObserver parent,
        CancellationToken cancellationToken
    ) : base(parent)
    {
        CancellationTokenRegistration? registration = null;
        registration = cancellationToken.Register(() =>
        {
            Dispose();
            registration?.Dispose();
        });
    }

    #endregion

    #region Methods

    /// <summary>
    /// This method always returns true.
    /// </summary>
    /// <returns>Always true.</returns>
    protected override bool OnEvent() => true;

    #endregion
}

/// <summary>
/// An observer that will observe events until the passed in lifecycle is
/// disposed or the passed in cancellation token is cancelled.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
internal class ObserverOn<TArgs> : ObserverBase<TArgs>
{
    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    /// <param name="lifecycle">
    /// The lifecycle that when disposed will dispose of this observer.
    /// </param>
    public ObserverOn(
        IObserver<TArgs> parent,
        ILifecycle lifecycle
    ) : base(parent)
    {
        if (lifecycle.IsDisposed)
        {
            Dispose();
            return;
        }

        lifecycle.Disposed
            .Observe(Dispose)
            .Once();
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    /// <param name="cancellationToken">
    /// The cancellation token that when cancelled will dispose of this
    /// observer.
    /// </param>
    public ObserverOn(
        IObserver<TArgs> parent,
        CancellationToken cancellationToken
    ) : base(parent)
    {
        CancellationTokenRegistration? registration = null;
        registration = cancellationToken.Register(() =>
        {
            Dispose();
            registration?.Dispose();
        });
    }

    #endregion

    #region Methods

    /// <summary>
    /// This method always returns true.
    /// </summary>
    /// <param name="args">Not used.</param>
    /// <returns>Always true.</returns>
    protected override bool OnEvent(TArgs args) => true;

    #endregion
}

/// <summary>
/// An observer that will observe events until the passed in lifecycle is
/// disposed or the passed in cancellation token is cancelled.
/// </summary>
/// <typeparam name="TSender">
/// The type of sender passed with observed events.
/// </typeparam>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
internal class ObserverOn<TSender, TArgs> : ObserverBase<TSender, TArgs>
{
    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    /// <param name="lifecycle">
    /// The lifecycle that when disposed will dispose of this observer.
    /// </param>
    public ObserverOn(
        IObserver<TSender, TArgs> parent,
        ILifecycle lifecycle
    ) : base(parent)
    {
        if (lifecycle.IsDisposed)
        {
            Dispose();
            return;
        }

        lifecycle.Disposed
            .Observe(Dispose)
            .Once();
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    /// <param name="cancellationToken">
    /// The cancellation token that when cancelled will dispose of this
    /// observer.
    /// </param>
    public ObserverOn(
        IObserver<TSender, TArgs> parent,
        CancellationToken cancellationToken
    ) : base(parent)
    {
        CancellationTokenRegistration? registration = null;
        registration = cancellationToken.Register(() =>
        {
            Dispose();
            registration?.Dispose();
        });
    }

    #endregion

    #region Methods

    /// <summary>
    /// This method always returns true.
    /// </summary>
    /// <param name="sender">Not used.</param>
    /// <param name="args">Not used.</param>
    /// <returns>Always true.</returns>
    protected override bool OnEvent(TSender sender, TArgs args) => true;

    #endregion
}
