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

using SingleFinite.Essentials;
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.Internal.Services;

/// <summary>
/// Implementation of <see cref="ICancellationTokenProvider"/>.
/// </summary>
internal sealed class CancellationTokenProvider :
    ICancellationTokenProvider,
    IDisposeObservable
{
    #region Fields

    /// <summary>
    /// Holds the source for the CancellationToken.
    /// </summary>
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public CancellationTokenProvider()
    {
        CancellationToken = _cancellationTokenSource.Token;
        _disposeState = new(
            owner: this,
            onDispose: OnDispose
        );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    DisposeState IDisposeObservable.DisposeState => _disposeState;
    private readonly DisposeState _disposeState;

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Cancel the CancellationToken when this service is disposed.
    /// </summary>
    private void OnDispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }

    #endregion
}
