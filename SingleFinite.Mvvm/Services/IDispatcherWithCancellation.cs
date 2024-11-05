// MIT License
// Copyright (c) 2024 Single Finite
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR 
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace SingleFinite.Mvvm.Services;

/// <summary>
/// A service that implements this interface is responsible for executing functions and actions on a specific thread determined by
/// the implementing service.  This service is similar to <see cref="IDispatcher"/> except it provides a <see cref="CancellationToken"/>
/// to the functions and actions being executed which allows the functions and actions to be cancelled.
/// </summary>
public interface IDispatcherWithCancellation
{
    /// <summary>
    /// Execute the given cancellable async function with result.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <param name="cancellationTokens">
    /// Optional cancellation tokens to link with the cancellation token from the dependency injection scope.
    /// </param>
    /// <returns>A task that runs until the function has completed.</returns>
    Task<TResult> RunAsync<TResult>(
        Func<CancellationToken, Task<TResult>> func,
        params CancellationToken[] cancellationTokens
    );

    /// <summary>
    /// Execute the given cancellable async function without result.
    /// </summary>
    /// <param name="func">The function to execute.</param>
    /// <param name="cancellationTokens">
    /// Optional cancellation tokens to link with the cancellation token from the dependency injection scope.
    /// </param>
    /// <returns>A task that runs until the function has completed.</returns>
    Task RunAsync(
        Func<CancellationToken, Task> func,
        params CancellationToken[] cancellationTokens
    );

    /// <summary>
    /// Execute the given cancellable function with result.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <param name="cancellationTokens">
    /// Optional cancellation tokens to link with the cancellation token from the dependency injection scope.
    /// </param>
    /// <returns>A task that runs until the function has completed.</returns>
    Task<TResult> RunAsync<TResult>(
        Func<CancellationToken, TResult> func,
        params CancellationToken[] cancellationTokens
    );

    /// <summary>
    /// Execute the given cancellable action.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="cancellationTokens">
    /// Optional cancellation tokens to link with the cancellation token from the dependency injection scope.
    /// </param>
    /// <returns>A task that runs until the action has completed.</returns>
    Task RunAsync(
        Action<CancellationToken> action,
        params CancellationToken[] cancellationTokens
    );

    /// <summary>
    /// Execute the given cancellable action.
    /// This function will dispatch the action to be executed and return right away
    /// without waiting for the action to complete execution.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="cancellationTokens">
    /// Optional cancellation tokens to link with the cancellation token from the dependency injection scope.
    /// </param>
    void Run(
        Action<CancellationToken> action,
        params CancellationToken[] cancellationTokens
    );
}
