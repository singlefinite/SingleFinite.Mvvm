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

using SingleFinite.Mvvm.Internal.Services;

using SingleFinite.Mvvm.Services;

/// <summary>
/// Implementation of <see cref="IDispatcherMain"/> service that ties the 
/// <see cref="IAppDispatcherMain"/> to the <see cref="CancellationToken"/> for 
/// the dependency injection scope this service belongs to.
/// </summary>
/// <param name="dispatcher">
/// The dispatcher to dispatch functions and actions to.
/// </param>
/// <param name="cancellationTokenProvider">
/// The service that provides the CancellationToken used by this service.
/// </param>
/// <param name="exceptionHandler">
/// Used to handle exceptions that are thrown when invoking actions passed to
/// the Run method.
/// </param>
internal sealed class DispatcherMain(
    IAppDispatcherMain dispatcher,
    ICancellationTokenProvider cancellationTokenProvider,
    IExceptionHandler exceptionHandler
) : DispatcherBase(
    cancellationTokenProvider,
    exceptionHandler
), IDispatcherMain
{
    #region Methods

    /// <inheritdoc/>
    public override Task<TResult> RunAsync<TResult>(Func<Task<TResult>> func) =>
        dispatcher.RunAsync(func);

    #endregion
}
