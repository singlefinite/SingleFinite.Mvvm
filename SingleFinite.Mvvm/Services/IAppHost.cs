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
using SingleFinite.Essentials;

namespace SingleFinite.Mvvm.Services;

/// <summary>
/// The top level object that holds the application state.
/// </summary>
public interface IAppHost
{
    /// <summary>
    /// The application service provider.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this app host has not been started yet.
    /// </exception>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Start this app host.
    /// If the app host has already been started this method will have no effect.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if this app host has been disposed.
    /// </exception>
    void Start();

    /// <summary>
    /// Request that the app be closed.  The request can be canceled by
    /// subscribers to the Closing event.
    /// </summary>
    /// <returns>true if the app was stopped, false if it wasn't.</returns>
    Task<bool> CloseAsync();

    /// <summary>
    /// Raised when this app host is started.
    /// </summary>
    Observable Started { get; }

    /// <summary>
    /// Raised when the app is being closed.
    /// </summary>
    AsyncObservable<CancelEventArgs> Closing { get; }

    /// <summary>
    /// Raised after the app has been closed.
    /// </summary>
    Observable Closed { get; }
}
