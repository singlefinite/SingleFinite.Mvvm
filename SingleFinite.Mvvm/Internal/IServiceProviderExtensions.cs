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

using SingleFinite.Mvvm.Services;
using Microsoft.Extensions.DependencyInjection;

namespace SingleFinite.Mvvm.Internal;

/// <summary>
/// Extension methods for the <see cref="IServiceProvider"/> class.
/// </summary>
internal static class IServiceProviderExtensions
{
    /// <summary>
    /// Create a new scope that will be linked to the scope of the given service provider.
    /// When the given service provider is disposed it will cause any and all service scopes
    /// created by this method to be disposed as well, cascading down to all linked service scopes
    /// and disposing of them as well.
    /// </summary>
    /// <param name="serviceProvider">The service provider to create a new scope from.</param>
    /// <returns>The newly created service scope.</returns>
    public static IServiceScope CreateLinkedScope(this IServiceProvider serviceProvider)
    {
        var childScope = serviceProvider.CreateScope();
        var childCancellationToken = childScope.ServiceProvider.GetRequiredService<ICancellationTokenProvider>().CancellationToken;
        var parentCancellationToken = serviceProvider.GetRequiredService<ICancellationTokenProvider>().CancellationToken;
        var registration = parentCancellationToken.Register(() => childScope.Dispose());
        childCancellationToken.Register(() => registration.Dispose());
        return childScope;
    }
}
