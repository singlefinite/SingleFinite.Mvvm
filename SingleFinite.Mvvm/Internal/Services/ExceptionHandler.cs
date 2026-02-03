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

using System.Diagnostics;
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.Internal.Services;

/// <summary>
/// Implementation of <see cref="IExceptionHandler"/>.
/// </summary>
internal class ExceptionHandler : IExceptionHandler
{
    #region Methods

    /// <inheritdoc/>
    public void Handle(object? exception, object? sourceArgs)
    {
        var sourceArgsText = string.Empty;
        if (sourceArgs != null)
            sourceArgsText = $", SourceArgs: [{sourceArgs.GetType().FullName}]";

        if (exception is Exception ex)
            Debug.WriteLine($"Exception: [{ex.GetType().FullName}] {ex.Message}{sourceArgsText}");
        else
            Debug.WriteLine($"Exception: [{exception?.GetType()?.FullName}]{sourceArgsText}");
    }

    #endregion
}
