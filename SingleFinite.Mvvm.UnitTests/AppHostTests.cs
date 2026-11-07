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

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class AppHostTests(TestContext testContext)
{
    [TestMethod]
    public async Task Start_Method_Invoke_Initializers()
    {
        var onStarted1Count = 0;
        var onStarted2Count = 0;

        var services = new ServiceCollection();

        var appHost = new AppHost(
            initializers: new InitializerCollection
            {
                _ => onStarted1Count++,
                _ => onStarted2Count++
            }
        );

        var provider = services.BuildServiceProvider();

        Assert.AreEqual(0, onStarted1Count);
        Assert.AreEqual(0, onStarted2Count);

        await appHost.StartAsync(provider);

        Assert.AreEqual(1, onStarted1Count);
        Assert.AreEqual(1, onStarted2Count);

        await appHost.StartAsync(provider);

        Assert.AreEqual(1, onStarted1Count);
        Assert.AreEqual(1, onStarted2Count);
    }

    [TestMethod]
    public async Task Start_Method_Throws_When_Disposed()
    {
        var services = new ServiceCollection();

        var appHost = new AppHost(
            initializers: new InitializerCollection()
        );

        var provider = services.BuildServiceProvider();

        appHost.Dispose();

        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(
            () => appHost.StartAsync(provider)
        );
    }

    [TestMethod]
    public async Task Unobserved_Task_Exception_Is_Handled()
    {
        var exceptionHandler = new TestExceptionHandler();

        var services = new ServiceCollection();
        services.AddSingleton<IExceptionHandler>(exceptionHandler);

        var appHost = new AppHost(
            initializers: new InitializerCollection()
        );

        await appHost.StartAsync(services.BuildServiceProvider());

        _ = Task.Run(
            function: () => throw new InvalidOperationException("Test exception"),
            cancellationToken: testContext.CancellationToken
        );

        await Task.Delay(100, testContext.CancellationToken);

        GC.Collect();
        GC.WaitForPendingFinalizers();

        Assert.HasCount(1, exceptionHandler.Exceptions);
        Assert.IsInstanceOfType<AggregateException>(exceptionHandler.Exceptions[0]);
        Assert.IsInstanceOfType<InvalidOperationException>(exceptionHandler.Exceptions[0]?.InnerException);

        Assert.HasCount(1, exceptionHandler.Messages);
        Assert.AreEqual("Unobserved task exception", exceptionHandler.Messages[0]);
    }

    private class TestExceptionHandler : IExceptionHandler
    {
        public List<Exception?> Exceptions { get; } = [];

        public List<string?> Messages { get; } = [];

        public bool Handle(Exception? exception, string? message = null)
        {
            Exceptions.Add(exception);
            Messages.Add(message);
            return true;
        }
    }
}
