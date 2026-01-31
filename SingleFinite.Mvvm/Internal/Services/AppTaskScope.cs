// MIT License
// Copyright (c) 2025 Single Finite
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
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.Internal.Services;

/// <summary>
/// Implementation of the <see cref="IAppTaskScope"/> interface.
/// This task scope is tied to the current service scope and has the
/// BackgroundDispatcher as its default dispatcher.
/// </summary>
/// <remarks>
/// Constructor.
/// </remarks>
/// <param name="cancellationTokenProvider">
/// The cancellation token from this provider will be passed to the task scope
/// so that when it is cancelled the task scope will also be cancelled.
/// </param>
/// <param name="backgroundDispatcher">
/// The background dispatcher will be passed to the task scope and used as the
/// default dispatcher for the task scope.
/// </param>
internal class AppTaskScope(
    ICancellationTokenProvider cancellationTokenProvider,
    IBackgroundDispatcher backgroundDispatcher
) : IAppTaskScope
{
    #region Fields

    /// <summary>
    /// The underlying task scope.
    /// </summary>
    private readonly TaskScope _taskScope = new(
        dispatcher: backgroundDispatcher,
        parentCancellationToken: cancellationTokenProvider.CancellationToken
    );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public IDispatcher Dispatcher => _taskScope.Dispatcher;

    /// <inheritdoc/>
    public CancellationToken CancellationToken => _taskScope.CancellationToken;

    /// <inheritdoc/>
    public bool IsDisposed => _taskScope.IsDisposed;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public TaskScope CreateChildScope(IDispatcher? dispatcher = null) =>
        _taskScope.CreateChildScope(dispatcher);

    /// <inheritdoc/>
    public Task<TResult> RunAsync<TResult>(
        Func<Task<TResult>> function,
        IDispatcher? dispatcher = null,
        CancellationToken cancellationToken = default
    ) => _taskScope.RunAsync(function, dispatcher, cancellationToken);

    /// <inheritdoc/>
    public Task<TResult> RunAsync<TResult>(
        Func<CancellationToken, Task<TResult>> function,
        IDispatcher? dispatcher = null,
        CancellationToken cancellationToken = default
    ) => _taskScope.RunAsync(function, dispatcher, cancellationToken);

    #endregion

    #region Events

    /// <inheritdoc/>
    public Observable Disposed => _taskScope.Disposed;

    #endregion
}
