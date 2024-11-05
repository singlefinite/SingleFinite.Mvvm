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

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class TransactionTests
{
    [TestMethod]
    public void Transaction_Stays_Open_Until_All_Disposables_Disposed()
    {
        var transaction = new Transaction();

        Assert.IsFalse(transaction.IsOpen);

        var disposable1 = transaction.Start();

        Assert.IsTrue(transaction.IsOpen);

        var disposable2 = transaction.Start();

        Assert.IsTrue(transaction.IsOpen);

        disposable1.Dispose();

        Assert.IsTrue(transaction.IsOpen);

        disposable2.Dispose();

        Assert.IsFalse(transaction.IsOpen);
    }

    [TestMethod]
    public void OnClosed_Event_Raised_When_Transaction_Is_Closed()
    {
        var onClosedCount = 0;

        var transaction = new Transaction();
        transaction.OnClosed.Register(() => onClosedCount++);

        Assert.AreEqual(0, onClosedCount);

        var disposable1 = transaction.Start();

        Assert.AreEqual(0, onClosedCount);

        var disposable2 = transaction.Start();

        Assert.AreEqual(0, onClosedCount);

        disposable1.Dispose();

        Assert.AreEqual(0, onClosedCount);

        disposable2.Dispose();

        Assert.AreEqual(1, onClosedCount);
    }
}
