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

using System.Diagnostics.CodeAnalysis;

namespace SingleFinite.Mvvm;

/// <summary>
/// Lookup a value of the given type.  The default implementation of this
/// service is to walk up the view model tree and return the first view model
/// that can be assigned from the given type.
/// </summary>
/// <typeparam name="TType">The type of value to look for.</typeparam>
public interface ILookup<TType>
    where TType : class
{
    /// <summary>
    /// Lookup the value.  If not found an exception will be thrown.
    /// </summary>
    /// <returns>The value.</returns>
    TType Get();

    /// <summary>
    /// Lookup the value.
    /// </summary>
    /// <param name="value">The value or null if not found.</param>
    /// <returns>
    /// true if the value was found, false if it wasn't.
    /// </returns>
    bool TryGet(
        [MaybeNullWhen(false)]
        out TType value
    );
}
