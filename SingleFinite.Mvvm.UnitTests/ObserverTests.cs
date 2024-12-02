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

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class ObserverTests
{
    [TestMethod]
    public void ForEach_Runs_As_Expected()
    {
        var observedCount = 0;

        var observableSource = new ObservableSource();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(() => observedCount++);

        Assert.AreEqual(0, observedCount);

        observableSource.RaiseEvent();

        Assert.AreEqual(1, observedCount);
        observedCount = 0;

        observer.OnEach(() => observedCount++);

        Assert.AreEqual(0, observedCount);

        observableSource.RaiseEvent();

        Assert.AreEqual(2, observedCount);
        observedCount = 0;

        observer.Dispose();

        observableSource.RaiseEvent();

        Assert.AreEqual(0, observedCount);
    }

    [TestMethod]
    public void Select_Runs_As_Expected()
    {
        var observedNames = new List<string>();

        var observableSource = new ObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Select(args => args.Name)
            .OnEach(observedNames.Add);

        Assert.AreEqual(0, observedNames.Count);

        observableSource.RaiseEvent(new("Hello", 0));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Hello", observedNames[0]);
        observedNames.Clear();

        observableSource.RaiseEvent(new("World", 0));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("World", observedNames[0]);
        observedNames.Clear();

        observer.Dispose();
        observableSource.RaiseEvent(new("Again", 0));

        Assert.AreEqual(0, observedNames.Count);
    }

    [TestMethod]
    public void Where_Runs_As_Expected()
    {
        var observedNames = new List<string>();

        var observableSource = new ObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Where(args => args.Name.Length > 3)
            .OnEach(args => observedNames.Add(args.Name));

        Assert.AreEqual(0, observedNames.Count);

        observableSource.RaiseEvent(new("Hello", 0));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Hello", observedNames[0]);
        observedNames.Clear();

        observableSource.RaiseEvent(new("Hi", 0));

        Assert.AreEqual(0, observedNames.Count);

        observer.Dispose();

        observableSource.RaiseEvent(new("Howdy", 0));

        Assert.AreEqual(0, observedNames.Count);
    }

    [TestMethod]
    public void Dispose_If_Runs_As_Expected()
    {
        var observedNames = new List<string>();

        var observableSource = new ObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(args => observedNames.Add(args.Name))
            .DisposeIf(args => args.Name == "stop")
            .OnEach(args => observedNames.Add(args.Name));

        Assert.AreEqual(0, observedNames.Count);

        observableSource.RaiseEvent(new("Hello", 0));

        Assert.AreEqual(2, observedNames.Count);
        Assert.AreEqual("Hello", observedNames[0]);
        Assert.AreEqual("Hello", observedNames[1]);
        observedNames.Clear();

        observableSource.RaiseEvent(new("stop", 0));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("stop", observedNames[0]);
        observedNames.Clear();

        observableSource.RaiseEvent(new("World", 0));

        Assert.AreEqual(0, observedNames.Count);
    }

    [TestMethod]
    public void Of_Type_Runs_As_Expected()
    {
        var observedNames = new List<string>();

        var observableSource = new ObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OfType<SubExampleArgs>()
            .OnEach(args => observedNames.Add(args.SubName));

        Assert.AreEqual(0, observedNames.Count);

        observableSource.RaiseEvent(new("Hello", 0));

        Assert.AreEqual(0, observedNames.Count);

        observableSource.RaiseEvent(new SubExampleArgs(SubName: "Hi"));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Hi", observedNames[0]);
    }

    [TestMethod]
    public void Of_Type_With_Sender_Runs_As_Expected()
    {
        var observedNames = new List<string>();

        var observableSource = new ObservableSource<ExampleSender, ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OfType<SubExampleSender, SubExampleArgs>()
            .OnEach((sender, args) => observedNames.Add(args.SubName));

        Assert.AreEqual(0, observedNames.Count);

        observableSource.RaiseEvent(
            new ExampleSender(),
            new ExampleArgs("Hello", 0)
        );

        Assert.AreEqual(0, observedNames.Count);

        observableSource.RaiseEvent(
            new SubExampleSender(),
            new ExampleArgs("Hello", 0)
        );

        Assert.AreEqual(0, observedNames.Count);

        observableSource.RaiseEvent(
            new ExampleSender(),
            new SubExampleArgs("Hi")
        );

        Assert.AreEqual(0, observedNames.Count);

        observableSource.RaiseEvent(
            new SubExampleSender(),
            new SubExampleArgs("Hi")
        );

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Hi", observedNames[0]);
    }

    [TestMethod]
    public void Once_Runs_As_Expected()
    {
        var observedNames = new List<string>();

        var observableSource = new ObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(args => observedNames.Add(args.Name))
            .Once()
            .OnEach(args => observedNames.Add(args.Name));

        Assert.AreEqual(0, observedNames.Count);

        observableSource.RaiseEvent(new("Hello", 0));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Hello", observedNames[0]);
        observedNames.Clear();

        observableSource.RaiseEvent(new("World", 0));

        Assert.AreEqual(0, observedNames.Count);
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

    #endregion
}
