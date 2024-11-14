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
public class EventTokenTests
{
    [TestMethod]
    public void Register_Method_Invokes_Callback_When_Event_Raised()
    {
        var observedNumber = 0;
        var eventTokenSource = new EventTokenSource<int>();
        eventTokenSource.Token.Register(args => observedNumber = args);

        Assert.AreEqual(0, observedNumber);

        eventTokenSource.RaiseEvent(81);

        Assert.AreEqual(81, observedNumber);
    }

    [TestMethod]
    public void Dispose_Method_Removes_Callback()
    {
        var observedNumber = 0;
        var eventTokenSource = new EventTokenSource<int>();
        var registration = eventTokenSource.Token.Register(args => observedNumber = args);

        Assert.AreEqual(0, observedNumber);

        eventTokenSource.RaiseEvent(81);

        Assert.AreEqual(81, observedNumber);

        registration.Dispose();

        eventTokenSource.RaiseEvent(99);

        Assert.AreEqual(81, observedNumber);
    }

    [TestMethod]
    public void Dispose_Method_On_Parameter_Removes_Callback()
    {
        var observedNumber = 0;
        var eventTokenSource = new EventTokenSource<int>();
        var registration = eventTokenSource.Token.Register(
            (args, disposable) =>
            {
                observedNumber = args;
                disposable.Dispose();
            }
        );

        Assert.AreEqual(0, observedNumber);

        eventTokenSource.RaiseEvent(81);

        Assert.AreEqual(81, observedNumber);

        eventTokenSource.RaiseEvent(99);

        Assert.AreEqual(81, observedNumber);
    }

    [TestMethod]
    public void Register_Method_Raises_ActionEvent_When_Event_Raised()
    {
        var observedNumber = 0;
        var eventTokenSource = new EventTokenSource<int>();
        eventTokenSource.Token.ActionEvent += args => observedNumber = args;

        Assert.AreEqual(0, observedNumber);

        eventTokenSource.RaiseEvent(81);

        Assert.AreEqual(81, observedNumber);
    }
}
