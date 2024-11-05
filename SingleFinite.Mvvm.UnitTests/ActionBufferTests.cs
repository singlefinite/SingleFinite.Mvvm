// MIT License
// Copyright (c) 2024 SingleFinite
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
public class ActionBufferTests
{
    [TestMethod]
    public void AddOrReplace_Method_Returns_Correct_Value()
    {
        var buffer = new ActionBuffer<int>();

        var result0 = buffer.AddOrReplace(0, () => { });
        var result1 = buffer.AddOrReplace(1, () => { });
        var result2 = buffer.AddOrReplace(2, () => { });
        var result3 = buffer.AddOrReplace(1, () => { });

        Assert.AreEqual(true, result0);
        Assert.AreEqual(true, result1);
        Assert.AreEqual(true, result2);
        Assert.AreEqual(false, result3);
    }

    [TestMethod]
    public void AddOrReplace_Method_Keeps_Actions_In_Order()
    {
        var output = new List<string>();
        var buffer = new ActionBuffer<int>();

        buffer.AddOrReplace(0, () => output.Add("0"));
        buffer.AddOrReplace(1, () => output.Add("1"));
        buffer.AddOrReplace(2, () => output.Add("2"));
        buffer.AddOrReplace(1, () => output.Add("1a"));

        buffer.Flush();

        Assert.AreEqual(3, output.Count);
        Assert.AreEqual("0", output[0]);
        Assert.AreEqual("1a", output[1]);
        Assert.AreEqual("2", output[2]);
    }
}
