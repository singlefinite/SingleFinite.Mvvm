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

using Microsoft.Extensions.DependencyInjection;
using SingleFinite.Mvvm.Internal.Services;
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class ViewBuilderTests
{
    [TestMethod]
    public void AssembleDoesNotCallOnCreatedUntilStartIsCalled()
    {
        using var context = new MvvmTestContext();
        var viewBuilder = (ViewBuilder)context.ServiceProvider.GetRequiredService<IViewBuilder>();

        var assembleResult = viewBuilder.Assemble<TestViewModel>();

        Assert.AreEqual(0, assembleResult.View.ViewModel.OnCreatedCount);
        Assert.AreEqual(0, assembleResult.View.ViewModel.OnPluginOnCreatedCount);

        assembleResult.Start();

        Assert.AreEqual(1, assembleResult.View.ViewModel.OnCreatedCount);
        Assert.AreEqual(1, assembleResult.View.ViewModel.OnPluginOnCreatedCount);

        assembleResult.Start();

        Assert.AreEqual(1, assembleResult.View.ViewModel.OnCreatedCount);
        Assert.AreEqual(1, assembleResult.View.ViewModel.OnPluginOnCreatedCount);
    }

    #region Types

    private class TestViewModel : ViewModel, IPluginHost
    {
        public int OnCreatedCount { get; private set; }

        public int OnPluginOnCreatedCount { get; set; }

        protected override void OnCreated()
        {
            OnCreatedCount++;
        }
    }

    private class TestViewModelPlugin : Plugin<TestViewModel>
    {
        protected override void OnCreated()
        {
            PluginHost.OnPluginOnCreatedCount++;
        }
    }

    private class TestView(TestViewModel viewModel) : IView<TestViewModel>
    {
        public TestViewModel ViewModel => viewModel;
    }

    #endregion
}
