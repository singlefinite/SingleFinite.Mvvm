// MIT License
// Copyright (c) 2024 Single Finite
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
using SingleFinite.Mvvm.Internal.Services;
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class AppHostBuilderTests
{
    [TestMethod]
    public void Build_Creates_AppHost_With_Given_Configuration()
    {
        var onStartedCount = 0;

        var builder = new AppHostBuilder();
        builder
            .AddOnStarted(() => onStartedCount++)
            .AddServices(services => services.AddSingleton<ExampleService>())
            .AddPlugins(
                plugins => plugins.Add(
                    new(
                        PluginType: typeof(ExamplePlugin),
                        PluginHostType: typeof(ExamplePluginHost)
                    )
                )
            )
            .AddViews(
                views => views.Add(
                    new(
                        ViewModelType: typeof(ExampleViewModel),
                        ViewType: typeof(ExampleView)
                    )
                )
            );

        Assert.AreEqual(0, onStartedCount);

        var appHost = builder.BuildAndStart();

        Assert.AreEqual(1, onStartedCount);

        var service = appHost.ServiceProvider.GetService<ExampleService>();
        Assert.IsNotNull(service);

        var pluginHost = new ExamplePluginHost();
        var pluginRegistry = appHost.ServiceProvider.GetRequiredService<IPluginRegistry>();
        var plugins = pluginRegistry.GetPlugins(pluginHost);
        Assert.AreEqual(1, plugins.Count);
        Assert.AreEqual(typeof(ExamplePlugin), plugins[0].PluginType);
        Assert.AreEqual(typeof(ExamplePluginHost), plugins[0].PluginHostType);

        var viewRegistry = appHost.ServiceProvider.GetRequiredService<IViewRegistry>();
        var viewType = viewRegistry.GetViewType(typeof(ExampleViewModel));
        Assert.AreEqual(typeof(ExampleView), viewType);
    }

    [TestMethod]
    public void Build_Overwrites_Services_From_Mvvm()
    {
        var builder = new AppHostBuilder();
        var appHost = builder
            .AddServices(services => services.AddSingleton<IAppMainDispatcher, ExampleAppMainDispatcher>())
            .BuildAndStart();

        var dispatcher = appHost.ServiceProvider.GetRequiredService<IAppMainDispatcher>();
        Assert.IsInstanceOfType<ExampleAppMainDispatcher>(dispatcher);
    }

    #region Types

    private class ExampleService
    {
    }

    private class ExamplePluginHost : ViewModel, IPluginHost
    {
    }

    private class ExamplePlugin : Plugin<ExamplePluginHost>
    {
    }

    private class ExampleViewModel : ViewModel
    {
    }

    private class ExampleView : IView<ExampleViewModel>
    {
        public ExampleViewModel ViewModel => throw new NotImplementedException();
    }

    private class ExampleAppMainDispatcher :
        DispatcherBase,
        IAppMainDispatcher
    {
        public ExampleAppMainDispatcher() : base(new ExceptionHandler())
        {
        }

        public override Task<TResult> RunAsync<TResult>(Func<Task<TResult>> func) =>
            throw new NotImplementedException();
    }

    #endregion
}
