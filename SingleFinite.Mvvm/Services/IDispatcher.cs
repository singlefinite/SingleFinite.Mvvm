// MIT License
// Copyright (c) 2024 SingleFinite
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
/// the implementing service.
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// Execute the given async function with result.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>A task that runs until the function has completed.</returns>
    Task<TResult> RunAsync<TResult>(Func<Task<TResult>> func);

    /// <summary>
    /// Execute the given async function without result.
    /// </summary>
    /// <param name="func">The function to execute.</param>
    /// <returns>A task that runs until the function has completed.</returns>
    Task RunAsync(Func<Task> func);

    /// <summary>
    /// Execute the given function with result.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>A task that runs until the function has completed.</returns>
    Task<TResult> RunAsync<TResult>(Func<TResult> func);

    /// <summary>
    /// Execute the given action.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>A task that runs until the action has completed.</returns>
    Task RunAsync(Action action);

    /// <summary>
    /// Execute the given action.
    /// This function will dispatch the action to be executed and return right away
    /// without waiting for the action to complete execution.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    void Run(Action action);
}
