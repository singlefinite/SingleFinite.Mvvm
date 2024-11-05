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
    public void Dispose_Method_Unregisters_Token_Callbacks()
    {
        var scopedEventObserver = new EventObserver();
        var testClass = new TestClassWithEvent();

        TestClassWithEvent? observedSender = null;
        var observedTestEvent = 0;

        scopedEventObserver.Observe(
            token: testClass.EventToken,
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
            token: testClass.EventToken,
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
    public void RaiseEvent_Method_Only_Invokes_Callbacks_With_Compatible_Arg_Types()
    {
        var eventSource = new EventTokenSource<IMessage>();
        var eventObserver = new EventObserver();
        var lines = new List<string>();

        eventObserver.Observe<IMessage, ParentMessage>(
            token: eventSource.Token,
            callback: message => lines.Add($"Parent -> {message.Line}")
        );
        eventObserver.Observe<IMessage, ChildMessage>(
            token: eventSource.Token,
            callback: message => lines.Add($"Child -> {message.Line}")
        );
        eventObserver.Observe<IMessage, GrandChildMessage>(
            token: eventSource.Token,
            callback: message => lines.Add($"GrandChild -> {message.Line}")
        );

        eventSource.RaiseEvent(new ParentMessage());
        Assert.AreEqual(1, lines.Count);
        Assert.AreEqual("Parent -> Parent", lines[0]);
        lines.Clear();

        eventSource.RaiseEvent(new ChildMessage());
        Assert.AreEqual(2, lines.Count);
        Assert.AreEqual("Parent -> Child", lines[0]);
        Assert.AreEqual("Child -> Child", lines[1]);
        lines.Clear();

        eventSource.RaiseEvent(new GrandChildMessage());
        Assert.AreEqual(3, lines.Count);
        Assert.AreEqual("Parent -> GrandChild", lines[0]);
        Assert.AreEqual("Child -> GrandChild", lines[1]);
        Assert.AreEqual("GrandChild -> GrandChild", lines[2]);
        lines.Clear();
    }

    [TestMethod]
    public void PropertyChanging_Events_Invoke_Callbacks()
    {
        var eventObserver = new EventObserver();
        var testClass = new TestClassWithNotify();

        List<int> observedTestEvent = [];

        var registration = eventObserver.ObservePropertyChanging(
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

        registration.Dispose();

        testClass.Number = 3;

        Assert.AreEqual(2, observedTestEvent.Count);
        Assert.AreEqual(0, observedTestEvent[0]);
        Assert.AreEqual(1, observedTestEvent[1]);
    }

    [TestMethod]
    public void Dispose_Method_Unregisters_PropertyChanging_Event_Callbacks()
    {
        var eventObserver = new EventObserver();
        var testClass = new TestClassWithNotify();
        var cancellationTokenSource = new CancellationTokenSource();

        var observedTestEvent = 0;
        var disposeOfPassedInRegistration = false;

        var registration1 = eventObserver.ObservePropertyChanging(
            owner: testClass,
            property: () => testClass.Number,
            callback: () => { observedTestEvent++; }
        );

        var registration2 = eventObserver.ObservePropertyChanging(
            owner: testClass,
            property: () => testClass.Number,
            callback: () => { observedTestEvent++; },
            cancellationToken: cancellationTokenSource.Token
        );

        eventObserver.ObservePropertyChangingWithRegistration(
            owner: testClass,
            property: () => testClass.Number,
            callback: registration =>
            {
                if (disposeOfPassedInRegistration)
                    registration.Dispose();
                else
                    observedTestEvent++;
            }
        );

        var registration4 = eventObserver.ObservePropertyChanging(
            owner: testClass,
            property: () => testClass.Number,
            callback: () => { observedTestEvent++; }
        );

        testClass.Number = 1;

        Assert.AreEqual(4, observedTestEvent);
        observedTestEvent = 0;

        registration1.Dispose();
        testClass.Number = 2;

        Assert.AreEqual(3, observedTestEvent);
        observedTestEvent = 0;

        cancellationTokenSource.Cancel();
        testClass.Number = 3;

        Assert.AreEqual(2, observedTestEvent);
        observedTestEvent = 0;

        disposeOfPassedInRegistration = true;
        testClass.Number = 4;

        Assert.AreEqual(1, observedTestEvent);
        observedTestEvent = 0;

        eventObserver.Dispose();
        testClass.Number = 5;

        Assert.AreEqual(0, observedTestEvent);
    }

    [TestMethod]
    public void PropertyChanged_Events_Invoke_Callbacks()
    {
        var eventObserver = new EventObserver();
        var testClass = new TestClassWithNotify();

        List<int> observedTestEvent = [];

        var registration = eventObserver.ObservePropertyChanged(
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

        registration.Dispose();

        testClass.Number = 3;

        Assert.AreEqual(2, observedTestEvent.Count);
        Assert.AreEqual(1, observedTestEvent[0]);
        Assert.AreEqual(2, observedTestEvent[1]);
    }

    [TestMethod]
    public void Dispose_Method_Unregisters_PropertyChanged_Event_Callbacks()
    {
        var eventObserver = new EventObserver();
        var testClass = new TestClassWithNotify();
        var cancellationTokenSource = new CancellationTokenSource();

        var observedTestEvent = 0;
        var disposeOfPassedInRegistration = false;

        var registration1 = eventObserver.ObservePropertyChanged(
            owner: testClass,
            property: () => testClass.Number,
            callback: () => { observedTestEvent++; }
        );

        var registration2 = eventObserver.ObservePropertyChanged(
            owner: testClass,
            property: () => testClass.Number,
            callback: () => { observedTestEvent++; },
            cancellationToken: cancellationTokenSource.Token
        );

        eventObserver.ObservePropertyChangedWithRegistration(
            owner: testClass,
            property: () => testClass.Number,
            callback: registration =>
            {
                if (disposeOfPassedInRegistration)
                    registration.Dispose();
                else
                    observedTestEvent++;
            }
        );

        var registration4 = eventObserver.ObservePropertyChanged(
            owner: testClass,
            property: () => testClass.Number,
            callback: () => { observedTestEvent++; }
        );

        testClass.Number = 1;

        Assert.AreEqual(4, observedTestEvent);
        observedTestEvent = 0;

        registration1.Dispose();
        testClass.Number = 2;

        Assert.AreEqual(3, observedTestEvent);
        observedTestEvent = 0;

        cancellationTokenSource.Cancel();
        testClass.Number = 3;

        Assert.AreEqual(2, observedTestEvent);
        observedTestEvent = 0;

        disposeOfPassedInRegistration = true;
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
            _eventTokenSource.RaiseEvent(this, args);
        }

        private readonly EventTokenSource<TestClassWithEvent, int> _eventTokenSource = new();
        public EventToken<TestClassWithEvent, int> EventToken => _eventTokenSource.Token;

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
            get => _number;
            set
            {
                if (_number == value)
                    return;

                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Number)));
                _number = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Number)));
            }
        }
        private int _number = 0;

        public event PropertyChangingEventHandler? PropertyChanging;

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    #endregion
}
