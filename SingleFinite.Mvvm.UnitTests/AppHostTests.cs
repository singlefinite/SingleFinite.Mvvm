// MIT License
// Copyright (c) 2024 Single Finite
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

using Microsoft.Extensions.DependencyInjection;
using SingleFinite.Mvvm.Internal.Services;
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class AppHostTests
{
    [TestMethod]
    public void ServiceProvider_Property_Throws_Until_Started()
    {
        var appHost = new AppHost(
            services: new ServiceCollection(),
            views: new ViewCollection(),
            plugins: new PluginCollection(),
            onStarted: []
        );

        Assert.ThrowsException<InvalidOperationException>(() => appHost.ServiceProvider);

        appHost.Start();

        Assert.IsNotNull(appHost.ServiceProvider);
    }

    [TestMethod]
    public void Start_And_Restart_Methods_Invoke_OnStarted_Actions()
    {
        var onStarted1Count = 0;
        var onStarted2Count = 0;

        var appHost = new AppHost(
            services: new ServiceCollection(),
            views: new ViewCollection(),
            plugins: new PluginCollection(),
            onStarted: [
                _ => onStarted1Count++,
                _ => onStarted2Count++
            ]
        );

        Assert.AreEqual(0, onStarted1Count);
        Assert.AreEqual(0, onStarted2Count);

        appHost.Start();

        Assert.AreEqual(1, onStarted1Count);
        Assert.AreEqual(1, onStarted2Count);

        appHost.Restart();

        Assert.AreEqual(2, onStarted1Count);
        Assert.AreEqual(2, onStarted2Count);
    }

    [TestMethod]
    public void Started_Event_Raised_When_Start_Method_Invoked()
    {
        var onStartCount = 0;

        var appHost = new AppHost(
            services: new ServiceCollection(),
            views: new ViewCollection(),
            plugins: new PluginCollection(),
            onStarted: []
        );

        appHost.Started.Register(() => onStartCount++);

        Assert.AreEqual(0, onStartCount);

        appHost.Start();

        Assert.AreEqual(1, onStartCount);

        appHost.Restart();

        Assert.AreEqual(1, onStartCount);
    }

    [TestMethod]
    public void Restarted_Event_Raised_When_Restart_Method_Invoked()
    {
        var onRestartCount = 0;

        var appHost = new AppHost(
            services: new ServiceCollection(),
            views: new ViewCollection(),
            plugins: new PluginCollection(),
            onStarted: []
        );

        appHost.Restarted.Register(() => onRestartCount++);

        Assert.AreEqual(0, onRestartCount);

        appHost.Start();

        Assert.AreEqual(0, onRestartCount);

        appHost.Restart();

        Assert.AreEqual(1, onRestartCount);

        appHost.Restart();

        Assert.AreEqual(2, onRestartCount);
    }

    [TestMethod]
    public void Start_And_Restart_Methods_Throw_When_Disposed()
    {
        var appHost = new AppHost(
            services: new ServiceCollection(),
            views: new ViewCollection(),
            plugins: new PluginCollection(),
            onStarted: []
        );

        appHost.Dispose();

        Assert.ThrowsException<ObjectDisposedException>(appHost.Start);
        Assert.ThrowsException<ObjectDisposedException>(appHost.Restart);
    }

    [TestMethod]
    public void Dispose_Method_Invokes_ServiceProvider_Dispose_Method()
    {
        var services = new ServiceCollection();
        services.AddSingleton<DisposableCounter>();

        var appHost = new AppHost(
            services: services,
            views: new ViewCollection(),
            plugins: new PluginCollection(),
            onStarted: []
        );

        appHost.Start();

        var disposableCounter = appHost.ServiceProvider.GetRequiredService<DisposableCounter>();
        Assert.AreEqual(0, disposableCounter.Count);

        appHost.Dispose();

        Assert.AreEqual(1, disposableCounter.Count);

        appHost.Dispose();

        Assert.AreEqual(1, disposableCounter.Count);
    }

    [TestMethod]
    public void Restart_Method_Disposes_Current_ServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddSingleton<DisposableCounter>();

        var appHost = new AppHost(
            services: services,
            views: new ViewCollection(),
            plugins: new PluginCollection(),
            onStarted: []
        );

        appHost.Start();

        var disposableCounter = appHost.ServiceProvider.GetRequiredService<DisposableCounter>();
        Assert.AreEqual(0, disposableCounter.Count);

        appHost.Restart();

        Assert.AreEqual(1, disposableCounter.Count);
    }

    [TestMethod]
    public void Restart_Method_Creates_New_ServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddSingleton<DisposableCounter>();

        var appHost = new AppHost(
            services: services,
            views: new ViewCollection(),
            plugins: new PluginCollection(),
            onStarted: []
        );

        appHost.Start();

        var exampleService1 = appHost.ServiceProvider.GetRequiredService<DisposableCounter>();

        appHost.Restart();

        var exampleService2 = appHost.ServiceProvider.GetRequiredService<DisposableCounter>();

        Assert.AreNotEqual(exampleService1, exampleService2);
    }

    [TestMethod]
    public void Restart_Method_Throws_If_Not_Started()
    {
        var appHost = new AppHost(
            services: new ServiceCollection(),
            views: new ViewCollection(),
            plugins: new PluginCollection(),
            onStarted: []
        );

        Assert.ThrowsException<InvalidOperationException>(appHost.Restart);
    }

    [TestMethod]
    public void Start_Method_Adds_AppHost_As_Service()
    {
        var appHost = new AppHost(
            services: new ServiceCollection(),
            views: new ViewCollection(),
            plugins: new PluginCollection(),
            onStarted: []
        );

        appHost.Start();

        var appHostService = appHost.ServiceProvider.GetService<IAppHost>();
        Assert.AreEqual(appHost, appHostService);
    }

    [TestMethod]
    public void Start_Method_Invoked_More_Than_Once_Has_No_Effect()
    {
        var services = new ServiceCollection();
        services.AddSingleton<DisposableCounter>();

        var startedCount = 0;

        var appHost = new AppHost(
            services: services,
            views: new ViewCollection(),
            plugins: new PluginCollection(),
            onStarted: []
        );

        appHost.Started.Register(() => startedCount++);

        appHost.Start();

        Assert.AreEqual(1, startedCount);

        var exampleService1 = appHost.ServiceProvider.GetRequiredService<DisposableCounter>();

        appHost.Start();

        Assert.AreEqual(1, startedCount);

        var exampleService2 = appHost.ServiceProvider.GetRequiredService<DisposableCounter>();

        Assert.AreEqual(exampleService1, exampleService2);
    }

    #region Types

    private class DisposableCounter : IDisposable
    {
        public int Count { get; private set; }

        public void Dispose() => Count++;
    }

    #endregion
}
