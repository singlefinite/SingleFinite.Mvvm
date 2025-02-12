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

using SingleFinite.Mvvm.Internal.Observers;

namespace SingleFinite.Mvvm;

/// <summary>
/// Interface for an observer.  Observers are used to handle events raised by
/// observables.  Observers can be chanined together using methods found in the
/// <see cref="IAsyncObserverExtensions"/> class.  When an observer is disposed
/// it will unregister all observers in the chain so any future observable
/// events will not be observed.
/// </summary>
public interface IAsyncObserver : IDisposable
{
    /// <summary>
    /// An event that is raised when the next observer(s) in the chain should
    /// handle the observed event.
    /// </summary>
    event Func<Task> Next;
}

/// <summary>
/// Interface for an observer.  Observers are used to handle events raised by
/// observables.  Observers can be chanined together using methods found in the
/// <see cref="IAsyncObserverExtensions"/> class.  When an observer is disposed it
/// will unregister all observers in the chain so any future observable events
/// will not be observed.
/// </summary>
/// <typeparam name="TArgs">
/// The type of event arguments passed with observed events.
/// </typeparam>
public interface IAsyncObserver<TArgs> : IDisposable
{
    /// <summary>
    /// Create an observer that will filter out events that don't have arguments
    /// of the given type.
    /// </summary>
    /// <typeparam name="TArgsOut">
    /// The type of arguments to filter out.
    /// </typeparam>
    /// <returns>
    /// A new observer that filters out events based on arguments type.
    /// </returns>
    public IAsyncObserver<TArgsOut> OfType<TArgsOut>() =>
        new AsyncObserverOfType<TArgs, TArgsOut>(this);

    /// <summary>
    /// An event that is raised when the next observer(s) in the chain should
    /// handle the observed event.
    /// </summary>
    event Func<TArgs, Task> Next;
}
