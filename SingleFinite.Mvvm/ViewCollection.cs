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

using System.Collections;

namespace SingleFinite.Mvvm;

/// <summary>
/// Implementation of <see cref="IViewCollection"/>.
/// </summary>
public class ViewCollection : IViewCollection
{
    #region Fields

    /// <summary>
    /// The underlying list.
    /// </summary>
    private readonly IList<ViewDescriptor> _list = [];

    #endregion

    #region Properties

    /// <inheritdoc/>
    public ViewDescriptor this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    /// <inheritdoc/>
    public int Count => _list.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => _list.IsReadOnly;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public void Add(ViewDescriptor item) => _list.Add(item);

    /// <inheritdoc/>
    public void Clear() => _list.Clear();

    /// <inheritdoc/>
    public bool Contains(ViewDescriptor item) => _list.Contains(item);

    /// <inheritdoc/>
    public void CopyTo(ViewDescriptor[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public IEnumerator<ViewDescriptor> GetEnumerator() => _list.GetEnumerator();

    /// <inheritdoc/>
    public int IndexOf(ViewDescriptor item) => _list.IndexOf(item);

    /// <inheritdoc/>
    public void Insert(int index, ViewDescriptor item) => _list.Insert(index, item);

    /// <inheritdoc/>
    public bool Remove(ViewDescriptor item) => _list.Remove(item);

    /// <inheritdoc/>
    public void RemoveAt(int index) => _list.RemoveAt(index);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

    #endregion
}
