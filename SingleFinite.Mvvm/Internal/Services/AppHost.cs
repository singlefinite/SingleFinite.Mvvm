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

using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using SingleFinite.Essentials;
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.Internal.Services;

/// <summary>
/// Implementation of <see cref="IAppHost"/>.
/// </summary>
internal sealed class AppHost : IAppHost, IDisposable
{
    #region Fields

    /// <summary>
    /// Set to true when this object is disposed.
    /// </summary>
    private bool _isDisposed = false;

    /// <summary>
    /// Holds the service collection for the app.
    /// </summary>
    private readonly IServiceCollection _services = new ServiceCollection();

    /// <summary>
    /// Holds the actions to invoke when the app is started.
    /// </summary>
    private readonly IList<Action<IServiceProvider>> _onStarted;

    /// <summary>
    /// Holds the service provider for the app.
    /// </summary>
    private IServiceProvider? _serviceProvider = default;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="services">
    /// The collection of services to include with the app.  Any services
    /// provided here will overwrite any MVVM services if they have the same
    /// interface.
    /// </param>
    /// <param name="views">The views collection for the app.</param>
    /// <param name="plugins">The plugins collection for the app.</param>
    /// <param name="onStarted">
    /// Actions to invoke when the app is started.
    /// </param>
    public AppHost(
        IServiceCollection services,
        IViewCollection views,
        IPluginCollection plugins,
        IList<Action<IServiceProvider>> onStarted
    )
    {
        _onStarted = onStarted;

        _services.AddMvvm(
            host: this,
            views: views,
            plugins: plugins
        );

        foreach (var service in services)
            _services.Add(service);
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public IServiceProvider ServiceProvider => _serviceProvider ??
        throw new InvalidOperationException("The host has not been started.");

    #endregion

    #region Methods

    /// <inheritdoc/>
    public void Start()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (_serviceProvider is not null)
            return;

        _serviceProvider = _services.BuildServiceProvider();

        foreach (var onStarted in _onStarted)
            onStarted(_serviceProvider);

        _startedSource.RaiseEvent();
    }

    /// <inheritdoc/>
    public async Task<bool> CloseAsync()
    {
        if (_isDisposed)
            return true;

        var cancelEventArgs = new CancelEventArgs();
        await _closingSource.RaiseEventAsync(cancelEventArgs);

        if (!cancelEventArgs.Cancel)
            _closedSource.RaiseEvent();

        return !cancelEventArgs.Cancel;
    }

    /// <summary>
    /// Dispose of this object.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        (_serviceProvider as IDisposable)?.Dispose();
        _isDisposed = true;
    }

    #endregion

    #region Events

    /// <inheritdoc/>
    public Observable Started => _startedSource.Observable;
    private readonly ObservableSource _startedSource = new();

    /// <inheritdoc/>
    public AsyncObservable<CancelEventArgs> Closing => _closingSource.Observable;
    private readonly AsyncObservableSource<CancelEventArgs> _closingSource = new();

    /// <inheritdoc/>
    public Observable Closed => _closedSource.Observable;
    private readonly ObservableSource _closedSource = new();

    #endregion
}
