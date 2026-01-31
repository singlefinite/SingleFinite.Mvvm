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

using SingleFinite.Mvvm.Internal.Services;
using SingleFinite.Mvvm.Services;
using Microsoft.Extensions.DependencyInjection;

namespace SingleFinite.Mvvm;

/// <summary>
/// This classed is used to build a new instance of <see cref="IAppHost"/>.
/// </summary>
public sealed class AppHostBuilder
{
    #region Fields

    /// <summary>
    /// Holds the services configured by the builder.
    /// </summary>
    private readonly IServiceCollection _services = new ServiceCollection();

    /// <summary>
    /// Holds the views configured by the builder.
    /// </summary>
    private readonly IViewCollection _views = new ViewCollection();

    /// <summary>
    /// Holds the plugins configured by the builder.
    /// </summary>
    private readonly IPluginCollection _plugins = new PluginCollection();

    /// <summary>
    /// Holds the on started actions configured by the builder.
    /// </summary>
    private readonly IList<Action<IServiceProvider>> _onStarted = [];

    #endregion

    #region Methods

    /// <summary>
    /// Add services to the host.
    /// </summary>
    /// <param name="configure">
    /// Action called to configure the service collection.
    /// </param>
    /// <returns>
    /// A reference to this object so a fluent pattern can be used.
    /// </returns>
    public AppHostBuilder AddServices(Action<IServiceCollection> configure)
    {
        configure(_services);
        return this;
    }

    /// <summary>
    /// Add views to the host.
    /// </summary>
    /// <param name="configure">
    /// Action called to configure the view collection.
    /// </param>
    /// <returns>
    /// A reference to this object so a fluent pattern can be used.
    /// </returns>
    public AppHostBuilder AddViews(Action<IViewCollection> configure)
    {
        configure(_views);
        return this;
    }

    /// <summary>
    /// Add plugins to host.
    /// </summary>
    /// <param name="configure">
    /// Action called to configure the plugin collection.
    /// </param>
    /// <returns>
    /// A reference to this object so a fluent pattern can be used.
    /// </returns>
    public AppHostBuilder AddPlugins(Action<IPluginCollection> configure)
    {
        configure(_plugins);
        return this;
    }

    /// <summary>
    /// Add action to invoke when the host is started or reset.
    /// </summary>
    /// <param name="onStarted">The action to add to the host.</param>
    /// <returns>
    /// A reference to this object so a fluent pattern can be used.
    /// </returns>
    public AppHostBuilder AddOnStarted(Action onStarted)
    {
        _onStarted.Add(_ => onStarted());
        return this;
    }

    /// <summary>
    /// Add action to invoke when the host is started or reset.
    /// </summary>
    /// <typeparam name="TService">
    /// The type of service to provide to the onStarted action.
    /// </typeparam>
    /// <param name="onStarted">The action to add to the host.</param>
    /// <returns>
    /// A reference to this object so a fluent pattern can be used.
    /// </returns>
    public AppHostBuilder AddOnStarted<TService>(Action<TService> onStarted)
        where TService : notnull
    {
        _onStarted.Add(serviceProvider =>
        {
            var service = serviceProvider.GetRequiredService<TService>();
            onStarted(service);
        });
        return this;
    }

    /// <summary>
    /// Build a new host using the configurations made through this builder.
    /// </summary>
    /// <returns>A newly built host.</returns>
    public IAppHost Build() => new AppHost(
        services: _services,
        views: _views,
        plugins: _plugins,
        onStarted: _onStarted
    );

    /// <summary>
    /// Build a new host using the configurations made through this builder and 
    /// then start it.
    /// </summary>
    /// <returns>A newly built host that has been started.</returns>
    public IAppHost BuildAndStart()
    {
        var host = Build();
        host.Start();
        return host;
    }

    #endregion
}
