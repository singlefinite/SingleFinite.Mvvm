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
/// Dispatcher that uses a dedicated thread to dispatch actions and functions.
/// </summary>
/// <param name="exceptionHandler">
/// Used to handle exceptions that are thrown when invoking actions passed to
/// the Run method if they wouldn't otherwise be handled the code that invoked
/// the Run method.
/// </param>
internal sealed class MainDispatcher(
    IExceptionHandler exceptionHandler
) :
    IApplicationMainDispatcher,
    IMainDispatcher
{
    #region Fields

    /// <summary>
    /// Dispatcher used to invoke functions.
    /// </summary>
    private readonly DedicatedThreadDispatcher _dispatcher =
        new(onError: exceptionHandler.Handle);

    #endregion

    #region Properties

    /// <inheritdoc/>
    public CancellationToken CancellationToken => _dispatcher.CancellationToken;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public Task<TResult> RunAsync<TResult>(
        Func<Task<TResult>> func,
        CancellationToken cancellationToken = default
    ) =>
        _dispatcher.RunAsync(func, cancellationToken);

    /// <inheritdoc/>
    public void OnError(Exception ex) => _dispatcher.OnError(ex);

    #endregion
}
