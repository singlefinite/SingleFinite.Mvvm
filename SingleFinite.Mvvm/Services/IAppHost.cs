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
    /// Start this app host.  This builds a new ServiceProvider.
    /// If the app host has already been started this method will have no effect.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if this app host has been disposed.
    /// </exception>
    void Start();

    /// <summary>
    /// Restart this app host.  This will dispose of the current service 
    /// provider and build a new one.  If this app host hasn't been started yet 
    /// this function will behave the same as if Start was called.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if this app host has been disposed.
    /// </exception>
    void Restart();

    /// <summary>
    /// Raised when this app host is started or restarted.
    /// </summary>
    EventToken<StartedEventArgs> Started { get; }

    /// <summary>
    /// Event arguments for the IAppHost.Started event.
    /// </summary>
    /// <param name="isRestart">Value for the IsRestart property.</param>
    public class StartedEventArgs(bool isRestart) : EventArgs
    {
        #region Properties

        /// <summary>
        /// If the app was started for the first time this returns false.
        /// If the app has was restarted this returns true.
        /// </summary>
        public bool IsRestart { get; } = isRestart;

        #endregion
    }
}
