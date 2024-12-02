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
public class ObservableTests
{
    [TestMethod]
    public void Observe_Method_Invokes_Callback_When_Event_Raised()
    {
        var observedNumber = 0;
        var observableSource = new ObservableSource<int>();
        observableSource.Observable.Observe(args => observedNumber = args);

        Assert.AreEqual(0, observedNumber);

        observableSource.RaiseEvent(81);

        Assert.AreEqual(81, observedNumber);
    }

    [TestMethod]
    public void Dispose_Method_Removes_Callback()
    {
        var observedNumber = 0;
        var observableSource = new ObservableSource<int>();
        var observer = observableSource.Observable.Observe(args => observedNumber = args);

        Assert.AreEqual(0, observedNumber);

        observableSource.RaiseEvent(81);

        Assert.AreEqual(81, observedNumber);

        observer.Dispose();

        observableSource.RaiseEvent(99);

        Assert.AreEqual(81, observedNumber);
    }

    [TestMethod]
    public void Observe_Method_Raises_Event_When_Event_Raised()
    {
        var observedNumber = 0;
        var observableSource = new ObservableSource<int>();
        observableSource.Observable.Event += args => observedNumber = args;

        Assert.AreEqual(0, observedNumber);

        observableSource.RaiseEvent(81);

        Assert.AreEqual(81, observedNumber);
    }
}
