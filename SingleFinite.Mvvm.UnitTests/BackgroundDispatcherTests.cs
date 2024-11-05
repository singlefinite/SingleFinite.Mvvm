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

using SingleFinite.Mvvm.Internal;
using SingleFinite.Mvvm.Internal.Services;
using SingleFinite.Mvvm.Services;
using Microsoft.Extensions.DependencyInjection;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class BackgroundDispatcherTests
{
    [TestMethod]
    public void Disposed_Dependency_Injection_Scope_Will_Cancel_Dispatcher_Cancellation_Tokens()
    {
        var parentCounter = 0;
        var childCounter = 0;
        var secondChildCounter = 0;
        var grandChildCounter = 0;
        var waitHandleForParent = new ManualResetEvent(false);
        var waitHandleForChild = new ManualResetEvent(false);
        var waitHandleForSecondChild = new ManualResetEvent(false);
        var waitHandleForGrandChild = new ManualResetEvent(false);
        var startWaitHandleForParent = new ManualResetEvent(false);
        var startWaitHandleForChild = new ManualResetEvent(false);
        var startWaitHandleForSecondChild = new ManualResetEvent(false);
        var startWaitHandleForGrandChild = new ManualResetEvent(false);

        using var context = new TestContext();

        var dispatcherFromParentScope = context.ServiceProvider.GetService<IBackgroundDispatcher>() as BackgroundDispatcher;
        Assert.IsNotNull(dispatcherFromParentScope);

        using var childScope = context.ServiceProvider.CreateLinkedScope();

        var dispatcherFromChildScope = childScope.ServiceProvider.GetService<IBackgroundDispatcher>();
        Assert.IsNotNull(dispatcherFromChildScope);

        using var secondChildScope = context.ServiceProvider.CreateLinkedScope();

        var dispatcherFromSecondChildScope = secondChildScope.ServiceProvider.GetService<IBackgroundDispatcher>();
        Assert.IsNotNull(dispatcherFromSecondChildScope);

        using var grandChildScope = childScope.ServiceProvider.CreateLinkedScope();

        var dispatcherForGrandChildScope = grandChildScope.ServiceProvider.GetService<IBackgroundDispatcher>();
        Assert.IsNotNull(dispatcherForGrandChildScope);

        dispatcherFromParentScope.Run(
            cancellationToken =>
            {
                startWaitHandleForParent.Set();
                cancellationToken.WaitHandle.WaitOne();
                parentCounter++;
                waitHandleForParent.Set();
            }
        );

        dispatcherFromChildScope.Run(
            cancellationToken =>
            {
                startWaitHandleForChild.Set();
                cancellationToken.WaitHandle.WaitOne();
                childCounter++;
                waitHandleForChild.Set();
            }
        );

        dispatcherFromSecondChildScope.Run(
            cancellationToken =>
            {
                startWaitHandleForSecondChild.Set();
                cancellationToken.WaitHandle.WaitOne();
                secondChildCounter++;
                waitHandleForSecondChild.Set();
            }
        );

        dispatcherForGrandChildScope.Run(
            cancellationToken =>
            {
                startWaitHandleForGrandChild.Set();
                cancellationToken.WaitHandle.WaitOne();
                grandChildCounter++;
                waitHandleForGrandChild.Set();
            }
        );

        startWaitHandleForParent.WaitOne();
        startWaitHandleForChild.WaitOne();
        startWaitHandleForSecondChild.WaitOne();
        startWaitHandleForGrandChild.WaitOne();

        childScope.Dispose();
        waitHandleForChild.WaitOne();
        waitHandleForGrandChild.WaitOne();

        Assert.AreEqual(0, parentCounter);
        Assert.AreEqual(1, childCounter);
        Assert.AreEqual(0, secondChildCounter);
        Assert.AreEqual(1, grandChildCounter);

        (context.ServiceProvider as IDisposable)?.Dispose();
        waitHandleForParent.WaitOne();
        waitHandleForSecondChild.WaitOne();

        Assert.AreEqual(1, parentCounter);
        Assert.AreEqual(1, childCounter);
        Assert.AreEqual(1, secondChildCounter);
        Assert.AreEqual(1, grandChildCounter);
    }

    [TestMethod]
    public void Cancel_Passed_In_Cancellation_Token_Cancels_Dispatcher_Provided_Cancellation_Token()
    {
        var counter = 0;
        var waitHandle = new ManualResetEvent(false);
        var startWaitHandle = new ManualResetEvent(false);
        using var cancellationTokenSource = new CancellationTokenSource();

        using var context = new TestContext();

        var cancellationTokenProvider = context.ServiceProvider.GetRequiredService<ICancellationTokenProvider>();
        var dispatcher = context.ServiceProvider.GetRequiredService<IBackgroundDispatcher>() as BackgroundDispatcher;
        Assert.IsNotNull(dispatcher);

        dispatcher.Run(
            cancellationToken =>
            {
                startWaitHandle.Set();
                cancellationToken.WaitHandle.WaitOne();
                counter++;
                waitHandle.Set();
            },
            cancellationTokenSource.Token
        );

        startWaitHandle.WaitOne();
        cancellationTokenSource.Cancel();

        waitHandle.WaitOne();

        Assert.AreEqual(1, counter);
        Assert.AreEqual(true, cancellationTokenSource.Token.IsCancellationRequested);
        Assert.AreEqual(false, cancellationTokenProvider.CancellationToken.IsCancellationRequested);
    }
}
