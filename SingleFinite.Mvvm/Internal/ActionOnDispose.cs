// MIT License
// Copyright (c) 2024 SingleFinite
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

namespace SingleFinite.Mvvm.Internal;

/// <summary>
/// Class used to execute an action when disposed.  This is useful when implementing transaction and register/unregister type patterns.
/// See <see cref="Observable.OpenPropertyChangedTransaction"/> and <see cref="EventToken.Register(Action)"/>.
/// </summary>
/// <param name="onDispose">The action to execute when this class has been disposed.</param>
internal sealed class ActionOnDispose(Action onDispose) : IDisposable
{
    #region Fields

    /// <summary>
    /// Set to true when this class has been disposed.
    /// </summary>
    private bool _isDisposed = false;

    #endregion

    #region Methods

    /// <summary>
    /// Invoke the onDispose action.  If this object has already been disposed the onDispose action will not be invoked again.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;

        onDispose();
    }

    #endregion
}
