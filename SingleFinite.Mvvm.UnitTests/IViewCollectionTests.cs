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

using System.Reflection;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class IViewCollectionTests
{
    /// <summary>
    /// Make sure the Scan method will register expected types.
    /// </summary>
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

    /// <summary>
    /// Class used to test the view registry.
    /// </summary>
    private class ExampleViewModel : ViewModel
    {
    }

    /// <summary>
    /// Class used to test the view registry.
    /// </summary>
    private class ExampleView(ExampleViewModel viewModel) : IView<ExampleViewModel>
    {
        public ExampleViewModel ViewModel => viewModel;
    }

    /// <summary>
    /// Class used to test the view registry.
    /// </summary>
    private class SubExampleViewModel : ExampleViewModel
    {
    }

    /// <summary>
    /// Class used to test the view registry.
    /// </summary>
    private class SubExampleView(SubExampleViewModel viewModel) : IView<SubExampleViewModel>
    {
        public SubExampleViewModel ViewModel => viewModel;
    }
}
