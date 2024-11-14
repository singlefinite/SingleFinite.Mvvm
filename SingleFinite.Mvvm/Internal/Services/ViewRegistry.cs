﻿// MIT License
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
/// Implementation of <see cref="IViewRegistry"/>.
/// </summary>
internal sealed class ViewRegistry : IViewRegistry
{
    #region Fields

    /// <summary>
    /// Holds dictionary created from view collection.
    /// </summary>
    private readonly Dictionary<Type, Type> _viewDictionary = [];

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="viewCollection">
    /// The view collection used to build the registry with.
    /// </param>
    public ViewRegistry(IViewCollection viewCollection)
    {
        foreach (var descriptor in viewCollection)
            _viewDictionary[descriptor.ViewModelType] = descriptor.ViewType;
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public Type GetViewType(Type viewModelType) =>
        _viewDictionary[viewModelType];

    #endregion
}
