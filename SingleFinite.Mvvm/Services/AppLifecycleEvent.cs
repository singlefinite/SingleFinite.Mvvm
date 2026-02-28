// MIT License
// Copyright (c) 2026 Single Finite
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
/// Possible lifecycle events for an app host.
/// </summary>
public enum AppLifecycleEvent
{
    /// <summary>
    /// The app has been created but might not be visible yet.
    /// </summary>
    Created,

    /// <summary>
    /// The app has become visible and has focus.
    /// </summary>
    Activated,

    /// <summary>
    /// The app no longer has focus but may still be visible.
    /// </summary>
    Deactivated,

    /// <summary>
    /// The app is no longer visible and may be about to be disposed.
    /// </summary>
    Stopped,

    /// <summary>
    /// The app was stopped and has now resumed.
    /// </summary>
    Resumed,

    /// <summary>
    /// The app is disposed.
    /// </summary>
    Disposed
}
