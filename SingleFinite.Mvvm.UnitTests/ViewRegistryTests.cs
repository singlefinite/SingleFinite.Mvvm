// MIT License
// Copyright (c) 2026 Single Finite
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
public class ViewRegistryTests
{
    [TestMethod]
    public void GetViewType_Moves_Up_BaseTypes()
    {
        var viewCollection = new ViewCollection
        {
            new ViewDescriptor(
                ViewModelType: typeof(ParentViewModel),
                ViewType: typeof(ParentView)
            )
        };

        var viewRegistry = new ViewRegistry(viewCollection);

        var viewType = viewRegistry.GetViewType(typeof(ChildViewModel));

        Assert.AreEqual(typeof(ParentView), viewType);
    }

    private abstract class ParentViewModel : ViewModel
    {
    }

    private class ChildViewModel : ParentViewModel
    {
    }

    private class ParentView : IView<ParentViewModel>
    {
        public ParentViewModel ViewModel => throw new NotImplementedException();
    }
}
