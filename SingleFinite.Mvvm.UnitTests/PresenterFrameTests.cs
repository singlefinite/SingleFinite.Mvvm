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

using SingleFinite.Mvvm.Internal.Services;
using SingleFinite.Mvvm.Services;
using Microsoft.Extensions.DependencyInjection;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class PresenterFrameTests
{
    [TestMethod]
    public void Lifecycle_Events_Raised_When_Expected()
    {
        using var context = new TestContext();
        var presenterFrame = (PresenterFrame)context.ServiceProvider.GetRequiredService<IPresenterFrame>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1, ViewModelTestContext>(viewModelContext);

        presenterFrame.Set(viewModelDescriptor1);
        Assert.AreEqual(2, output.Count);
        Assert.AreEqual("OnInit - TestViewModel1", output[0]);
        Assert.AreEqual("OnStart - TestViewModel1", output[1]);

        output.Clear();
        presenterFrame.Clear();
        Assert.AreEqual(2, output.Count);
        Assert.AreEqual("OnStop - TestViewModel1", output[0]);
        Assert.AreEqual("OnDispose - TestViewModel1", output[1]);

        output.Clear();
        presenterFrame.Set<TestViewModel2, ViewModelTestContext>(viewModelContext);
        Assert.AreEqual(2, output.Count);
        Assert.AreEqual("OnInit - TestViewModel2", output[0]);
        Assert.AreEqual("OnStart - TestViewModel2", output[1]);

        output.Clear();
        presenterFrame.Set<TestViewModel3, ViewModelTestContext>(viewModelContext);
        Assert.AreEqual(4, output.Count);
        Assert.AreEqual("OnStop - TestViewModel2", output[0]);
        Assert.AreEqual("OnDispose - TestViewModel2", output[1]);
        Assert.AreEqual("OnInit - TestViewModel3", output[2]);
        Assert.AreEqual("OnStart - TestViewModel3", output[3]);

        output.Clear();
        presenterFrame.Clear();
        Assert.AreEqual(2, output.Count);
        Assert.AreEqual("OnStop - TestViewModel3", output[0]);
        Assert.AreEqual("OnDispose - TestViewModel3", output[1]);
    }

    [TestMethod]
    public void Presenter_Is_Disposed_When_ServiceScope_Is_Disposed()
    {
        using var context = new TestContext();
        var scope = context.ServiceProvider.CreateScope();
        var presenterFrameInScope = (PresenterFrame)scope.ServiceProvider.GetRequiredService<IPresenterFrame>();
        var presenterFrameInRoot = (PresenterFrame)context.ServiceProvider.GetRequiredService<IPresenterFrame>();

        var output = new List<string>();
        var viewModelTestContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1, ViewModelTestContext>(viewModelTestContext);

        presenterFrameInScope.Set(viewModelDescriptor1);
        presenterFrameInRoot.Set(viewModelDescriptor1);
        Assert.IsNotNull(presenterFrameInScope.Current);
        Assert.IsNotNull(presenterFrameInRoot.Current);

        scope.Dispose();
        Assert.IsNull(presenterFrameInScope.Current);
        Assert.IsNotNull(presenterFrameInRoot.Current);
    }

    [TestMethod]
    public void Set_Method_Throws_If_Disposed()
    {
        using var context = new TestContext();
        var presenterFrame = (PresenterFrame)context.ServiceProvider.GetRequiredService<IPresenterFrame>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor = new ViewModelDescriptor<TestViewModel1, ViewModelTestContext>(viewModelContext);

        presenterFrame.Dispose();

        Assert.ThrowsException<ObjectDisposedException>(() => presenterFrame.Set(viewModelDescriptor));
    }

    #region Types

    private class TestView1(TestViewModel1 viewModel) : IView<TestViewModel1>
    {
        public TestViewModel1 ViewModel => viewModel;
    }

    private class TestView2(TestViewModel2 viewModel) : IView<TestViewModel2>
    {
        public TestViewModel2 ViewModel => viewModel;
    }

    private class TestView3(TestViewModel3 viewModel) : IView<TestViewModel3>
    {
        public TestViewModel3 ViewModel => viewModel;
    }

    private record ViewModelTestContext(List<string> Output);

    private class TestViewModel1(ViewModelTestContext context) : ViewModel<ViewModelTestContext>
    {
        protected ViewModelTestContext Context => context;

        protected override void OnInitialize() => Context.Output.Add($"OnInit - {nameof(TestViewModel1)}");
        protected override void OnActivate(CancellationToken _) => Context.Output.Add($"OnStart - {nameof(TestViewModel1)}");
        protected override void OnDeactivate() => Context.Output.Add($"OnStop - {nameof(TestViewModel1)}");
        protected override void OnDispose() => Context.Output.Add($"OnDispose - {nameof(TestViewModel1)}");
    }

    private class TestViewModel2(ViewModelTestContext context) : TestViewModel1(context)
    {
        protected override void OnInitialize() => Context.Output.Add($"OnInit - {nameof(TestViewModel2)}");
        protected override void OnActivate(CancellationToken _) => Context.Output.Add($"OnStart - {nameof(TestViewModel2)}");
        protected override void OnDeactivate() => Context.Output.Add($"OnStop - {nameof(TestViewModel2)}");
        protected override void OnDispose() => Context.Output.Add($"OnDispose - {nameof(TestViewModel2)}");
    }

    private class TestViewModel3(ViewModelTestContext context) : TestViewModel2(context)
    {
        protected override void OnInitialize() => Context.Output.Add($"OnInit - {nameof(TestViewModel3)}");
        protected override void OnActivate(CancellationToken _) => Context.Output.Add($"OnStart - {nameof(TestViewModel3)}");
        protected override void OnDeactivate() => Context.Output.Add($"OnStop - {nameof(TestViewModel3)}");
        protected override void OnDispose() => Context.Output.Add($"OnDispose - {nameof(TestViewModel3)}");
    }

    #endregion
}
