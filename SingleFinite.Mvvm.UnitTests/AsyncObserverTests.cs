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

using SingleFinite.Mvvm.Internal;
using SingleFinite.Mvvm.Internal.Services;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class AsyncObserverTests
{
    [TestMethod]
    public async Task ForEach_Runs_As_Expected_Async()
    {
        var observedCount = 0;

        var observableSource = new AsyncObservableSource();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(async () =>
            {
                await Task.Run(() => observedCount++);
            });

        Assert.AreEqual(0, observedCount);

        await observableSource.RaiseEventAsync();

        Assert.AreEqual(1, observedCount);
        observedCount = 0;

        observer.OnEach(async () =>
        {
            await Task.Run(() => observedCount++);
        });

        Assert.AreEqual(0, observedCount);

        await observableSource.RaiseEventAsync();

        Assert.AreEqual(2, observedCount);
        observedCount = 0;

        observer.Dispose();

        await observableSource.RaiseEventAsync();

        Assert.AreEqual(0, observedCount);
    }

    [TestMethod]
    public async Task Select_Runs_As_Expected_Async()
    {
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Select(args => Task.Run(() => args.Name))
            .OnEach(async name =>
            {
                await Task.Run(() => observedNames.Add(name));
            });

        Assert.AreEqual(0, observedNames.Count);

        await observableSource.RaiseEventAsync(new("Hello", 0));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Hello", observedNames[0]);
        observedNames.Clear();

        await observableSource.RaiseEventAsync(new("World", 0));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("World", observedNames[0]);
        observedNames.Clear();

        observer.Dispose();
        await observableSource.RaiseEventAsync(new("Again", 0));

        Assert.AreEqual(0, observedNames.Count);
    }

    [TestMethod]
    public async Task Where_Runs_As_Expected_Async()
    {
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Where(args => Task.Run(() => args.Name.Length > 3))
            .OnEach(async args =>
            {
                await Task.Run(() => observedNames.Add(args.Name));
            });

        Assert.AreEqual(0, observedNames.Count);

        await observableSource.RaiseEventAsync(new("Hello", 0));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Hello", observedNames[0]);
        observedNames.Clear();

        await observableSource.RaiseEventAsync(new("Hi", 0));

        Assert.AreEqual(0, observedNames.Count);

        observer.Dispose();

        await observableSource.RaiseEventAsync(new("Howdy", 0));

        Assert.AreEqual(0, observedNames.Count);
    }

    [TestMethod]
    public async Task Until_Runs_As_Expected_Async()
    {
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(async args =>
            {
                await Task.Run(() => observedNames.Add(args.Name));
            })
            .Until(args => Task.Run(() => args.Name == "stop"))
            .OnEach(async args =>
            {
                await Task.Run(() => observedNames.Add(args.Name));
            });

        Assert.AreEqual(0, observedNames.Count);

        await observableSource.RaiseEventAsync(new("Hello", 0));

        Assert.AreEqual(2, observedNames.Count);
        Assert.AreEqual("Hello", observedNames[0]);
        Assert.AreEqual("Hello", observedNames[1]);
        observedNames.Clear();

        await observableSource.RaiseEventAsync(new("stop", 0));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("stop", observedNames[0]);
        observedNames.Clear();

        await observableSource.RaiseEventAsync(new("World", 0));

        Assert.AreEqual(0, observedNames.Count);
    }

    [TestMethod]
    public async Task On_Runs_As_Expected_Async()
    {
        var viewModel = new ExampleViewModel();

        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(args => observedNames.Add(args.Name))
            .On(viewModel);

        Assert.AreEqual(0, observedNames.Count);

        await observableSource.RaiseEventAsync(new("Hello", 0));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Hello", observedNames[0]);
        observedNames.Clear();

        viewModel.Dispose();

        await observableSource.RaiseEventAsync(new("World", 0));

        Assert.AreEqual(0, observedNames.Count);
    }

    [TestMethod]
    public async Task Until_Continue_On_Disposed_Runs_As_Expected_Async()
    {
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(async args =>
            {
                await Task.Run(() => observedNames.Add(args.Name));
            })
            .Until(
                args => Task.Run(() => args.Name == "stop"),
                continueOnDispose: true
            )
            .OnEach(async args =>
            {
                await Task.Run(() => observedNames.Add($"{args.Name}!"));
            });

        Assert.AreEqual(0, observedNames.Count);

        await observableSource.RaiseEventAsync(new("Hello", 0));

        Assert.AreEqual(2, observedNames.Count);
        Assert.AreEqual("Hello", observedNames[0]);
        Assert.AreEqual("Hello!", observedNames[1]);
        observedNames.Clear();

        await observableSource.RaiseEventAsync(new("stop", 0));

        Assert.AreEqual(2, observedNames.Count);
        Assert.AreEqual("stop", observedNames[0]);
        Assert.AreEqual("stop!", observedNames[1]);
        observedNames.Clear();

        await observableSource.RaiseEventAsync(new("World", 0));

        Assert.AreEqual(0, observedNames.Count);
    }

    [TestMethod]
    public async Task Of_Type_Runs_As_Expected_Async()
    {
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OfType<SubExampleArgs>()
            .OnEach(async args =>
            {
                await Task.Run(() => observedNames.Add(args.SubName));
            });

        Assert.AreEqual(0, observedNames.Count);

        await observableSource.RaiseEventAsync(new("Hello", 0));

        Assert.AreEqual(0, observedNames.Count);

        await observableSource.RaiseEventAsync(new SubExampleArgs(SubName: "Hi"));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Hi", observedNames[0]);
    }

    [TestMethod]
    public async Task Of_Type_With_Sender_Runs_As_Expected_Async()
    {
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleSender, ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OfType<SubExampleSender, SubExampleArgs>()
            .OnEach(async (sender, args) =>
            {
                await Task.Run(() => observedNames.Add(args.SubName));
            });

        Assert.AreEqual(0, observedNames.Count);

        await observableSource.RaiseEventAsync(
            new ExampleSender(),
            new ExampleArgs("Hello", 0)
        );

        Assert.AreEqual(0, observedNames.Count);

        await observableSource.RaiseEventAsync(
            new SubExampleSender(),
            new ExampleArgs("Hello", 0)
        );

        Assert.AreEqual(0, observedNames.Count);

        await observableSource.RaiseEventAsync(
            new ExampleSender(),
            new SubExampleArgs("Hi")
        );

        Assert.AreEqual(0, observedNames.Count);

        await observableSource.RaiseEventAsync(
            new SubExampleSender(),
            new SubExampleArgs("Hi")
        );

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Hi", observedNames[0]);
    }

    [TestMethod]
    public async Task Once_Runs_As_Expected_Async()
    {
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(async args =>
            {
                await Task.Run(() => observedNames.Add(args.Name));
            })
            .Once()
            .OnEach(async args =>
            {
                await Task.Run(() => observedNames.Add($"{args.Name}!"));
            });

        Assert.AreEqual(0, observedNames.Count);

        await observableSource.RaiseEventAsync(new("Hello", 0));

        Assert.AreEqual(2, observedNames.Count);
        Assert.AreEqual("Hello", observedNames[0]);
        Assert.AreEqual("Hello!", observedNames[1]);
        observedNames.Clear();

        await observableSource.RaiseEventAsync(new("World", 0));

        Assert.AreEqual(0, observedNames.Count);
    }

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

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Debounce(
                delay: TimeSpan.FromSeconds(1),
                dispatcher: dispatcher
            )
            .OnEach(async args =>
            {
                await Task.Delay(5);
                observedNames.Add(args.Name);
            });

        Assert.AreEqual(0, observedNames.Count);

        await observableSource.RaiseEventAsync(new("One", 0));
        await observableSource.RaiseEventAsync(new("Two", 0));
        await observableSource.RaiseEventAsync(new("Three", 0));

        Assert.AreEqual(0, observedNames.Count);

        await Task.Delay(TimeSpan.FromSeconds(1.1));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Three", observedNames[0]);

        Assert.AreEqual(0, observedErrors);
    }

    [TestMethod]
    public async Task Catch_Catches_Exceptions_Async()
    {
        var observedExceptions = new List<Exception>();
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Catch(async (_, exArgs) =>
            {
                await Task.Run(() => observedExceptions.Add(exArgs.Exception));
            })
            .OnEach(async args =>
            {
                await Task.Run(() =>
                {
                    if (args.Age == 99)
                        throw new InvalidOperationException("err");
                });
            })
            .OnEach(async args =>
            {
                await Task.Run(() => observedNames.Add(args.Name));
            });

        await observableSource.RaiseEventAsync(new ExampleArgs("Hello", 99));

        Assert.AreEqual(1, observedExceptions.Count);
        Assert.IsInstanceOfType<InvalidOperationException>(observedExceptions[0]);
        Assert.AreEqual(0, observedNames.Count);

        observedExceptions.Clear();
        observedNames.Clear();

        await observableSource.RaiseEventAsync(new ExampleArgs("Hi", 10));

        Assert.AreEqual(0, observedExceptions.Count);
        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Hi", observedNames[0]);
    }

    [TestMethod]
    public async Task Catch_Moves_Past_When_Not_Handled_Async()
    {
        var observedUnhandledExceptions = new List<Exception>();
        var observedExceptions = new List<Exception>();
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Catch(async (_, exArgs) =>
            {
                await Task.Run(() => observedUnhandledExceptions.Add(exArgs.Exception));
            })
            .Catch((args, exArgs) =>
            {
                return Task.Run(() =>
                {
                    observedExceptions.Add(exArgs.Exception);
                    exArgs.IsHandled = args.Age > 50;
                });
            })
            .OnEach(async args =>
            {
                await Task.Run(() =>
                {
                    if (args.Age == 99 || args.Age == 11)
                        throw new InvalidOperationException("err");
                });
            })
            .OnEach(async args =>
            {
                await Task.Run(() => observedNames.Add(args.Name));
            });

        await observableSource.RaiseEventAsync(new ExampleArgs("Hello", 99));

        Assert.AreEqual(0, observedUnhandledExceptions.Count);
        Assert.AreEqual(1, observedExceptions.Count);
        Assert.IsInstanceOfType<InvalidOperationException>(observedExceptions[0]);
        Assert.AreEqual(0, observedNames.Count);

        observedUnhandledExceptions.Clear();
        observedExceptions.Clear();
        observedNames.Clear();

        await observableSource.RaiseEventAsync(new ExampleArgs("World", 11));

        Assert.AreEqual(1, observedUnhandledExceptions.Count);
        Assert.IsInstanceOfType<InvalidOperationException>(observedUnhandledExceptions[0]);
        Assert.AreEqual(1, observedExceptions.Count);
        Assert.IsInstanceOfType<InvalidOperationException>(observedExceptions[0]);
        Assert.AreEqual(0, observedNames.Count);

        observedUnhandledExceptions.Clear();
        observedExceptions.Clear();
        observedNames.Clear();

        await observableSource.RaiseEventAsync(new ExampleArgs("Hi", 10));

        Assert.AreEqual(0, observedUnhandledExceptions.Count);
        Assert.AreEqual(0, observedExceptions.Count);
        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Hi", observedNames[0]);
    }

    [TestMethod]
    public async Task Limit_Limits_The_Number_Of_Executing_Observers()
    {
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Limit(
                maxConcurrent: 1,
                maxBuffer: 0
            )
            .OnEach(async args =>
            {
                await Task.Delay(500);
                observedNames.Add(args.Name);
            });

        Assert.AreEqual(0, observedNames.Count);

        var firstEventTask = observableSource.RaiseEventAsync(new("Hello", 0));
        var secondEventTask = observableSource.RaiseEventAsync(new("World", 0));

        await firstEventTask;
        await secondEventTask;

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Hello", observedNames[0]);
    }

    #region Types

    private record ExampleArgs(
        string Name,
        int Age
    );

    private record SubExampleArgs(
        string SubName
    ) : ExampleArgs(
        Name: "Sub",
        Age: 0
    );

    private class ExampleSender;

    private class SubExampleSender : ExampleSender;

    private class ExampleViewModel : ViewModel
    {
    }

    #endregion
}
