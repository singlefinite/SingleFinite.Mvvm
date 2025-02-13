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
/// Base class for classes that implement <see cref="IDispatcher"/> which will 
/// route all method calls to a single abstract method that implementing classes
/// can override with their custom dispatch logic.
/// </summary>
/// <param name="cancellationTokenProvider">
/// The service that provides the CancellationToken used by this service.
/// </param>
/// <param name="exceptionHandler">
/// Used to handle exceptions that are thrown when invoking actions passed to
/// the Run method.
/// </param>
public abstract class DispatcherBase(
    ICancellationTokenProvider cancellationTokenProvider,
    IExceptionHandler exceptionHandler
) : IDispatcher
{
    #region Methods

    /// <summary>
    /// Classes that extend this class only need to override this method with 
    /// their custom dispatch logic.  All other dispatcher
    /// methods will be routed to this method.
    /// </summary>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>A task that runs until the function has completed.</returns>
    public abstract Task<TResult> RunAsync<TResult>(Func<Task<TResult>> func);

    /// <inheritdoc/>
    public Task RunAsync(Func<Task> func) =>
        RunAsync(
            async () =>
            {
                await func();
                return 0;
            }
        );

    /// <inheritdoc/>
    public Task<TResult> RunAsync<TResult>(Func<TResult> func) =>
        RunAsync(
            () => Task.FromResult(func())
        );

    /// <inheritdoc/>
    public Task RunAsync(Action action) =>
        RunAsync(
            () =>
            {
                action();
                return Task.FromResult(0);
            }
        );

    /// <inheritdoc/>
    public void Run(Action action, Action<ExceptionEventArgs>? onError = default)
    {
        _ = RunAsync(
            () =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    var args = new ExceptionEventArgs(ex);
                    onError?.Invoke(args);
                    if (!args.IsHandled)
                        HandleError(ex);
                }

                return Task.FromResult(0);
            }
        );
    }

    /// <inheritdoc/>
    public void Run(Func<Task> func, Action<ExceptionEventArgs>? onError = default)
    {
        _ = RunAsync(
            async () =>
            {
                try
                {
                    await func();
                }
                catch (Exception ex)
                {
                    var args = new ExceptionEventArgs(ex);
                    onError?.Invoke(args);
                    if (!args.IsHandled)
                        HandleError(ex);
                }

                return Task.FromResult(0);
            }
        );
    }

    /// <inheritdoc/>
    public async Task<TResult> RunAsync<TResult>(
        Func<CancellationToken, Task<TResult>> func,
        params CancellationToken[] cancellationTokens
    )
    {
        if (cancellationTokens.Length == 0)
        {
            return await RunAsync(
                func: () => func(cancellationTokenProvider.CancellationToken)
            );
        }

        var tokenList = new List<CancellationToken>
        {
            cancellationTokenProvider.CancellationToken
        };
        tokenList.AddRange(cancellationTokens);

        using var linkedCancellationTokenSource =
            CancellationTokenSource.CreateLinkedTokenSource([.. tokenList]);

        return await RunAsync(
            func: () => func(linkedCancellationTokenSource.Token)
        ); ;
    }

    /// <inheritdoc/>
    public Task RunAsync(
        Func<CancellationToken, Task> func,
        params CancellationToken[] cancellationTokens
    ) =>
        RunAsync(
            func: async cancellationToken =>
            {
                await func(cancellationToken);
                return 0;
            },
            cancellationTokens: cancellationTokens
        );

    /// <inheritdoc/>
    public Task<TResult> RunAsync<TResult>(
        Func<CancellationToken, TResult> func,
        params CancellationToken[] cancellationTokens
    ) =>
        RunAsync(
            func: cancellationToken => Task.FromResult(func(cancellationToken)),
            cancellationTokens: cancellationTokens
        );

    /// <inheritdoc/>
    public Task RunAsync(
        Action<CancellationToken> action,
        params CancellationToken[] cancellationTokens
    ) =>
        RunAsync(
            func: cancellationToken =>
            {
                action(cancellationToken);
                return Task.FromResult(0);
            },
            cancellationTokens: cancellationTokens
        );

    /// <inheritdoc/>
    public void Run(
        Action<CancellationToken> action,
        params CancellationToken[] cancellationTokens
    ) => Run(
        action,
        onError: null,
        cancellationTokens
    );

    /// <inheritdoc/>
    public void Run(
        Action<CancellationToken> action,
        Action<ExceptionEventArgs>? onError,
        params CancellationToken[] cancellationTokens
    )
    {
        _ = RunAsync(
            func: cancellationToken =>
            {
                try
                {
                    action(cancellationToken);
                }
                catch (Exception ex)
                {
                    var args = new ExceptionEventArgs(ex);
                    onError?.Invoke(args);
                    if (!args.IsHandled)
                        HandleError(ex);
                }

                return Task.CompletedTask;
            },
            cancellationTokens: cancellationTokens
        );
    }

    /// <inheritdoc/>
    public void Run(
        Func<CancellationToken, Task> func,
        params CancellationToken[] cancellationTokens
    ) => Run(
        func,
        onError: null,
        cancellationTokens
    );

    /// <inheritdoc/>
    public void Run(
        Func<CancellationToken, Task> func,
        Action<ExceptionEventArgs>? onError,
        params CancellationToken[] cancellationTokens
    )
    {
        _ = RunAsync(
            func: async cancellationToken =>
            {
                try
                {
                    await func(cancellationToken);
                }
                catch (Exception ex)
                {
                    var args = new ExceptionEventArgs(ex);
                    onError?.Invoke(args);
                    if (!args.IsHandled)
                        HandleError(ex);
                }

                return Task.CompletedTask;
            },
            cancellationTokens: cancellationTokens
        );
    }

    /// <summary>
    /// Handles the given exception.
    /// </summary>
    /// <param name="ex">The exception to handle.</param>
    protected void HandleError(Exception ex) => exceptionHandler.Handle(ex);

    #endregion
}
