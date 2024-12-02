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

namespace SingleFinite.Mvvm;

/// <summary>
/// An object that can be observed moving through different states in a
/// lifecycle.
/// </summary>
public interface ILifecycleObservable
{
    /// <summary>
    /// Event raised when this object has been initialized.
    /// </summary>
    Observable Initialized { get; }

    /// <summary>
    /// Event raised when this object has been activated.
    /// </summary>
    Observable Activated { get; }

    /// <summary>
    /// Event raised when this object has been deactivated.
    /// </summary>
    Observable Deactivated { get; }

    /// <summary>
    /// Event raised when this object has been disposed.
    /// </summary>
    Observable Disposed { get; }
}
