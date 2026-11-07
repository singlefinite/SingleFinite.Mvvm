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
using SingleFinite.Mvvm.Internal.Services.Presenters;
using SingleFinite.Mvvm.Services.Presenters;
using static SingleFinite.Mvvm.Services.Presenters.IPresenter;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class TabPresenterTests
{
    public TestContext TestContext { get; set; }

    [TestMethod]
    public async Task Lifecycle_Events_Raised_When_Expected()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var tabPresenter = (TabPresenter)context.ServiceProvider.GetRequiredService<ITabPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2>(viewModelContext);
        var viewModelDescriptor3 = new ViewModelDescriptor<TestViewModel3>(viewModelContext);

        tabPresenter.Add(viewModelDescriptor1);
        Assert.HasCount(1, output);
        Assert.AreEqual("OnInit - TestViewModel1", output[0]);

        tabPresenter.SetCurrentIndex(0);
        Assert.HasCount(2, output);
        Assert.AreEqual("OnStart - TestViewModel1", output[1]);

        output.Clear();

        tabPresenter.AddAll([viewModelDescriptor2, viewModelDescriptor3]);
        Assert.HasCount(2, output);
        Assert.AreEqual("OnInit - TestViewModel2", output[0]);
        Assert.AreEqual("OnInit - TestViewModel3", output[1]);

        tabPresenter.SetCurrentIndex(2);
        Assert.HasCount(4, output);
        Assert.AreEqual("OnStop - TestViewModel1", output[2]);
        Assert.AreEqual("OnStart - TestViewModel3", output[3]);

        output.Clear();

        tabPresenter.Remove(2);
        Assert.HasCount(2, output);
        Assert.AreEqual("OnStop - TestViewModel3", output[0]);
        Assert.AreEqual("OnDispose - TestViewModel3", output[1]);

        tabPresenter.SetCurrentIndex(1);
        Assert.HasCount(3, output);
        Assert.AreEqual("OnStart - TestViewModel2", output[2]);

        output.Clear();

        tabPresenter.Clear();
        Assert.HasCount(3, output);
        Assert.AreEqual("OnStop - TestViewModel2", output[0]);
        Assert.AreEqual("OnDispose - TestViewModel1", output[1]);
        Assert.AreEqual("OnDispose - TestViewModel2", output[2]);
    }

    [TestMethod]
    public async Task SetCurrent_With_Type()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var tabPresenter = (TabPresenter)context.ServiceProvider.GetRequiredService<ITabPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2>(viewModelContext);

        tabPresenter.AddAll([viewModelDescriptor1, viewModelDescriptor2]);

        Assert.IsNull(tabPresenter.Current);
        Assert.AreEqual(-1, tabPresenter.CurrentIndex);

        tabPresenter.SetCurrent<TestViewModel1>();

        Assert.IsInstanceOfType<TestViewModel1>(tabPresenter.Current?.ViewModel);
        Assert.AreEqual(0, tabPresenter.CurrentIndex);

        tabPresenter.SetCurrent<TestViewModel2>();

        Assert.IsInstanceOfType<TestViewModel2>(tabPresenter.Current?.ViewModel);
        Assert.AreEqual(1, tabPresenter.CurrentIndex);

        tabPresenter.SetCurrent<TestViewModel3>();

        Assert.IsNull(tabPresenter.Current);
        Assert.AreEqual(-1, tabPresenter.CurrentIndex);
    }

    [TestMethod]
    public async Task Closable_Event_Removes_View_Model()
    {
        using var context = await MvvmTestContext.CreateAsync();

        var viewModelTestContext = new ViewModelTestContext([]);

        var tabPresenter = (TabPresenter)context.ServiceProvider.GetRequiredService<ITabPresenter>();
        var viewModel = tabPresenter.Add<TestViewModel1>(viewModelTestContext);

        tabPresenter.SetCurrent(viewModel);

        Assert.IsNotNull(tabPresenter.Current);

        viewModel.Close();

        Assert.IsNull(tabPresenter.Current);
    }

    [TestMethod]
    public async Task Nested_Presenter_Deactivates_When_Parent_Deactivates()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var tabPresenter = (TabPresenter)context.ServiceProvider.GetRequiredService<ITabPresenter>();

        var parentViewModel = tabPresenter.Add<ParentViewModel>();

        tabPresenter.SetCurrent(parentViewModel);

        Assert.IsTrue(parentViewModel.IsActive);
        Assert.IsTrue(parentViewModel.Child?.IsActive);
        Assert.IsTrue(parentViewModel.Child?.Example?.IsActive);

        parentViewModel.Deactivate();

        Assert.IsFalse(parentViewModel.IsActive);
        Assert.IsFalse(parentViewModel.Child?.IsActive);
        Assert.IsFalse(parentViewModel.Child?.Example?.IsActive);

        parentViewModel.Activate();

        Assert.IsTrue(parentViewModel.IsActive);
        Assert.IsTrue(parentViewModel.Child?.IsActive);
        Assert.IsTrue(parentViewModel.Child?.Example?.IsActive);
    }

    [TestMethod]
    public async Task AddAndSetCurrent_Makes_New_ViewModel_Current()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var tabPresenter = (TabPresenter)context.ServiceProvider.GetRequiredService<ITabPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);

        Assert.IsNull(tabPresenter.Current);
        Assert.AreEqual(-1, tabPresenter.CurrentIndex);

        tabPresenter.AddAndSetCurrent<TestViewModel1>(viewModelContext);

        Assert.IsInstanceOfType<TestViewModel1>(tabPresenter.Current?.ViewModel);
        Assert.AreEqual(0, tabPresenter.CurrentIndex);

        tabPresenter.AddAndSetCurrent<TestViewModel2>(viewModelContext);

        Assert.IsInstanceOfType<TestViewModel2>(tabPresenter.Current?.ViewModel);
        Assert.AreEqual(1, tabPresenter.CurrentIndex);

        tabPresenter.AddAndSetCurrent<TestViewModel3>(viewModelContext);

        Assert.IsInstanceOfType<TestViewModel3>(tabPresenter.Current?.ViewModel);
        Assert.AreEqual(2, tabPresenter.CurrentIndex);
    }

    [TestMethod]
    public async Task Changed_Is_Observed_When_Changes_Occur()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var tabPresenter = (TabPresenter)context.ServiceProvider.GetRequiredService<ITabPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);

        var observedCount = 0;
        tabPresenter.Changed
            .Observe()
            .OnEach(() => observedCount++)
            .Until(TestContext.CancellationToken);

        tabPresenter.Add<TestViewModel1>(viewModelContext);
        Assert.AreEqual(1, observedCount);

        observedCount = 0;

        tabPresenter.Add<TestViewModel2>(viewModelContext);
        Assert.AreEqual(1, observedCount);

        observedCount = 0;

        tabPresenter.SetCurrentIndex(1);
        Assert.AreEqual(1, observedCount);

        observedCount = 0;

        tabPresenter.SetCurrentIndex(1);
        Assert.AreEqual(0, observedCount);

        observedCount = 0;

        tabPresenter.Remove(0);
        Assert.AreEqual(1, observedCount);

        observedCount = 0;

        tabPresenter.Clear();
        Assert.AreEqual(1, observedCount);

        observedCount = 0;

        tabPresenter.Clear();
        Assert.AreEqual(0, observedCount);
    }

    [TestMethod]
    public async Task Move_To_Same_Index_Has_No_Effect()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var tabPresenter = (TabPresenter)context.ServiceProvider.GetRequiredService<ITabPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);

        tabPresenter.Add<TestViewModel1>(viewModelContext);
        tabPresenter.Add<TestViewModel2>(viewModelContext);
        tabPresenter.AddAndSetCurrent<TestViewModel3>(viewModelContext);

        var observedChangedCount = 0;
        tabPresenter.Changed
            .Observe()
            .OnEach(() => observedChangedCount++)
            .Until(TestContext.CancellationToken);

        var observedCurrentIndexChanged = new List<int>();
        tabPresenter.CurrentIndexChanged
            .Observe()
            .OnEach(observedCurrentIndexChanged.Add)
            .Until(TestContext.CancellationToken);

        var observedCurrentChanged = new List<CurrentChangedEventArgs>();
        tabPresenter.CurrentChanged
            .Observe()
            .OnEach(observedCurrentChanged.Add)
            .Until(TestContext.CancellationToken);

        tabPresenter.Move(0, 0);
        Assert.AreEqual(0, observedChangedCount);
        Assert.IsEmpty(observedCurrentIndexChanged);
        Assert.IsEmpty(observedCurrentChanged);

        tabPresenter.MoveAndSetCurrent(2, 2);
        Assert.AreEqual(0, observedChangedCount);
        Assert.IsEmpty(observedCurrentIndexChanged);
        Assert.IsEmpty(observedCurrentChanged);

        Assert.AreEqual(2, tabPresenter.CurrentIndex);
    }

    [TestMethod]
    public async Task MoveAndSetCurrent_To_Same_Index_Only_Changes_Current()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var tabPresenter = (TabPresenter)context.ServiceProvider.GetRequiredService<ITabPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);

        var viewModel1 = tabPresenter.Add<TestViewModel1>(viewModelContext);
        tabPresenter.Add<TestViewModel2>(viewModelContext);
        tabPresenter.AddAndSetCurrent<TestViewModel3>(viewModelContext);

        var observedChangedCount = 0;
        tabPresenter.Changed
            .Observe()
            .OnEach(() => observedChangedCount++)
            .Until(TestContext.CancellationToken);

        var observedCurrentIndexChanged = new List<int>();
        tabPresenter.CurrentIndexChanged
            .Observe()
            .OnEach(observedCurrentIndexChanged.Add)
            .Until(TestContext.CancellationToken);

        var observedCurrentChanged = new List<CurrentChangedEventArgs>();
        tabPresenter.CurrentChanged
            .Observe()
            .OnEach(observedCurrentChanged.Add)
            .Until(TestContext.CancellationToken);

        tabPresenter.MoveAndSetCurrent(0, 0);

        Assert.AreEqual(1, observedChangedCount);

        Assert.HasCount(1, observedCurrentIndexChanged);
        Assert.AreEqual(0, observedCurrentIndexChanged[0]);

        Assert.HasCount(1, observedCurrentChanged);
        Assert.AreEqual(viewModel1, observedCurrentChanged[0].View?.ViewModel);

        Assert.AreEqual(0, tabPresenter.CurrentIndex);
        Assert.AreEqual(viewModel1, tabPresenter.Current?.ViewModel);
    }

    [TestMethod]
    public async Task MoveAndSetCurrent_Updates_And_Emits()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var tabPresenter = (TabPresenter)context.ServiceProvider.GetRequiredService<ITabPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);

        var viewModel1 = tabPresenter.Add<TestViewModel1>(viewModelContext);
        var viewModel2 = tabPresenter.AddAndSetCurrent<TestViewModel2>(viewModelContext);
        var viewModel3 = tabPresenter.Add<TestViewModel3>(viewModelContext);

        var observedChangedCount = 0;
        tabPresenter.Changed
            .Observe()
            .OnEach(() => observedChangedCount++)
            .Until(TestContext.CancellationToken);

        var observedCurrentIndexChanged = new List<int>();
        tabPresenter.CurrentIndexChanged
            .Observe()
            .OnEach(observedCurrentIndexChanged.Add)
            .Until(TestContext.CancellationToken);

        var observedCurrentChanged = new List<CurrentChangedEventArgs>();
        tabPresenter.CurrentChanged
            .Observe()
            .OnEach(observedCurrentChanged.Add)
            .Until(TestContext.CancellationToken);

        tabPresenter.MoveAndSetCurrent(0, 2);

        Assert.AreEqual(viewModel2, tabPresenter.ViewModels[0]);
        Assert.AreEqual(viewModel3, tabPresenter.ViewModels[1]);
        Assert.AreEqual(viewModel1, tabPresenter.ViewModels[2]);

        Assert.AreEqual(2, tabPresenter.CurrentIndex);
        Assert.AreEqual(viewModel1, tabPresenter.Current?.ViewModel);

        Assert.AreEqual(1, observedChangedCount);

        Assert.HasCount(1, observedCurrentIndexChanged);
        Assert.AreEqual(2, observedCurrentIndexChanged[0]);

        Assert.HasCount(1, observedCurrentChanged);
        Assert.AreEqual(viewModel1, tabPresenter.Current?.ViewModel);
    }

    [TestMethod]
    public async Task Move_From_Before_Current_To_After_Current()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var tabPresenter = (TabPresenter)context.ServiceProvider.GetRequiredService<ITabPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);

        var viewModel1 = tabPresenter.Add<TestViewModel1>(viewModelContext);
        var viewModel2 = tabPresenter.AddAndSetCurrent<TestViewModel2>(viewModelContext);
        var viewModel3 = tabPresenter.Add<TestViewModel3>(viewModelContext);

        var observedChangedCount = 0;
        tabPresenter.Changed
            .Observe()
            .OnEach(() => observedChangedCount++)
            .Until(TestContext.CancellationToken);

        var observedCurrentIndexChanged = new List<int>();
        tabPresenter.CurrentIndexChanged
            .Observe()
            .OnEach(observedCurrentIndexChanged.Add)
            .Until(TestContext.CancellationToken);

        var observedCurrentChanged = new List<CurrentChangedEventArgs>();
        tabPresenter.CurrentChanged
            .Observe()
            .OnEach(observedCurrentChanged.Add)
            .Until(TestContext.CancellationToken);

        tabPresenter.Move(0, 2);

        Assert.AreEqual(viewModel2, tabPresenter.ViewModels[0]);
        Assert.AreEqual(viewModel3, tabPresenter.ViewModels[1]);
        Assert.AreEqual(viewModel1, tabPresenter.ViewModels[2]);

        Assert.AreEqual(0, tabPresenter.CurrentIndex);
        Assert.AreEqual(viewModel2, tabPresenter.Current?.ViewModel);

        Assert.AreEqual(1, observedChangedCount);

        Assert.HasCount(1, observedCurrentIndexChanged);
        Assert.AreEqual(0, observedCurrentIndexChanged[0]);

        Assert.IsEmpty(observedCurrentChanged);
    }

    [TestMethod]
    public async Task Move_From_After_Current_To_Before_Current()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var tabPresenter = (TabPresenter)context.ServiceProvider.GetRequiredService<ITabPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);

        var viewModel1 = tabPresenter.Add<TestViewModel1>(viewModelContext);
        var viewModel2 = tabPresenter.AddAndSetCurrent<TestViewModel2>(viewModelContext);
        var viewModel3 = tabPresenter.Add<TestViewModel3>(viewModelContext);

        var observedChangedCount = 0;
        tabPresenter.Changed
            .Observe()
            .OnEach(() => observedChangedCount++)
            .Until(TestContext.CancellationToken);

        var observedCurrentIndexChanged = new List<int>();
        tabPresenter.CurrentIndexChanged
            .Observe()
            .OnEach(observedCurrentIndexChanged.Add)
            .Until(TestContext.CancellationToken);

        var observedCurrentChanged = new List<CurrentChangedEventArgs>();
        tabPresenter.CurrentChanged
            .Observe()
            .OnEach(observedCurrentChanged.Add)
            .Until(TestContext.CancellationToken);

        tabPresenter.Move(2, 0);

        Assert.AreEqual(viewModel3, tabPresenter.ViewModels[0]);
        Assert.AreEqual(viewModel1, tabPresenter.ViewModels[1]);
        Assert.AreEqual(viewModel2, tabPresenter.ViewModels[2]);

        Assert.AreEqual(2, tabPresenter.CurrentIndex);
        Assert.AreEqual(viewModel2, tabPresenter.Current?.ViewModel);

        Assert.AreEqual(1, observedChangedCount);

        Assert.HasCount(1, observedCurrentIndexChanged);
        Assert.AreEqual(2, observedCurrentIndexChanged[0]);

        Assert.IsEmpty(observedCurrentChanged);
    }

    [TestMethod]
    public async Task Move_From_Before_Current_To_Before_Current()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var tabPresenter = (TabPresenter)context.ServiceProvider.GetRequiredService<ITabPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);

        var viewModel1 = tabPresenter.Add<TestViewModel1>(viewModelContext);
        var viewModel2 = tabPresenter.Add<TestViewModel2>(viewModelContext);
        var viewModel3 = tabPresenter.AddAndSetCurrent<TestViewModel3>(viewModelContext);
        var viewModel4 = tabPresenter.Add<TestViewModel3>(viewModelContext);

        var observedChangedCount = 0;
        tabPresenter.Changed
            .Observe()
            .OnEach(() => observedChangedCount++)
            .Until(TestContext.CancellationToken);

        var observedCurrentIndexChanged = new List<int>();
        tabPresenter.CurrentIndexChanged
            .Observe()
            .OnEach(observedCurrentIndexChanged.Add)
            .Until(TestContext.CancellationToken);

        var observedCurrentChanged = new List<CurrentChangedEventArgs>();
        tabPresenter.CurrentChanged
            .Observe()
            .OnEach(observedCurrentChanged.Add)
            .Until(TestContext.CancellationToken);

        tabPresenter.Move(0, 1);

        Assert.AreEqual(viewModel2, tabPresenter.ViewModels[0]);
        Assert.AreEqual(viewModel1, tabPresenter.ViewModels[1]);
        Assert.AreEqual(viewModel3, tabPresenter.ViewModels[2]);
        Assert.AreEqual(viewModel4, tabPresenter.ViewModels[3]);

        Assert.AreEqual(2, tabPresenter.CurrentIndex);
        Assert.AreEqual(viewModel3, tabPresenter.Current?.ViewModel);

        Assert.AreEqual(1, observedChangedCount);

        Assert.IsEmpty(observedCurrentIndexChanged);

        Assert.IsEmpty(observedCurrentChanged);
    }

    [TestMethod]
    public async Task Move_From_After_Current_To_After_Current()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var tabPresenter = (TabPresenter)context.ServiceProvider.GetRequiredService<ITabPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);

        var viewModel1 = tabPresenter.Add<TestViewModel1>(viewModelContext);
        var viewModel2 = tabPresenter.AddAndSetCurrent<TestViewModel2>(viewModelContext);
        var viewModel3 = tabPresenter.Add<TestViewModel3>(viewModelContext);
        var viewModel4 = tabPresenter.Add<TestViewModel3>(viewModelContext);

        var observedChangedCount = 0;
        tabPresenter.Changed
            .Observe()
            .OnEach(() => observedChangedCount++)
            .Until(TestContext.CancellationToken);

        var observedCurrentIndexChanged = new List<int>();
        tabPresenter.CurrentIndexChanged
            .Observe()
            .OnEach(observedCurrentIndexChanged.Add)
            .Until(TestContext.CancellationToken);

        var observedCurrentChanged = new List<CurrentChangedEventArgs>();
        tabPresenter.CurrentChanged
            .Observe()
            .OnEach(observedCurrentChanged.Add)
            .Until(TestContext.CancellationToken);

        tabPresenter.Move(2, 3);

        Assert.AreEqual(viewModel1, tabPresenter.ViewModels[0]);
        Assert.AreEqual(viewModel2, tabPresenter.ViewModels[1]);
        Assert.AreEqual(viewModel4, tabPresenter.ViewModels[2]);
        Assert.AreEqual(viewModel3, tabPresenter.ViewModels[3]);

        Assert.AreEqual(1, tabPresenter.CurrentIndex);
        Assert.AreEqual(viewModel2, tabPresenter.Current?.ViewModel);

        Assert.AreEqual(1, observedChangedCount);

        Assert.IsEmpty(observedCurrentIndexChanged);

        Assert.IsEmpty(observedCurrentChanged);
    }

    [TestMethod]
    public async Task Remove_Index_Updates_ViewModels()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var tabPresenter = (TabPresenter)context.ServiceProvider.GetRequiredService<ITabPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);

        var viewModel1 = tabPresenter.Add<TestViewModel1>(viewModelContext);
        var viewModel2 = tabPresenter.AddAndSetCurrent<TestViewModel2>(viewModelContext);
        var viewModel3 = tabPresenter.Add<TestViewModel3>(viewModelContext);

        tabPresenter.Remove(1);

        Assert.AreEqual(-1, tabPresenter.CurrentIndex);
        Assert.IsNull(tabPresenter.Current);

        Assert.HasCount(2, tabPresenter.ViewModels);
        Assert.AreEqual(viewModel1, tabPresenter.ViewModels[0]);
        Assert.AreEqual(viewModel3, tabPresenter.ViewModels[1]);
    }

    [TestMethod]
    public async Task Remove_ViewModel_Updates_ViewModels()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var tabPresenter = (TabPresenter)context.ServiceProvider.GetRequiredService<ITabPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);

        var viewModel1 = tabPresenter.Add<TestViewModel1>(viewModelContext);
        var viewModel2 = tabPresenter.AddAndSetCurrent<TestViewModel2>(viewModelContext);
        var viewModel3 = tabPresenter.Add<TestViewModel3>(viewModelContext);

        tabPresenter.Remove(viewModel2);

        Assert.AreEqual(-1, tabPresenter.CurrentIndex);
        Assert.IsNull(tabPresenter.Current);

        Assert.HasCount(2, tabPresenter.ViewModels);
        Assert.AreEqual(viewModel1, tabPresenter.ViewModels[0]);
        Assert.AreEqual(viewModel3, tabPresenter.ViewModels[1]);
    }

    [TestMethod]
    public async Task RemoveAndSetCurernt_ViewModel_SetIndex_Below()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var tabPresenter = (TabPresenter)context.ServiceProvider.GetRequiredService<ITabPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);

        var viewModel1 = tabPresenter.Add<TestViewModel1>(viewModelContext);
        var viewModel2 = tabPresenter.AddAndSetCurrent<TestViewModel2>(viewModelContext);
        var viewModel3 = tabPresenter.Add<TestViewModel3>(viewModelContext);

        tabPresenter.RemoveAndSetCurrent(viewModel2);

        Assert.AreEqual(0, tabPresenter.CurrentIndex);
        Assert.AreEqual(viewModel1, tabPresenter.Current?.ViewModel);

        Assert.HasCount(2, tabPresenter.ViewModels);
        Assert.AreEqual(viewModel1, tabPresenter.ViewModels[0]);
        Assert.AreEqual(viewModel3, tabPresenter.ViewModels[1]);
    }

    [TestMethod]
    public async Task RemoveAndSetCurernt_ViewModel_SetIndex_Above()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var tabPresenter = (TabPresenter)context.ServiceProvider.GetRequiredService<ITabPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);

        var viewModel1 = tabPresenter.AddAndSetCurrent<TestViewModel1>(viewModelContext);
        var viewModel2 = tabPresenter.Add<TestViewModel2>(viewModelContext);
        var viewModel3 = tabPresenter.Add<TestViewModel3>(viewModelContext);

        tabPresenter.RemoveAndSetCurrent(viewModel1);

        Assert.AreEqual(0, tabPresenter.CurrentIndex);
        Assert.AreEqual(viewModel2, tabPresenter.Current?.ViewModel);

        Assert.HasCount(2, tabPresenter.ViewModels);
        Assert.AreEqual(viewModel2, tabPresenter.ViewModels[0]);
        Assert.AreEqual(viewModel3, tabPresenter.ViewModels[1]);
    }

    [TestMethod]
    public async Task RemoveAndSetCurernt_ViewModel_SetIndex_Empty()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var tabPresenter = (TabPresenter)context.ServiceProvider.GetRequiredService<ITabPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);

        var viewModel1 = tabPresenter.AddAndSetCurrent<TestViewModel1>(viewModelContext);

        tabPresenter.RemoveAndSetCurrent(viewModel1);

        Assert.AreEqual(-1, tabPresenter.CurrentIndex);
        Assert.IsNull(tabPresenter.Current);
        Assert.IsEmpty(tabPresenter.ViewModels);
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
