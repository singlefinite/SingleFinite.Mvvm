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

using SingleFinite.Mvvm.Internal.Services;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class ThreadPoolDispatcherTests
{
    [TestMethod]
    public async Task Run_Method_Invokes_Action()
    {
        var dispatcher = new ThreadPoolDispatcher();

        var testField1 = 0;
        await dispatcher.RunAsync(() => testField1 = 1);
        Assert.AreEqual(1, testField1);

        var testField2 = await dispatcher.RunAsync(() => 2);
        Assert.AreEqual(2, testField2);

        var testField3 = 0;
        var waitHandle = new ManualResetEvent(false);
        dispatcher.Run(
            () =>
            {
                testField3 = 3;
                waitHandle.Set();
            }
        );
        waitHandle.WaitOne(1000);
        Assert.AreEqual(3, testField3);
    }

    [TestMethod]
    public async Task Run_Method_Propogates_Exceptions_Correctly_When_Thrown()
    {
        var dispatcher = new ThreadPoolDispatcher();

        // Make sure an uncaught exception doesn't bring down the app.
        //
        dispatcher.Run(() => throw new InvalidOperationException());

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(
            async () =>
            {
                await dispatcher.RunAsync(() => throw new InvalidOperationException());
            }
        );
    }

    [TestMethod]
    public async Task Run_Method_Throws_If_Disposed()
    {
        var count = 0;
        using var dispatcher = new ThreadPoolDispatcher();

        dispatcher.Dispose();

        await Assert.ThrowsExceptionAsync<ObjectDisposedException>(() => dispatcher.RunAsync(() => count++));
        Assert.AreEqual(0, count);

        // Make sure multiple calls to dispose don't cause an exception.
        //
        dispatcher.Dispose();
    }
}
