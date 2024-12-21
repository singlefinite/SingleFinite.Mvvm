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
using System.Data.Common;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class ComponentTests
{
    [TestMethod]
    public void Property_Changes_Raise_PropertyChanged_Events()
    {
        var testClass = new TestClass();
        var observedEvents = new List<string?>();

        testClass.PropertyChanged += (sender, args) =>
        {
            observedEvents.Add(args.PropertyName);
        };

        testClass.FieldOne = "hello";
        Assert.AreEqual(1, observedEvents.Count);
        Assert.AreEqual("FieldOne", observedEvents[0]);
        observedEvents.Clear();

        testClass.FieldOne = "hello";
        Assert.AreEqual(0, observedEvents.Count);
        observedEvents.Clear();

        testClass.FieldOne = "changed";
        Assert.AreEqual(1, observedEvents.Count);
        Assert.AreEqual("FieldOne", observedEvents[0]);
        observedEvents.Clear();
    }

    [TestMethod]
    public void OnChanged_Updates_Other_Properties()
    {
        var testClass = new TestOnStateChangedClass();

        Assert.AreEqual(0, testClass.FieldOne);
        Assert.AreEqual(0, testClass.FieldTwo);
        Assert.AreEqual(0, testClass.FieldThree);

        testClass.FieldOne = 9;

        Assert.AreEqual(9, testClass.FieldOne);
        Assert.AreEqual(9, testClass.FieldTwo);
        Assert.AreEqual(9, testClass.FieldThree);
    }

    [TestMethod]
    public void OnChanged_Raises_PropertyChange_Events_In_Order()
    {
        var output = new List<string?>();
        var testClass = new TestOnStateChangedClass();
        testClass.PropertyChanging += (_, args) => output.Add($"PropertyChanging: {args.PropertyName} {testClass.CurrentValuesAsString}");
        testClass.PropertyChanged += (_, args) => output.Add($"PropertyChanged: {args.PropertyName} {testClass.CurrentValuesAsString}");

        testClass.FieldOne = 9;

        Assert.AreEqual(6, output.Count);
        Assert.AreEqual("PropertyChanging: FieldOne 0,0,0", output[0]);
        Assert.AreEqual("PropertyChanging: FieldTwo 9,0,0", output[1]);
        Assert.AreEqual("PropertyChanging: FieldThree 9,9,0", output[2]);
        Assert.AreEqual("PropertyChanged: FieldOne 9,9,9", output[3]);
        Assert.AreEqual("PropertyChanged: FieldTwo 9,9,9", output[4]);
        Assert.AreEqual("PropertyChanged: FieldThree 9,9,9", output[5]);
    }

    [TestMethod]
    public void OnChanged_Prevents_Recursive_Calls()
    {
        var testClass = new TestOnStateChangedRecurseClass();

        Assert.AreEqual(0, testClass.FieldOne);
        Assert.AreEqual(0, testClass.FieldTwo);

        testClass.FieldOne = 1;

        Assert.AreEqual(1, testClass.FieldOne);
        Assert.AreEqual(2, testClass.FieldTwo);
    }

    [TestMethod]
    public void PropertyChangingObservable_Observed_Events()
    {
        var observedEvents = new List<(object?, PropertyChangingEventArgs)>();

        var testClass = new TestClass();
        testClass
            .ObservePropertyChanging(() => testClass.FieldOne)
            .OnEach((sender, args) => observedEvents.Add((sender, args)));

        Assert.AreEqual(0, observedEvents.Count);

        testClass.FieldTwo = 99;

        Assert.AreEqual(0, observedEvents.Count);

        testClass.FieldOne = "hello";

        Assert.AreEqual(1, observedEvents.Count);
        var (observedSender, observedArgs) = observedEvents[0];
        Assert.AreEqual(observedSender, testClass);
        Assert.AreEqual(observedArgs.PropertyName, "FieldOne");
    }

    [TestMethod]
    public void PropertyChangedObservable_Observed_Events()
    {
        var observedEvents = new List<(object?, PropertyChangedEventArgs)>();

        var testClass = new TestClass();
        testClass
            .ObservePropertyChanged(() => testClass.FieldOne)
            .OnEach((sender, args) => observedEvents.Add((sender, args)));

        Assert.AreEqual(0, observedEvents.Count);

        testClass.FieldTwo = 99;

        Assert.AreEqual(0, observedEvents.Count);

        testClass.FieldOne = "hello";

        Assert.AreEqual(1, observedEvents.Count);
        var (observedSender, observedArgs) = observedEvents[0];
        Assert.AreEqual(observedSender, testClass);
        Assert.AreEqual(observedArgs.PropertyName, "FieldOne");
    }

    #region Types

    private class TestClass : Component
    {
        public string FieldOne
        {
            get;
            set => ChangeProperty(ref field, value);
        } = "";

        public int FieldTwo
        {
            get;
            set => ChangeProperty(ref field, value);
        }

        public double FieldThree
        {
            get;
            set => ChangeProperty(ref field, value);
        }
    }

    private class TestOnStateChangedClass : Component
    {
        public int FieldOne
        {
            get;
            set => ChangeProperty(ref field, value);
        }

        public int FieldTwo
        {
            get;
            set => ChangeProperty(ref field, value);
        }

        public int FieldThree
        {
            get;
            set => ChangeProperty(ref field, value);
        }

        public string CurrentValuesAsString
        {
            get => $"{FieldOne},{FieldTwo},{FieldThree}";
        }

        protected override void OnChanged()
        {
            if (FieldOne > FieldTwo)
                FieldTwo = FieldOne;

            if (FieldTwo > FieldThree)
                FieldThree = FieldTwo;
        }
    }

    private class TestOnStateChangedRecurseClass : Component
    {
        public int FieldOne
        {
            get;
            set => ChangeProperty(ref field, value);
        }

        public int FieldTwo
        {
            get;
            set => ChangeProperty(ref field, value);
        }

        protected override void OnChanged()
        {
            FieldTwo = FieldOne + 1;
        }
    }

    #endregion
}
