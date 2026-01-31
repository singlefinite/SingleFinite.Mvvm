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

using SingleFinite.Essentials;

namespace SingleFinite.Mvvm;

/// <summary>
/// Extensions methods for the IObserver interface.
/// </summary>
public static class IObserverExtensions
{
    /// <summary>
    /// Dispose of the observer chain when the given lifecycle is disposed.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="lifecycle">
    /// The lifecylce that when disposed will disposed of the observer.
    /// </param>
    /// <returns>
    /// The passed in observer.
    /// </returns>
    public static IObserver On(
        this IObserver observer,
        ILifecycleObservable lifecycle
    )
    {
        lifecycle.Disposed
            .Observe()
            .OnEach(observer.Dispose)
            .Once();

        return observer;
    }

    /// <summary>
    /// Dispose of the observer chain when the given lifecycle is disposed.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="lifecycle">
    /// The lifecylce that when disposed will disposed of the observer.
    /// </param>
    /// <returns>
    /// The passed in observer.
    /// </returns>
    public static Essentials.IObserver<TArgs> On<TArgs>(
        this Essentials.IObserver<TArgs> observer,
        ILifecycleObservable lifecycle
    )
    {
        lifecycle.Disposed
            .Observe()
            .OnEach(observer.Dispose)
            .Once();

        return observer;
    }
}
