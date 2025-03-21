// MIT License
// Copyright (c) 2025 Single Finite
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

namespace SingleFinite.Mvvm.Internal;

/// <summary>
/// This class is used to buffer actions to be invoked in the future.
/// The buffered actions are stored with a key and invoked in the order they 
/// were added.
/// </summary>
internal class ActionBuffer<TKey> where TKey : notnull
{
    #region Fields

    /// <summary>
    /// Holds actions that will be invoked when the buffer is flushed.
    /// </summary>
    private readonly Dictionary<TKey, Entry> _buffer = [];

    /// <summary>
    /// Used to keep track of the order in which actions are added to the 
    /// buffer.
    /// </summary>
    private int _bufferIndex = 0;

    #endregion

    #region Properties

    /// <summary>
    /// The keys of the actions currently held in the buffer.
    /// </summary>
    public TKey[] Keys => [.. _buffer.Keys];

    #endregion

    #region Methods

    /// <summary>
    /// Add an action to be invoked in the future.  If an action with the given 
    /// key already exists in the buffer it will be replaced with the new action
    /// but will still be invoked in the same order as the action it replaces.
    /// </summary>
    /// <param name="key">The key for the action.</param>
    /// <param name="action">The action to invoke.</param>
    /// <returns>
    /// true if the action was added to the buffer or false if the action 
    /// replaces an already existing action in the buffer with the same key.
    /// </returns>
    public bool AddOrReplace(TKey key, Action action)
    {
        if (_buffer.TryGetValue(key, out var existingEntry))
        {
            _buffer[key] = new(
                Key: key,
                Action: action,
                Index: existingEntry.Index
            );

            return false;
        }
        else
        {
            _buffer.Add(
                key: key,
                value: new(
                    Key: key,
                    Action: action,
                    Index: _bufferIndex++
                )
            );

            return true;
        }
    }

    /// <summary>
    /// Invoke all of the buffered actions and clear the buffer.
    /// </summary>
    public void Flush()
    {
        var entries = _buffer
            .Values
            .OrderBy(entry => entry.Index)
            .ToList();

        _buffer.Clear();

        entries.ForEach(entry => entry.Action());
    }

    #endregion

    #region Records

    /// <summary>
    /// Holds the action to invoke when the buffer is flushed.
    /// </summary>
    /// <param name="Key">The key for the action.</param>
    /// <param name="Action">The action to invoke.</param>
    /// <param name="Index">
    /// The index that determines the order to invoke the Action.
    /// </param>
    private record Entry(
        TKey Key,
        Action Action,
        int Index
    );

    #endregion
}
