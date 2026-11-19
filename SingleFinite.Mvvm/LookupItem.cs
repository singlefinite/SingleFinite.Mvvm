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
/// An implementation of <see cref="ILookup{TType}"/> that provides a given
/// value.  This is useful for unit testing purposes when you want to provide a
/// value through a Lookup service.
/// </summary>
/// <typeparam name="TType">The type of value that will be provided.</typeparam>
public class LookupItem<TType> : ILookup<TType>
    where TType : class
{
    #region Fields

    /// <summary>
    /// Holds the value to provide.
    /// </summary>
    private readonly TType? _value;

    /// <summary>
    /// Indicates if there is a value to return.
    /// </summary>
    private readonly bool _hasValue;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor for when there isn't a value to provide.
    /// </summary>
    internal LookupItem()
    {
        _value = default;
        _hasValue = false;
    }

    /// <summary>
    /// Constructor for when there is a value to provide.
    /// </summary>
    /// <param name="value">The value to provide.</param>
    internal LookupItem(TType? value)
    {
        _value = value;
        _hasValue = true;
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public TType Get()
    {
        if (TryGet(out var value))
            return value;

        throw new InvalidOperationException("Couldn't find value.");
    }

    /// <inheritdoc/>
    public bool TryGet(
        [MaybeNullWhen(false)]
        out TType value
    )
    {
        value = _value;
        return _hasValue;
    }

    #endregion
}

/// <summary>
/// Static class used to build Lookup objects.
/// </summary>
public static class LookupItem
{
    #region Methods

    /// <summary>
    /// Create a Lookup service that will provide the given value.
    /// </summary>
    /// <typeparam name="TType">The type of value to provide.</typeparam>
    /// <param name="value">The value that will be provided.</param>
    /// <returns>A new Lookup service.</returns>
    public static ILookup<TType> For<TType>(TType? value)
        where TType : class => new LookupItem<TType>(value);

    /// <summary>
    /// Create a Lookup service that doesn't provide a value.
    /// </summary>
    /// <typeparam name="TType">The type of value to provide.</typeparam>
    /// <returns>A new Lookup service.</returns>
    public static ILookup<TType> For<TType>()
        where TType : class => new LookupItem<TType>();

    #endregion
}
