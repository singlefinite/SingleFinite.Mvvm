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
using SingleFinite.Mvvm.Internal;
using SingleFinite.Mvvm.Internal.Services;
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm;

/// <summary>
/// This classed is used to build a new instance of <see cref="AppHost"/>.
/// </summary>
public class AppHostBuilder
{
    #region Fields

    /// <summary>
    /// Holds the services configured by the builder.
    /// </summary>
    private readonly ServiceCollection _services = [];

    /// <summary>
    /// Holds the views configured by the builder.
    /// </summary>
    private readonly ViewCollection _views = [];

    /// <summary>
    /// Holds the plugins configured by the builder.
    /// </summary>
    private readonly PluginCollection _plugins = [];

    /// <summary>
    /// Holds the initializers configured by the builder.
    /// </summary>
    private readonly InitializerCollection _initializers = [];

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
    /// Add plugins to the host.
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
    /// Add initializers to the host.
    /// </summary>
    /// <param name="configure">
    /// Action called to configure the initializer collection.
    /// </param>
    /// <returns>
    /// A reference to this object so a fluent pattern can be used.
    /// </returns>
    public AppHostBuilder AddInitializers(Action<IInitializerCollection> configure)
    {
        configure(_initializers);
        return this;
    }

    /// <summary>
    /// Create the app host instance.  This can be overridden to create a custom
    /// app host.
    /// </summary>
    /// <param name="initializers">The initializers for the app host.</param>
    /// <returns>The newly created app host.</returns>
    protected virtual AppHost CreateAppHost(IInitializerCollection initializers) =>
        new(initializers);

    /// <summary>
    /// Build a new host using the configurations made through this builder.
    /// </summary>
    /// <returns>A newly built host.</returns>
    public AppHost Build(IServiceCollection services)
    {
        services
            .AddMvvm()
            .AddSingleton<IViewRegistry>(new ViewRegistry(_views.Copy()))
            .AddSingleton<IPluginRegistry>(new PluginRegistry(_plugins.Copy()));

        foreach (var service in _services)
            services.Add(service);

        var appHost = CreateAppHost(initializers: _initializers.Copy());

        services.AddSingleton(appHost);

        return appHost;
    }

    #endregion
}
