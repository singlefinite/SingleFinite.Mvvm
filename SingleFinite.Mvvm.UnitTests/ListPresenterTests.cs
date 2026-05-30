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

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class ListPresenterTests
{
    [TestMethod]
    public void Lifecycle_Events_Raised_When_Expected()
    {
        using var context = new MvvmTestContext();
        var listPresenter = (ListPresenter)context.ServiceProvider.GetRequiredService<IListPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2>(viewModelContext);
        var viewModelDescriptor3 = new ViewModelDescriptor<TestViewModel3>(viewModelContext);

        listPresenter.Add(viewModelDescriptor1);
        Assert.HasCount(1, output);
        Assert.AreEqual("OnInit - TestViewModel1", output[0]);

        listPresenter.SetCurrentIndex(0);
        Assert.HasCount(2, output);
        Assert.AreEqual("OnStart - TestViewModel1", output[1]);

        output.Clear();

        listPresenter.AddAll([viewModelDescriptor2, viewModelDescriptor3]);
        Assert.HasCount(2, output);
        Assert.AreEqual("OnInit - TestViewModel2", output[0]);
        Assert.AreEqual("OnInit - TestViewModel3", output[1]);

        listPresenter.SetCurrentIndex(2);
        Assert.HasCount(4, output);
        Assert.AreEqual("OnStop - TestViewModel1", output[2]);
        Assert.AreEqual("OnStart - TestViewModel3", output[3]);

        output.Clear();

        listPresenter.Remove(2);
        Assert.HasCount(2, output);
        Assert.AreEqual("OnStop - TestViewModel3", output[0]);
        Assert.AreEqual("OnDispose - TestViewModel3", output[1]);

        listPresenter.SetCurrentIndex(1);
        Assert.HasCount(3, output);
        Assert.AreEqual("OnStart - TestViewModel2", output[2]);

        output.Clear();

        listPresenter.Clear();
        Assert.HasCount(3, output);
        Assert.AreEqual("OnStop - TestViewModel2", output[0]);
        Assert.AreEqual("OnDispose - TestViewModel1", output[1]);
        Assert.AreEqual("OnDispose - TestViewModel2", output[2]);
    }

    [TestMethod]
    public void SetCurrent_With_Type()
    {
        using var context = new MvvmTestContext();
        var listPresenter = (ListPresenter)context.ServiceProvider.GetRequiredService<IListPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2>(viewModelContext);

        listPresenter.AddAll([viewModelDescriptor1, viewModelDescriptor2]);

        Assert.IsNull(listPresenter.Current);
        Assert.AreEqual(-1, listPresenter.CurrentIndex);

        listPresenter.SetCurrent<TestViewModel1>();

        Assert.IsInstanceOfType<TestViewModel1>(listPresenter.Current?.ViewModel);
        Assert.AreEqual(0, listPresenter.CurrentIndex);

        listPresenter.SetCurrent<TestViewModel2>();

        Assert.IsInstanceOfType<TestViewModel2>(listPresenter.Current?.ViewModel);
        Assert.AreEqual(1, listPresenter.CurrentIndex);

        listPresenter.SetCurrent<TestViewModel3>();

        Assert.IsNull(listPresenter.Current);
        Assert.AreEqual(-1, listPresenter.CurrentIndex);
    }

    [TestMethod]
    public void Closable_Event_Removes_View_Model()
    {
        using var context = new MvvmTestContext();

        var viewModelTestContext = new ViewModelTestContext([]);

        var listPresenter = (ListPresenter)context.ServiceProvider.GetRequiredService<IListPresenter>();
        var viewModel = listPresenter.Add<TestViewModel1>(viewModelTestContext);

        listPresenter.SetCurrent(viewModel);

        Assert.IsNotNull(listPresenter.Current);

        viewModel.Close();

        Assert.IsNull(listPresenter.Current);
    }

    [TestMethod]
    public void Nested_Presenter_Deactivates_When_Parent_Deactivates()
    {
        using var context = new MvvmTestContext();
        var listPresenter = (ListPresenter)context.ServiceProvider.GetRequiredService<IListPresenter>();

        var parentViewModel = listPresenter.Add<ParentViewModel>();

        listPresenter.SetCurrent(parentViewModel);

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

    private class ParentViewModel(IListPresenter presenter) : ViewModel
    {
        public ChildViewModel? Child { get; private set; }

        protected override void OnCreated()
        {
            Child = presenter.Add<ChildViewModel>();
            presenter.SetCurrent(Child);
        }
    }

    private class ChildViewModel(IListPresenter presenter) : ViewModel
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
