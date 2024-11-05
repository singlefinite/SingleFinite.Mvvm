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

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SingleFinite.Mvvm.UnitTests;

/// <summary>
/// This is a helper class for testing services.
/// </summary>
public sealed class TestContext : IDisposable
{
    #region Fields

    /// <summary>
    /// Set to true after this class has been disposed.
    /// </summary>
    private bool _isDisposed = false;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="configureServices">Optional action used to customize the service configuration.</param>
    /// <param name="configureViews">Optional action used to customize the view configuration.</param>
    /// <param name="configurePlugins">Optional action used to customize the plugin configuration.</param>
    public TestContext(
        Action<IServiceCollection>? configureServices = null,
        Action<IViewCollection>? configureViews = null,
        Action<IPluginCollection>? configurePlugins = null
    )
    {
        var callingAssembly = Assembly.GetCallingAssembly();

        var host = new AppHostBuilder()
            .AddServices(configureServices ?? (_ => { }))
            .AddViews(configureViews ?? (views => views.Scan(callingAssembly)))
            .AddPlugins(configurePlugins ?? (plugins => plugins.Scan(callingAssembly)))
            .BuildAndStart();

        ServiceProvider = host.ServiceProvider;
    }

    #endregion

    #region Properties

    /// <summary>
    /// The service provider built from registered services.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Dispose of the ServiceProvider.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        (ServiceProvider as IDisposable)?.Dispose();

        _isDisposed = true;
    }

    #endregion
}
