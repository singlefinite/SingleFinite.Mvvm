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

using System.Runtime.CompilerServices;

namespace SingleFinite.Mvvm.Internal;

/// <summary>
/// Extensions methods for object types.
/// </summary>
internal static class ObjectExtensions
{
    #region Methods

    /// <summary>
    /// Throw an exception if the given item is null, otherwise return the item
    /// ensuring it's not null.
    /// </summary>
    /// <typeparam name="TType">The type of object required.</typeparam>
    /// <param name="item">The item to check.</param>
    /// <param name="name">
    /// The name used in the exception messsage if the item is null.
    /// Leave unset to use the argument expression passed into this method.
    /// </param>
    /// <returns>The given item if it's not null.</returns>
    /// <exception cref="NullReferenceException">
    /// Thrown if the given item is null.
    /// </exception>
    public static TType Require<TType>(
        this TType? item,
        [CallerArgumentExpression(nameof(item))] string? name = default
    ) =>
        item ?? throw new NullReferenceException($"{name} is null.");

    #endregion
}
