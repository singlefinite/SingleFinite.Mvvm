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

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class PluginLoaderTests
{
    [TestMethod]
    public void LoadPlugins_Method_Throws_If_PluginHost_Is_Plugin()
    {
        var services = new ServiceCollection();
        var pluginLoader = new PluginLoader(
            pluginRegistry: new PluginRegistry(new PluginCollection()),
            serviceProvider: services.BuildServiceProvider()
        );

        var pluginHost = new BadPluginHost();

        Assert.ThrowsExactly<ArgumentException>(() => pluginLoader.LoadPlugins(pluginHost));
    }

    [TestMethod]
    public void LoadPlugins_Method_Builds_And_Wires_Up_Registered_Plugins()
    {
        using var testContext = new TestContext();
        var pluginCollection = new PluginCollection
        {
            new PluginDescriptor(
                PluginType: typeof(ExamplePlugin),
                PluginHostType: typeof(ExamplePluginHost)
            )
        };
        var pluginRegistry = new PluginRegistry(pluginCollection);

        var pluginLoader = new PluginLoader(
            pluginRegistry: pluginRegistry,
            serviceProvider: testContext.ServiceProvider
        );

        var pluginHost = new ExamplePluginHost();
        var hostViewModel = (IViewModel)pluginHost;

        pluginLoader.LoadPlugins(pluginHost);

        var plugin = pluginHost.Plugin;

        Assert.IsNotNull(plugin);
        Assert.AreEqual(pluginHost, plugin.Host);

        Assert.AreEqual(1, plugin.OnInitializeCount);
        Assert.AreEqual(0, plugin.OnActivateCount);
        Assert.AreEqual(0, plugin.OnDeactivateCount);
        Assert.AreEqual(0, plugin.OnDisposeCount);

        hostViewModel.Activate();

        Assert.AreEqual(1, plugin.OnInitializeCount);
        Assert.AreEqual(1, plugin.OnActivateCount);
        Assert.AreEqual(0, plugin.OnDeactivateCount);
        Assert.AreEqual(0, plugin.OnDisposeCount);

        hostViewModel.Deactivate();

        Assert.AreEqual(1, plugin.OnInitializeCount);
        Assert.AreEqual(1, plugin.OnActivateCount);
        Assert.AreEqual(1, plugin.OnDeactivateCount);
        Assert.AreEqual(0, plugin.OnDisposeCount);

        hostViewModel.Dispose();

        Assert.AreEqual(1, plugin.OnInitializeCount);
        Assert.AreEqual(1, plugin.OnActivateCount);
        Assert.AreEqual(1, plugin.OnDeactivateCount);
        Assert.AreEqual(1, plugin.OnDisposeCount);
    }

    [TestMethod]
    public void LoadPlugins_Method_Does_Not_Break_If_No_Registered_Plugins()
    {
        using var testContext = new TestContext();
        var pluginCollection = new PluginCollection();
        var pluginRegistry = new PluginRegistry(pluginCollection);

        var pluginLoader = new PluginLoader(
            pluginRegistry: pluginRegistry,
            serviceProvider: testContext.ServiceProvider
        );

        var pluginHost = new ExamplePluginHost();

        pluginLoader.LoadPlugins(pluginHost);

        Assert.IsNull(pluginHost.Plugin);
    }

    #region Types

    private class BadPluginHost : ViewModel, IPluginHost, IPlugin
    {
        public void Load(IPluginHost pluginHost)
        {
        }
    }

    private class ExamplePluginHost : ViewModel, IPluginHost
    {
        public ExamplePlugin? Plugin { get; set; }
    }

    private class ExamplePlugin : Plugin<ExamplePluginHost>
    {
        public ExamplePluginHost? Host { get; private set; }

        public int OnInitializeCount { get; private set; }
        public int OnActivateCount { get; private set; }
        public int OnDeactivateCount { get; private set; }
        public int OnDisposeCount { get; private set; }

        protected override void OnInitialize()
        {
            Host = PluginHost;
            PluginHost.Plugin = this;
            OnInitializeCount++;
        }
        protected override void OnActivate(CancellationToken cancellationToken) => OnActivateCount++;
        protected override void OnDeactivate() => OnDeactivateCount++;
        protected override void OnDispose() => OnDisposeCount++;
    }

    #endregion
}
