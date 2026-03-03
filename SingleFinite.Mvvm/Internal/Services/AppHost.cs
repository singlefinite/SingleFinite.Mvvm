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

namespace SingleFinite.Mvvm.Internal.Services;

/// <summary>
/// Implementation of <see cref="IAppHost"/>.
/// </summary>
internal sealed class AppHost : IAppHost
{
    #region Fields

    /// <summary>
    /// Holds the dispose state for this object.
    /// </summary>
    private readonly DisposeState _disposeState;

    /// <summary>
    /// Holds the service collection for the app.
    /// </summary>
    private readonly IServiceCollection _services = new ServiceCollection();

    /// <summary>
    /// Holds the actions to invoke when the app is started.
    /// </summary>
    private readonly IList<Action<IServiceProvider>> _onStarted;

    /// <summary>
    /// Holds the optional platform for this app.
    /// </summary>
    private IPlatform? _platform;

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
        _disposeState = new(
            owner: this,
            onDispose: () => (_serviceProvider as IDisposable)?.Dispose()
        );

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
        _disposeState.ThrowIfDisposed();

        if (_serviceProvider is not null)
            return;

        _serviceProvider = _services.BuildServiceProvider();

        var exceptionHandler = _serviceProvider.GetService<IExceptionHandler>();

        EventObservable
            .Observe<UnhandledExceptionEventHandler, UnhandledExceptionEventArgs>(
                register: handler => AppDomain.CurrentDomain.UnhandledException += handler,
                unregister: handler => AppDomain.CurrentDomain.UnhandledException -= handler,
                handler: nextEvent => (object sender, UnhandledExceptionEventArgs e) => nextEvent(e)
            )
            .OnEach(e => exceptionHandler?.Handle(e.ExceptionObject, e))
            .Until(_disposeState.CancellationToken);

        Dispatcher.UnhandledException
            .Observe()
            .OnEach(e => exceptionHandler?.Handle(e.Exception, e))
            .Until(_disposeState.CancellationToken);

        foreach (var onStarted in _onStarted)
            onStarted(_serviceProvider);

        _platform = _serviceProvider.GetService<IPlatform>();
        if (_platform is null)
        {
            _lifecycleEvent.Emit(AppLifecycleEvent.Created);
            _lifecycleEvent.Emit(AppLifecycleEvent.Activated);
        }
        else
        {
            _platform.LifecycleEvent
                .Observe()
                .OnEach(e => _lifecycleEvent.Emit(e))
                .Until(_disposeState.CancellationToken);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposeState.IsDisposed)
            return;

        if (_platform is null)
        {
            _lifecycleEvent.Emit(AppLifecycleEvent.Deactivated);
            _lifecycleEvent.Emit(AppLifecycleEvent.Stopped);
            _lifecycleEvent.Emit(AppLifecycleEvent.Disposed);
        }

        _disposeState.Dispose();
    }

    #endregion

    #region Events

    /// <inheritdoc/>
    public IEventObservable<AppLifecycleEvent> LifecycleEvent => _lifecycleEvent.Observable;
    private readonly EventObservableSource<AppLifecycleEvent> _lifecycleEvent = new();

    #endregion
}
