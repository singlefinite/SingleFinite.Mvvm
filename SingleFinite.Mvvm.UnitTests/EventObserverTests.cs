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

using System.ComponentModel;
using SingleFinite.Mvvm.Internal.Services;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class EventObserverTests
{
    [TestMethod]
    public void Dispose_Method_Unregisters_Native_Callbacks()
    {
        var scopedEventObserver = new EventObserver();
        var testClass = new TestClassWithEvent();

        var observedTestEvent = 0;

        scopedEventObserver.Observe<TestClassWithEvent.TestDelegate>(
            register: handler => testClass.TestEvent += handler,
            unregister: handler => testClass.TestEvent -= handler,
            handler: x =>
            {
                observedTestEvent += x;
            }
        );

        testClass.RaiseTestEvent(1);
        Assert.AreEqual(1, observedTestEvent);

        scopedEventObserver.Dispose();
        testClass.RaiseTestEvent(1);
        Assert.AreEqual(1, observedTestEvent);
    }

    [TestMethod]
    public void Dispose_Method_Unregisters_Observable_Callbacks()
    {
        var scopedEventObserver = new EventObserver();
        var testClass = new TestClassWithEvent();

        TestClassWithEvent? observedSender = null;
        var observedTestEvent = 0;

        scopedEventObserver.Observe(
            observable: testClass.Observable,
            callback: (sender, args) =>
            {
                observedSender = sender;
                observedTestEvent += args;
            }
        );

        testClass.RaiseTestToken(1);
        Assert.AreEqual(testClass, observedSender);
        Assert.AreEqual(1, observedTestEvent);

        scopedEventObserver.Dispose();
        testClass.RaiseTestToken(1);
        Assert.AreEqual(testClass, observedSender);
        Assert.AreEqual(1, observedTestEvent);
    }

    [TestMethod]
    public void Cancelled_CancellationToken_Unregisters_Callbacks()
    {
        var eventObserver = new EventObserver();
        var testClass = new TestClassWithEvent();

        TestClassWithEvent? observedSender = null;
        var observedTestEvent = 0;
        using var cancellationTokenSource = new CancellationTokenSource();

        eventObserver.Observe(
            observable: testClass.Observable,
            callback: (sender, args) =>
            {
                observedSender = sender;
                observedTestEvent += args;
            },
            cancellationToken: cancellationTokenSource.Token
        );

        testClass.RaiseTestToken(1);
        Assert.AreEqual(testClass, observedSender);
        Assert.AreEqual(1, observedTestEvent);

        cancellationTokenSource.Cancel();
        testClass.RaiseTestToken(1);

        Assert.AreEqual(testClass, observedSender);
        Assert.AreEqual(1, observedTestEvent);
    }

    [TestMethod]
    public void PropertyChanging_Events_Invoke_Name_Callbacks()
    {
        var eventObserver = new EventObserver();
        var testClass = new TestClassWithNotify();

        List<int> observedTestEvent = [];

        var observer = eventObserver.ObservePropertyChanging(
            owner: testClass,
            property: () => testClass.Number,
            callback: () => { observedTestEvent.Add(testClass.Number); }
        );

        Assert.AreEqual(0, observedTestEvent.Count);

        testClass.Number = 1;

        Assert.AreEqual(1, observedTestEvent.Count);
        Assert.AreEqual(0, observedTestEvent[0]);

        testClass.Number = 2;

        Assert.AreEqual(2, observedTestEvent.Count);
        Assert.AreEqual(0, observedTestEvent[0]);
        Assert.AreEqual(1, observedTestEvent[1]);

        observer.Dispose();

        testClass.Number = 3;

        Assert.AreEqual(2, observedTestEvent.Count);
        Assert.AreEqual(0, observedTestEvent[0]);
        Assert.AreEqual(1, observedTestEvent[1]);
    }

    [TestMethod]
    public void PropertyChanging_Events_Invoke_Unamed_Callbacks()
    {
        var eventObserver = new EventObserver();
        var testClass = new TestClassWithNotify();

        List<string?> observedTestEvent = [];

        var observer = eventObserver.ObservePropertyChanging(
            owner: testClass,
            callback: name => { observedTestEvent.Add(name); }
        );

        Assert.AreEqual(0, observedTestEvent.Count);

        testClass.Number = 1;

        Assert.AreEqual(1, observedTestEvent.Count);
        Assert.AreEqual("Number", observedTestEvent[0]);
        observedTestEvent.Clear();

        testClass.Text = "hi";

        Assert.AreEqual(1, observedTestEvent.Count);
        Assert.AreEqual("Text", observedTestEvent[0]);
        observedTestEvent.Clear();

        observer.Dispose();

        testClass.Number = 3;

        Assert.AreEqual(0, observedTestEvent.Count);
    }

    [TestMethod]
    public void Dispose_Method_Unregisters_PropertyChanging_Event_Callbacks()
    {
        var eventObserver = new EventObserver();
        var testClass = new TestClassWithNotify();
        var cancellationTokenSource = new CancellationTokenSource();

        var observedTestEvent = 0;
        var disposeOfObserver = false;

        var observer1 = eventObserver.ObservePropertyChanging(
            owner: testClass,
            property: () => testClass.Number,
            callback: () => { observedTestEvent++; }
        );

        var observer2 = eventObserver.ObservePropertyChanging(
            owner: testClass,
            property: () => testClass.Number,
            callback: () => { observedTestEvent++; },
            cancellationToken: cancellationTokenSource.Token
        );

        eventObserver.ObservePropertyChanging(
            owner: testClass,
            property: () => testClass.Number,
            callback: () =>
            {
                if (!disposeOfObserver)
                    observedTestEvent++;
            }
        ).DisposeIf(() => disposeOfObserver);

        var observer4 = eventObserver.ObservePropertyChanging(
            owner: testClass,
            property: () => testClass.Number,
            callback: () => { observedTestEvent++; }
        );

        testClass.Number = 1;

        Assert.AreEqual(4, observedTestEvent);
        observedTestEvent = 0;

        observer1.Dispose();
        testClass.Number = 2;

        Assert.AreEqual(3, observedTestEvent);
        observedTestEvent = 0;

        cancellationTokenSource.Cancel();
        testClass.Number = 3;

        Assert.AreEqual(2, observedTestEvent);
        observedTestEvent = 0;

        disposeOfObserver = true;
        testClass.Number = 4;

        Assert.AreEqual(1, observedTestEvent);
        observedTestEvent = 0;

        eventObserver.Dispose();
        testClass.Number = 5;

        Assert.AreEqual(0, observedTestEvent);
    }

    [TestMethod]
    public void PropertyChanged_Events_Invoke_Named_Callbacks()
    {
        var eventObserver = new EventObserver();
        var testClass = new TestClassWithNotify();

        List<int> observedTestEvent = [];

        var observer = eventObserver.ObservePropertyChanged(
            owner: testClass,
            property: () => testClass.Number,
            callback: () => { observedTestEvent.Add(testClass.Number); }
        );

        Assert.AreEqual(0, observedTestEvent.Count);

        testClass.Number = 1;

        Assert.AreEqual(1, observedTestEvent.Count);
        Assert.AreEqual(1, observedTestEvent[0]);

        testClass.Number = 2;

        Assert.AreEqual(2, observedTestEvent.Count);
        Assert.AreEqual(1, observedTestEvent[0]);
        Assert.AreEqual(2, observedTestEvent[1]);

        observer.Dispose();

        testClass.Number = 3;

        Assert.AreEqual(2, observedTestEvent.Count);
        Assert.AreEqual(1, observedTestEvent[0]);
        Assert.AreEqual(2, observedTestEvent[1]);
    }

    [TestMethod]
    public void PropertyChanged_Events_Invoke_Unamed_Callbacks()
    {
        var eventObserver = new EventObserver();
        var testClass = new TestClassWithNotify();

        List<string?> observedTestEvent = [];

        var observer = eventObserver.ObservePropertyChanged(
            owner: testClass,
            callback: name => { observedTestEvent.Add(name); }
        );

        Assert.AreEqual(0, observedTestEvent.Count);

        testClass.Number = 1;

        Assert.AreEqual(1, observedTestEvent.Count);
        Assert.AreEqual("Number", observedTestEvent[0]);
        observedTestEvent.Clear();

        testClass.Text = "hi";

        Assert.AreEqual(1, observedTestEvent.Count);
        Assert.AreEqual("Text", observedTestEvent[0]);
        observedTestEvent.Clear();

        observer.Dispose();

        testClass.Number = 3;

        Assert.AreEqual(0, observedTestEvent.Count);
    }

    [TestMethod]
    public void Dispose_Method_Unregisters_PropertyChanged_Event_Callbacks()
    {
        var eventObserver = new EventObserver();
        var testClass = new TestClassWithNotify();
        var cancellationTokenSource = new CancellationTokenSource();

        var observedTestEvent = 0;
        var disposeOfObserver = false;

        var observer1 = eventObserver.ObservePropertyChanged(
            owner: testClass,
            property: () => testClass.Number,
            callback: () => { observedTestEvent++; }
        );

        var observer2 = eventObserver.ObservePropertyChanged(
            owner: testClass,
            property: () => testClass.Number,
            callback: () => { observedTestEvent++; },
            cancellationToken: cancellationTokenSource.Token
        );

        eventObserver.ObservePropertyChanged(
            owner: testClass,
            property: () => testClass.Number,
            callback: () =>
            {
                if (!disposeOfObserver)
                    observedTestEvent++;
            }
        ).DisposeIf(() => disposeOfObserver);

        var observer4 = eventObserver.ObservePropertyChanged(
            owner: testClass,
            property: () => testClass.Number,
            callback: () => { observedTestEvent++; }
        );

        testClass.Number = 1;

        Assert.AreEqual(4, observedTestEvent);
        observedTestEvent = 0;

        observer1.Dispose();
        testClass.Number = 2;

        Assert.AreEqual(3, observedTestEvent);
        observedTestEvent = 0;

        cancellationTokenSource.Cancel();
        testClass.Number = 3;

        Assert.AreEqual(2, observedTestEvent);
        observedTestEvent = 0;

        disposeOfObserver = true;
        testClass.Number = 4;

        Assert.AreEqual(1, observedTestEvent);
        observedTestEvent = 0;

        eventObserver.Dispose();
        testClass.Number = 5;

        Assert.AreEqual(0, observedTestEvent);
    }

    #region Types

    private interface IMessage
    {
        string Line { get; }
    }

    private class ParentMessage : IMessage
    {
        public virtual string Line { get; } = "Parent";
    }

    private class ChildMessage : ParentMessage
    {
        public override string Line { get; } = "Child";
    }

    private class GrandChildMessage : ChildMessage
    {
        public override string Line { get; } = "GrandChild";
    }

    private class TestClassWithEvent
    {
        public void RaiseTestToken(int args)
        {
            _observableSource.RaiseEvent(this, args);
        }

        public Observable<TestClassWithEvent, int> Observable => _observableSource.Observable;
        private readonly ObservableSource<TestClassWithEvent, int> _observableSource = new();

        public void RaiseTestEvent(int x)
        {
            TestEvent?.Invoke(x);
        }

        public event TestDelegate? TestEvent;

        public delegate void TestDelegate(int x);
    }

    private class TestClassWithNotify : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public int Number
        {
            get => field;
            set
            {
                if (field == value)
                    return;

                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Number)));
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Number)));
            }
        }

        public string? Text
        {
            get => field;
            set
            {
                if (field == value)
                    return;

                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Text)));
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
            }
        }

        public event PropertyChangingEventHandler? PropertyChanging;

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    #endregion
}
