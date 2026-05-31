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

using Microsoft.Extensions.DependencyInjection;
using SingleFinite.Essentials;
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm;

/// <summary>
/// Host for the MVVM services.
/// </summary>
public sealed class AppHost : IDisposable
{
    #region Fields

    /// <summary>
    /// Holds the dispose state for this object.
    /// </summary>
    private readonly DisposeState _disposeState;

    /// <summary>
    /// Holds the initializers to invoke when the app is started.
    /// </summary>
    private readonly IInitializerCollection _initializers;

    /// <summary>
    /// Set to true when the AppHost is started.
    /// </summary>
    private bool _isStarted = false;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="initializers">
    /// Initializers to invoke when the app is started.
    /// </param>
    public AppHost(IInitializerCollection initializers)
    {
        _disposeState = new(owner: this);
        _initializers = initializers;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Start the app host.
    /// </summary>
    /// <param name="provider">The service provider for the app.</param>
    public void Start(IServiceProvider provider)
    {
        _disposeState.ThrowIfDisposed();

        if (_isStarted)
            return;

        _isStarted = true;

        var exceptionHandler = provider.GetService<IExceptionHandler>();

        EventObservable
            .Observe<UnhandledExceptionEventHandler, UnhandledExceptionEventArgs>(
                register: handler => AppDomain.CurrentDomain.UnhandledException += handler,
                unregister: handler => AppDomain.CurrentDomain.UnhandledException -= handler,
                handler: nextEvent => (sender, e) => nextEvent(e)
            )
            .OnEach(e => exceptionHandler?.Handle(e.ExceptionObject, e))
            .Until(_disposeState.CancellationToken);

        Dispatcher.UnhandledException
            .Observe()
            .OnEach(e => exceptionHandler?.Handle(e.Exception, e))
            .Until(_disposeState.CancellationToken);

        foreach (var initializer in _initializers)
            initializer(provider);
    }

    /// <summary>
    /// Dispose of this object.
    /// </summary>
    public void Dispose() => _disposeState.Dispose();

    #endregion
}
