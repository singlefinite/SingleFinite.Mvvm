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
public sealed class DebouncerTests
{
    [TestMethod]
    public async Task Debounce_Runs_AsExpected()
    {
        var observedNames = new List<string>();

        var exceptionHandler = new ExceptionHandler();
        var observedErrors = 0;
        exceptionHandler.ExceptionHandled.Observe(_ => observedErrors++);

        var dispatcher = new MainDispatcher(
            new DedicatedThreadDispatcher(exceptionHandler),
            new CancellationTokenProvider(),
            exceptionHandler
        );

        var debouncer = new Debouncer();

        debouncer.Debounce(
            action: () => observedNames.Add("One"),
            delay: TimeSpan.FromSeconds(1),
            dispatcher: dispatcher,
            onError: null
        );

        debouncer.Debounce(
            action: () => observedNames.Add("Two"),
            delay: TimeSpan.FromSeconds(1),
            dispatcher: dispatcher,
            onError: null
        );

        debouncer.Debounce(
            action: () => observedNames.Add("Three"),
            delay: TimeSpan.FromSeconds(1),
            dispatcher: dispatcher,
            onError: null
        );

        Assert.AreEqual(0, observedNames.Count);

        await Task.Delay(TimeSpan.FromSeconds(1.1));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Three", observedNames[0]);

        Assert.AreEqual(0, observedErrors);
    }

    [TestMethod]
    public async Task Debounce_With_Async_Func_Runs_AsExpected()
    {
        var observedNames = new List<string>();

        var exceptionHandler = new ExceptionHandler();
        var observedErrors = 0;
        exceptionHandler.ExceptionHandled.Observe(_ => observedErrors++);

        var dispatcher = new MainDispatcher(
            new DedicatedThreadDispatcher(exceptionHandler),
            new CancellationTokenProvider(),
            exceptionHandler
        );

        var debouncer = new Debouncer();

        debouncer.Debounce(
            func: async () =>
            {
                await Task.Delay(5);
                observedNames.Add("One");
            },
            delay: TimeSpan.FromSeconds(1),
            dispatcher: dispatcher,
            onError: null
        );

        debouncer.Debounce(
            func: async () =>
            {
                await Task.Delay(5);
                observedNames.Add("Two");
            },
            delay: TimeSpan.FromSeconds(1),
            dispatcher: dispatcher,
            onError: null
        );

        debouncer.Debounce(
            func: async () =>
            {
                await Task.Delay(5);
                observedNames.Add("Three");
            },
            delay: TimeSpan.FromSeconds(1),
            dispatcher: dispatcher,
            onError: null
        );

        Assert.AreEqual(0, observedNames.Count);

        await Task.Delay(TimeSpan.FromSeconds(1.1));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Three", observedNames[0]);

        Assert.AreEqual(0, observedErrors);
    }
}
