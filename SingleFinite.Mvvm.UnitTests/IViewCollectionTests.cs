// MIT License
// Copyright (c) 2025 Single Finite
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

using System.Reflection;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class IViewCollectionTests
{
    [TestMethod]
    public void ScanRegistersViewsFoundInThisFile()
    {
        var viewCollection = new ViewCollection();
        viewCollection.Scan(Assembly.GetExecutingAssembly());

        var descriptor1 = viewCollection.First(descriptor => descriptor.ViewModelType == typeof(ExampleViewModel));
        Assert.AreEqual(typeof(ExampleView), descriptor1.ViewType);

        var descriptor2 = viewCollection.First(descriptor => descriptor.ViewModelType == typeof(SubExampleViewModel));
        Assert.AreEqual(typeof(SubExampleView), descriptor2.ViewType);
    }

    [TestMethod]
    public void AddWithGenericTypeParameters()
    {
        var viewCollection = new ViewCollection();
        viewCollection.Add<ExampleViewModel, ExampleView>();

        Assert.AreEqual(1, viewCollection.Count);

        var descriptor = viewCollection.First();
        Assert.AreEqual(typeof(ExampleViewModel), descriptor.ViewModelType);
        Assert.AreEqual(typeof(ExampleView), descriptor.ViewType);
    }

    #region Types

    private class ExampleViewModel : ViewModel
    {
    }

    private class ExampleView(ExampleViewModel viewModel) : IView<ExampleViewModel>
    {
        public ExampleViewModel ViewModel => viewModel;
    }

    private class SubExampleViewModel : ExampleViewModel
    {
    }

    private class SubExampleView(SubExampleViewModel viewModel) : IView<SubExampleViewModel>
    {
        public SubExampleViewModel ViewModel => viewModel;
    }

    #endregion
}
