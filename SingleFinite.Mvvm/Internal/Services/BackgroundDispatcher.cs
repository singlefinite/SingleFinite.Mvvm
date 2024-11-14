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

using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.Internal.Services;

/// <summary>
/// Implementation of <see cref="IBackgroundDispatcher"/> service that uses the 
/// <see cref="IAppBackgroundDispatcher"/> to dispatch execution of functions 
/// and actions.  It will use the <see cref="ICancellationTokenProvider"/> to 
/// provide a cancellation token to the functions and actions that can be 
/// cancelled.  This service should be registered as a scoped service so that 
/// each instance of this service created for a dependency injection scope will 
/// be given the <see cref="ICancellationTokenProvider"/> for the same 
/// dependency injection scope.  This will result in the executing functions and
/// actions dispatched through this service being cancelled when the dependency 
/// injection scope this service belongs to is disposed.
/// </summary>
/// <param name="dispatcher">
/// The dispatcher to dispatch functions and actions to.
/// </param>
/// <param name="cancellationTokenProvider">
/// The service that provides the CancellationToken used by this service.
/// </param>
internal sealed class BackgroundDispatcher(
    IAppBackgroundDispatcher dispatcher,
    ICancellationTokenProvider cancellationTokenProvider
) : DispatcherWithCancellationBase<IAppBackgroundDispatcher>(
    dispatcher,
    cancellationTokenProvider
), IBackgroundDispatcher
{
}
