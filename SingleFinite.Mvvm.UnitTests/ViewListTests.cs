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
using SingleFinite.Mvvm.Internal;
using SingleFinite.Mvvm.Services;
using SingleFinite.Mvvm.Services.Presenters;
using static SingleFinite.Mvvm.Services.Presenters.IPresenter;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class ViewListTests(TestContext testContext)
{
    [TestMethod]
    public async Task Lifecycle_Events_Raised_When_Expected()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var list = (ViewList)context.ServiceProvider.GetRequiredService<IViewList>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2>(viewModelContext);
        var viewModelDescriptor3 = new ViewModelDescriptor<TestViewModel3>(viewModelContext);

        list.Add(viewModelDescriptor1);
        Assert.HasCount(1, list);
        Assert.HasCount(2, output);
        Assert.AreEqual("OnInit - TestViewModel1", output[0]);
        Assert.AreEqual("OnStart - TestViewModel1", output[1]);

        output.Clear();

        list.AddAll([viewModelDescriptor2, viewModelDescriptor3]);
        Assert.HasCount(3, list);
        Assert.HasCount(4, output);
        Assert.AreEqual("OnInit - TestViewModel2", output[0]);
        Assert.AreEqual("OnStart - TestViewModel2", output[1]);
        Assert.AreEqual("OnInit - TestViewModel3", output[2]);
        Assert.AreEqual("OnStart - TestViewModel3", output[3]);

        output.Clear();

        list.Remove(2);
        Assert.HasCount(2, list);
        Assert.HasCount(2, output);
        Assert.AreEqual("OnStop - TestViewModel3", output[0]);
        Assert.AreEqual("OnDispose - TestViewModel3", output[1]);

        output.Clear();

        list.Clear();
        Assert.IsEmpty(list);
        Assert.HasCount(4, output);
        Assert.AreEqual("OnStop - TestViewModel1", output[0]);
        Assert.AreEqual("OnDispose - TestViewModel1", output[1]);
        Assert.AreEqual("OnStop - TestViewModel2", output[2]);
        Assert.AreEqual("OnDispose - TestViewModel2", output[3]);
    }

    [TestMethod]
    public async Task Closable_Event_Removes_View_Model()
    {
        using var context = await MvvmTestContext.CreateAsync();

        var viewModelTestContext = new ViewModelTestContext([]);

        var list = (ViewList)context.ServiceProvider.GetRequiredService<IViewList>();
        var viewModel = list.Add<TestViewModel1>(viewModelTestContext);

        Assert.HasCount(1, list);

        viewModel.Close();

        Assert.IsEmpty(list);
    }

    [TestMethod]
    public async Task Changed_Is_Observed_When_Changes_Occur()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var list = (ViewList)context.ServiceProvider.GetRequiredService<IViewList>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);

        var observedCount = 0;
        list.Changed
            .Observe()
            .OnEach(() => observedCount++)
            .Until(testContext.CancellationToken);

        list.Add<TestViewModel1>(viewModelContext);
        Assert.AreEqual(1, observedCount);

        observedCount = 0;

        list.Add<TestViewModel2>(viewModelContext);
        Assert.AreEqual(1, observedCount);

        observedCount = 0;

        list.Move(0, 1);
        Assert.AreEqual(1, observedCount);

        observedCount = 0;

        list.Remove(0);
        Assert.AreEqual(1, observedCount);

        observedCount = 0;

        list.Clear();
        Assert.AreEqual(1, observedCount);

        observedCount = 0;

        list.Clear();
        Assert.AreEqual(0, observedCount);
    }

    [TestMethod]
    public async Task Move_To_Same_Index_Has_No_Effect()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var list = (ViewList)context.ServiceProvider.GetRequiredService<IViewList>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);

        list.Add<TestViewModel1>(viewModelContext);
        list.Add<TestViewModel2>(viewModelContext);

        var observedChangedCount = 0;
        list.Changed
            .Observe()
            .OnEach(() => observedChangedCount++)
            .Until(testContext.CancellationToken);

        list.Move(0, 0);
        Assert.AreEqual(0, observedChangedCount);
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

    private class ParentView(ParentViewModel viewModel) : IView<ParentViewModel>
    {
        public ParentViewModel ViewModel => viewModel;
    }

    private class ChildView(ChildViewModel viewModel) : IView<ChildViewModel>
    {
        public ChildViewModel ViewModel => viewModel;
    }

    private class ExampleView(ExampleViewModel viewModel) : IView<ExampleViewModel>
    {
        public ExampleViewModel ViewModel => viewModel;
    }

    private record ViewModelTestContext(List<string> Output);

    private class TestViewModel1(ViewModelTestContext context) : ViewModel, ICloseObservable
    {
        protected ViewModelTestContext Context => context;

        protected override void OnCreated() => Context.Output.Add($"OnInit - {nameof(TestViewModel1)}");
        protected override void OnActivate() => Context.Output.Add($"OnStart - {nameof(TestViewModel1)}");
        protected override void OnDeactivate() => Context.Output.Add($"OnStop - {nameof(TestViewModel1)}");
        protected override void OnDispose() => Context.Output.Add($"OnDispose - {nameof(TestViewModel1)}");

        public void Close() => _closedSource.Emit(this);

        public IEventObservable<ICloseObservable> Closed => _closedSource.Observable;
        private readonly EventObservableSource<ICloseObservable> _closedSource = new();
    }

    private class TestViewModel2(ViewModelTestContext context) : TestViewModel1(context)
    {
        protected override void OnCreated() => Context.Output.Add($"OnInit - {nameof(TestViewModel2)}");
        protected override void OnActivate() => Context.Output.Add($"OnStart - {nameof(TestViewModel2)}");
        protected override void OnDeactivate() => Context.Output.Add($"OnStop - {nameof(TestViewModel2)}");
        protected override void OnDispose() => Context.Output.Add($"OnDispose - {nameof(TestViewModel2)}");
    }

    private class TestViewModel3(ViewModelTestContext context) : TestViewModel2(context)
    {
        protected override void OnCreated() => Context.Output.Add($"OnInit - {nameof(TestViewModel3)}");
        protected override void OnActivate() => Context.Output.Add($"OnStart - {nameof(TestViewModel3)}");
        protected override void OnDeactivate() => Context.Output.Add($"OnStop - {nameof(TestViewModel3)}");
        protected override void OnDispose() => Context.Output.Add($"OnDispose - {nameof(TestViewModel3)}");
    }

    private class ParentViewModel(ITabPresenter presenter) : ViewModel
    {
        public ChildViewModel? Child { get; private set; }

        protected override void OnCreated()
        {
            Child = presenter.Add<ChildViewModel>();
            presenter.SetCurrent(Child);
        }
    }

    private class ChildViewModel(ITabPresenter presenter) : ViewModel
    {
        public ExampleViewModel? Example { get; private set; }

        protected override void OnCreated()
        {
            Example = presenter.Add<ExampleViewModel>();
            presenter.SetCurrent(Example);
        }
    }

    private class ExampleViewModel : ViewModel
    {
    }

    #endregion
}
