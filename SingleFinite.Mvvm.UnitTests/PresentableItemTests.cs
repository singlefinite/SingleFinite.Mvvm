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

using Microsoft.Extensions.DependencyInjection;
using SingleFinite.Essentials;
using SingleFinite.Mvvm.Internal.Services;
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class PresentableItemTests
{
    [TestMethod]
    public void Lifecycle_Events_Raised_When_Expected()
    {
        using var context = new TestContext();
        var presentableItem = (PresentableItem)context.ServiceProvider.GetRequiredService<IPresentableItem>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1>(viewModelContext);

        presentableItem.Set(viewModelDescriptor1);
        Assert.AreEqual(2, output.Count);
        Assert.AreEqual("OnInit - TestViewModel1", output[0]);
        Assert.AreEqual("OnStart - TestViewModel1", output[1]);

        output.Clear();
        presentableItem.Clear();
        Assert.AreEqual(2, output.Count);
        Assert.AreEqual("OnStop - TestViewModel1", output[0]);
        Assert.AreEqual("OnDispose - TestViewModel1", output[1]);

        output.Clear();
        presentableItem.Set<TestViewModel2>(viewModelContext);
        Assert.AreEqual(2, output.Count);
        Assert.AreEqual("OnInit - TestViewModel2", output[0]);
        Assert.AreEqual("OnStart - TestViewModel2", output[1]);

        output.Clear();
        presentableItem.Set<TestViewModel3>(viewModelContext);
        Assert.AreEqual(4, output.Count);
        Assert.AreEqual("OnInit - TestViewModel3", output[0]);
        Assert.AreEqual("OnStop - TestViewModel2", output[1]);
        Assert.AreEqual("OnDispose - TestViewModel2", output[2]);
        Assert.AreEqual("OnStart - TestViewModel3", output[3]);

        output.Clear();
        presentableItem.Clear();
        Assert.AreEqual(2, output.Count);
        Assert.AreEqual("OnStop - TestViewModel3", output[0]);
        Assert.AreEqual("OnDispose - TestViewModel3", output[1]);
    }

    [TestMethod]
    public void Changed_Event_Is_Raised()
    {
        using var context = new TestContext();
        var presentableItem = (PresentableItem)context.ServiceProvider.GetRequiredService<IPresentableItem>();

        IPresentable.CurrentChangedEventArgs? observedArgs = null;
        presentableItem.CurrentChanged
            .Observe()
            .OnEach(args => observedArgs = args);

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2>(viewModelContext);

        Assert.IsNull(observedArgs);

        var viewModel1 = presentableItem.Set(viewModelDescriptor1);

        Assert.IsNotNull(observedArgs);
        Assert.AreEqual(viewModel1, observedArgs.View?.ViewModel);
        Assert.IsTrue(observedArgs.IsNew);

        observedArgs = null;

        var viewModel2 = presentableItem.Set(viewModelDescriptor2);

        Assert.IsNotNull(observedArgs);
        Assert.AreEqual(viewModel2, observedArgs.View?.ViewModel);
        Assert.IsTrue(observedArgs.IsNew);

        observedArgs = null;

        presentableItem.Clear();

        Assert.IsNotNull(observedArgs);
        Assert.IsNull(observedArgs.View);
        Assert.IsFalse(observedArgs.IsNew);
    }

    [TestMethod]
    public void Presenter_Is_Disposed_When_ServiceScope_Is_Disposed()
    {
        using var context = new TestContext();
        var scope = context.ServiceProvider.CreateScope();
        var presentableItemInScope = (PresentableItem)scope.ServiceProvider.GetRequiredService<IPresentableItem>();
        var presentableItemInRoot = (PresentableItem)context.ServiceProvider.GetRequiredService<IPresentableItem>();

        var output = new List<string>();
        var viewModelTestContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1>(viewModelTestContext);

        presentableItemInScope.Set(viewModelDescriptor1);
        presentableItemInRoot.Set(viewModelDescriptor1);
        Assert.IsNotNull(presentableItemInScope.Current);
        Assert.IsNotNull(presentableItemInRoot.Current);

        scope.Dispose();
        Assert.IsNull(presentableItemInScope.Current);
        Assert.IsNotNull(presentableItemInRoot.Current);
    }

    [TestMethod]
    public void Set_Method_With_Template_Creates_View_With_Template()
    {
        using var context = new TestContext();
        var presentableItem = (PresentableItem)context.ServiceProvider.GetRequiredService<IPresentableItem>();
        var viewModelTestContext = new ViewModelTestContext([]);
        var viewModel = presentableItem.Set<TestViewModel1>(viewModelTestContext);

        Assert.IsNotNull(viewModel);
    }

    [TestMethod]
    public void Set_Method_Throws_If_Disposed()
    {
        using var context = new TestContext();
        var presentableItem = (PresentableItem)context.ServiceProvider.GetRequiredService<IPresentableItem>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor = new ViewModelDescriptor<TestViewModel1>(viewModelContext);

        presentableItem.Dispose();

        Assert.ThrowsException<ObjectDisposedException>(() => presentableItem.Set(viewModelDescriptor));
    }

    [TestMethod]
    public void Closable_Event_Removes_View_Model()
    {
        using var context = new TestContext();
        var presentableItem = (PresentableItem)context.ServiceProvider.GetRequiredService<IPresentableItem>();
        var viewModelTestContext = new ViewModelTestContext([]);
        var viewModel = presentableItem.Set<TestViewModel1>(viewModelTestContext);

        Assert.IsNotNull(presentableItem.Current);

        viewModel.Close();

        Assert.IsNull(presentableItem.Current);
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

    private class TestViewModel1(ViewModelTestContext context) : ViewModel, IClosable
    {
        protected ViewModelTestContext Context => context;

        protected override void OnInitialize() => Context.Output.Add($"OnInit - {nameof(TestViewModel1)}");
        protected override void OnActivate(CancellationToken _) => Context.Output.Add($"OnStart - {nameof(TestViewModel1)}");
        protected override void OnDeactivate() => Context.Output.Add($"OnStop - {nameof(TestViewModel1)}");
        protected override void OnDispose() => Context.Output.Add($"OnDispose - {nameof(TestViewModel1)}");

        public void Close() => _closedSource.RaiseEvent(this);

        public Observable<IClosable> Closed => _closedSource.Observable;
        private readonly ObservableSource<IClosable> _closedSource = new();
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
