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
using SingleFinite.Essentials;
using SingleFinite.Mvvm.Internal.Services.Presenters;
using SingleFinite.Mvvm.Services.Presenters;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class ItemPresenterTests
{
    [TestMethod]
    public void Lifecycle_Events_Raised_When_Expected()
    {
        using var context = new MvvmTestContext();
        var itemPresenter = (ItemPresenter)context.ServiceProvider.GetRequiredService<IItemPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1>(viewModelContext);

        itemPresenter.Set(viewModelDescriptor1);
        Assert.HasCount(2, output);
        Assert.AreEqual("OnInit - TestViewModel1", output[0]);
        Assert.AreEqual("OnStart - TestViewModel1", output[1]);

        output.Clear();
        itemPresenter.Clear();
        Assert.HasCount(2, output);
        Assert.AreEqual("OnStop - TestViewModel1", output[0]);
        Assert.AreEqual("OnDispose - TestViewModel1", output[1]);

        output.Clear();
        itemPresenter.Set<TestViewModel2>(viewModelContext);
        Assert.HasCount(2, output);
        Assert.AreEqual("OnInit - TestViewModel2", output[0]);
        Assert.AreEqual("OnStart - TestViewModel2", output[1]);

        output.Clear();
        itemPresenter.Set<TestViewModel3>(viewModelContext);
        Assert.HasCount(4, output);
        Assert.AreEqual("OnInit - TestViewModel3", output[0]);
        Assert.AreEqual("OnStop - TestViewModel2", output[1]);
        Assert.AreEqual("OnDispose - TestViewModel2", output[2]);
        Assert.AreEqual("OnStart - TestViewModel3", output[3]);

        output.Clear();
        itemPresenter.Clear();
        Assert.HasCount(2, output);
        Assert.AreEqual("OnStop - TestViewModel3", output[0]);
        Assert.AreEqual("OnDispose - TestViewModel3", output[1]);
    }

    [TestMethod]
    public void Changed_Event_Is_Raised()
    {
        using var context = new MvvmTestContext();
        var itemPresenter = (ItemPresenter)context.ServiceProvider.GetRequiredService<IItemPresenter>();

        IPresenter.CurrentChangedEventArgs? observedArgs = null;
        itemPresenter.CurrentChanged
            .Observe()
            .OnEach(args => observedArgs = args);

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2>(viewModelContext);

        Assert.IsNull(observedArgs);

        var viewModel1 = itemPresenter.Set(viewModelDescriptor1);

        Assert.IsNotNull(observedArgs);
        Assert.AreEqual(viewModel1, observedArgs.View?.ViewModel);
        Assert.IsTrue(observedArgs.IsNew);

        observedArgs = null;

        var viewModel2 = itemPresenter.Set(viewModelDescriptor2);

        Assert.IsNotNull(observedArgs);
        Assert.AreEqual(viewModel2, observedArgs.View?.ViewModel);
        Assert.IsTrue(observedArgs.IsNew);

        observedArgs = null;

        itemPresenter.Clear();

        Assert.IsNotNull(observedArgs);
        Assert.IsNull(observedArgs.View);
        Assert.IsFalse(observedArgs.IsNew);
    }

    [TestMethod]
    public void Presenter_Is_Disposed_When_ServiceScope_Is_Disposed()
    {
        using var context = new MvvmTestContext();
        var scope = context.ServiceProvider.CreateScope();
        var itemPresenterInScope = (ItemPresenter)scope.ServiceProvider.GetRequiredService<IItemPresenter>();
        var itemPresenterInRoot = (ItemPresenter)context.ServiceProvider.GetRequiredService<IItemPresenter>();

        var output = new List<string>();
        var viewModelTestContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1>(viewModelTestContext);

        itemPresenterInScope.Set(viewModelDescriptor1);
        itemPresenterInRoot.Set(viewModelDescriptor1);
        Assert.IsNotNull(itemPresenterInScope.Current);
        Assert.IsNotNull(itemPresenterInRoot.Current);

        scope.Dispose();
        Assert.IsNull(itemPresenterInScope.Current);
        Assert.IsNotNull(itemPresenterInRoot.Current);
    }

    [TestMethod]
    public void Set_Method_With_Template_Creates_View_With_Template()
    {
        using var context = new MvvmTestContext();
        var itemPresenter = (ItemPresenter)context.ServiceProvider.GetRequiredService<IItemPresenter>();
        var viewModelTestContext = new ViewModelTestContext([]);
        var viewModel = itemPresenter.Set<TestViewModel1>(viewModelTestContext);

        Assert.IsNotNull(viewModel);
    }

    [TestMethod]
    public void Set_Method_Throws_If_Disposed()
    {
        using var context = new MvvmTestContext();
        var itemPresenter = (ItemPresenter)context.ServiceProvider.GetRequiredService<IItemPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor = new ViewModelDescriptor<TestViewModel1>(viewModelContext);

        itemPresenter.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => itemPresenter.Set(viewModelDescriptor));
    }

    [TestMethod]
    public void Closable_Event_Removes_View_Model()
    {
        using var context = new MvvmTestContext();
        var itemPresenter = (ItemPresenter)context.ServiceProvider.GetRequiredService<IItemPresenter>();
        var viewModelTestContext = new ViewModelTestContext([]);
        var viewModel = itemPresenter.Set<TestViewModel1>(viewModelTestContext);

        Assert.IsNotNull(itemPresenter.Current);

        viewModel.Close();

        Assert.IsNull(itemPresenter.Current);
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

    private class TestViewModel1(ViewModelTestContext context) : ViewModel, ICloseObservable
    {
        protected ViewModelTestContext Context => context;

        protected override void OnCreated() => Context.Output.Add($"OnInit - {nameof(TestViewModel1)}");
        protected override void OnActivate(CancellationToken _) => Context.Output.Add($"OnStart - {nameof(TestViewModel1)}");
        protected override void OnDeactivate() => Context.Output.Add($"OnStop - {nameof(TestViewModel1)}");
        protected override void OnDispose() => Context.Output.Add($"OnDispose - {nameof(TestViewModel1)}");

        public void Close() => _closedSource.Emit(this);

        public IEventObservable<ICloseObservable> Closed => _closedSource.Observable;
        private readonly EventObservableSource<ICloseObservable> _closedSource = new();
    }

    private class TestViewModel2(ViewModelTestContext context) : TestViewModel1(context)
    {
        protected override void OnCreated() => Context.Output.Add($"OnInit - {nameof(TestViewModel2)}");
        protected override void OnActivate(CancellationToken _) => Context.Output.Add($"OnStart - {nameof(TestViewModel2)}");
        protected override void OnDeactivate() => Context.Output.Add($"OnStop - {nameof(TestViewModel2)}");
        protected override void OnDispose() => Context.Output.Add($"OnDispose - {nameof(TestViewModel2)}");
    }

    private class TestViewModel3(ViewModelTestContext context) : TestViewModel2(context)
    {
        protected override void OnCreated() => Context.Output.Add($"OnInit - {nameof(TestViewModel3)}");
        protected override void OnActivate(CancellationToken _) => Context.Output.Add($"OnStart - {nameof(TestViewModel3)}");
        protected override void OnDeactivate() => Context.Output.Add($"OnStop - {nameof(TestViewModel3)}");
        protected override void OnDispose() => Context.Output.Add($"OnDispose - {nameof(TestViewModel3)}");
    }

    #endregion
}
