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
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.Internal.Services;

/// <summary>
/// Implementation of the <see cref="IAppTaskScope"/> interface.
/// </summary>
/// <remarks>
/// Constructor.
/// </remarks>
/// <param name="scopeContext">
/// The cancellation token from this context will be passed to the task scope
/// so that when it is cancelled the task scope will also be cancelled.
/// </param>
internal class AppTaskScope(
    IScopeContext scopeContext
) : IAppTaskScope
{
    #region Fields

    /// <summary>
    /// The underlying task scope.
    /// </summary>
    private readonly TaskScope _taskScope = new(
        parentCancellationToken: scopeContext.CancellationToken
    );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public ITaskDispatcher Dispatcher => _taskScope.Dispatcher;

    /// <inheritdoc/>
    public CancellationToken CancellationToken => _taskScope.CancellationToken;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public void Cancel() => _taskScope.Cancel();

    /// <inheritdoc/>
    public TaskScope CreateChildScope(ITaskDispatcher? dispatcher = null) =>
        _taskScope.CreateChildScope(dispatcher);

    /// <inheritdoc/>
    public ITaskJob<TResult> Run<TResult>(
        Func<Task<TResult>> function,
        ITaskDispatcher? dispatcher = null
    ) =>
        _taskScope.Run(function, dispatcher);

    #endregion
}
