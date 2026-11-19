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

namespace SingleFinite.Mvvm.Internal.Services;

/// <summary>
/// Implementation of <see cref="ILookup{TType}"/> that walks up the view model
/// tree.
/// </summary>
/// <typeparam name="TType">The type to look for.</typeparam>
/// <param name="node">The node for the current scope.</param>
internal class Lookup<TType>(ViewModelNode node) : ILookup<TType>
    where TType : class
{
    #region Methods

    /// <inheritdoc />
    public TType Get()
    {
        if (TryGet(out var value))
            return value;

        throw new InvalidOperationException("Couldn't find value.");
    }

    /// <inheritdoc />
    public bool TryGet([MaybeNullWhen(false)] out TType value)
    {
        var current = node.Parent;
        while (current != null)
        {
            if (current.ViewModel is TType item)
            {
                value = item;
                return true;
            }

            current = current.Parent;
        }

        value = default;
        return false;
    }

    #endregion
}
