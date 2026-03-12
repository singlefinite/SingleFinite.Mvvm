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
public class StackPresenterTests
{
    [TestMethod]
    public void Lifecycle_Events_Raised_When_Expected()
    {
        using var context = new MvvmTestContext();
        var stackPresenter = (StackPresenter)context.ServiceProvider.GetRequiredService<IStackPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2>(viewModelContext);
        var viewModelDescriptor3 = new ViewModelDescriptor<TestViewModel3>(viewModelContext);

        stackPresenter.Push(viewModelDescriptor1);
        Assert.HasCount(2, output);
        Assert.AreEqual("OnInit - TestViewModel1", output[0]);
        Assert.AreEqual("OnStart - TestViewModel1", output[1]);

        output.Clear();
        stackPresenter.PushAll([viewModelDescriptor2, viewModelDescriptor3]);
        Assert.HasCount(4, output);
        Assert.AreEqual("OnInit - TestViewModel2", output[0]);
        Assert.AreEqual("OnInit - TestViewModel3", output[1]);
        Assert.AreEqual("OnStop - TestViewModel1", output[2]);
        Assert.AreEqual("OnStart - TestViewModel3", output[3]);

        output.Clear();
        stackPresenter.PopTo<TestViewModel2>();
        Assert.HasCount(3, output);
        Assert.AreEqual("OnStop - TestViewModel3", output[0]);
        Assert.AreEqual("OnDispose - TestViewModel3", output[1]);
        Assert.AreEqual("OnStart - TestViewModel2", output[2]);

        output.Clear();
        stackPresenter.Clear();
        Assert.HasCount(3, output);
        Assert.AreEqual("OnStop - TestViewModel2", output[0]);
        Assert.AreEqual("OnDispose - TestViewModel2", output[1]);
        Assert.AreEqual("OnDispose - TestViewModel1", output[2]);
    }

    [TestMethod]
    public void Add_To_Middle_Of_Stack()
    {
        using var context = new MvvmTestContext();
        var stackPresenter = (StackPresenter)context.ServiceProvider.GetRequiredService<IStackPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2>(viewModelContext);
        var viewModelDescriptor3 = new ViewModelDescriptor<TestViewModel3>(viewModelContext);

        stackPresenter.PushAll([viewModelDescriptor1, viewModelDescriptor2]);
        Assert.HasCount(3, output);
        Assert.AreEqual("OnInit - TestViewModel1", output[0]);
        Assert.AreEqual("OnInit - TestViewModel2", output[1]);
        Assert.AreEqual("OnStart - TestViewModel2", output[2]);

        output.Clear();
        stackPresenter.Add(1, viewModelDescriptor3);
        Assert.HasCount(1, output);
        Assert.AreEqual("OnInit - TestViewModel3", output[0]);

        Assert.IsTrue(stackPresenter.Current?.ViewModel is TestViewModel2);
    }

    [TestMethod]
    public void Changed_Event_Is_Raised()
    {
        using var context = new MvvmTestContext();
        var stackPresenter = (StackPresenter)context.ServiceProvider.GetRequiredService<IStackPresenter>();

        IPresenter.CurrentChangedEventArgs? observedArgs = null;
        stackPresenter.CurrentChanged
            .Observe()
            .OnEach(args => observedArgs = args);

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2>(viewModelContext);

        Assert.IsNull(observedArgs);

        var viewModel1 = stackPresenter.Push(viewModelDescriptor1);

        Assert.IsNotNull(observedArgs);
        Assert.AreEqual(viewModel1, observedArgs.View?.ViewModel);
        Assert.IsTrue(observedArgs.IsNew);

        observedArgs = null;

        var viewModel2 = stackPresenter.Push(viewModelDescriptor2);

        Assert.IsNotNull(observedArgs);
        Assert.AreEqual(viewModel2, observedArgs.View?.ViewModel);
        Assert.IsTrue(observedArgs.IsNew);

        observedArgs = null;

        stackPresenter.Pop();

        Assert.IsNotNull(observedArgs);
        Assert.AreEqual(viewModel1, observedArgs.View?.ViewModel);
        Assert.IsFalse(observedArgs.IsNew);
    }

    [TestMethod]
    public void PopTo_Method_With_ViewModel_Type_Pops_Views()
    {
        using var context = new MvvmTestContext();
        var stackPresenter = (StackPresenter)context.ServiceProvider.GetRequiredService<IStackPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2>(viewModelContext);
        var viewModelDescriptor3 = new ViewModelDescriptor<TestViewModel3>(viewModelContext);

        stackPresenter.PushAll([viewModelDescriptor1, viewModelDescriptor2, viewModelDescriptor3]);
        var result1 = stackPresenter.PopTo<TestViewModel1>(inclusive: true);
        Assert.IsTrue(result1);
        Assert.IsEmpty(stackPresenter.ViewModels);
        Assert.IsNull(stackPresenter.Current);

        stackPresenter.PushAll([viewModelDescriptor1, viewModelDescriptor2]);
        var result2 = stackPresenter.PopTo<TestViewModel3>(inclusive: true);
        Assert.IsFalse(result2);
        Assert.HasCount(2, stackPresenter.ViewModels);
        Assert.IsNotNull(stackPresenter.Current);

        var result3 = stackPresenter.PopTo<TestViewModel1>(inclusive: true);
        Assert.IsTrue(result3);
        Assert.IsEmpty(stackPresenter.ViewModels);
        Assert.IsNull(stackPresenter.Current);
    }

    [TestMethod]
    public void PopTo_Method_With_Query_Pops_Views()
    {
        using var context = new MvvmTestContext();
        var stackPresenter = (StackPresenter)context.ServiceProvider.GetRequiredService<IStackPresenter>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2>(viewModelContext);
        var viewModelDescriptor3 = new ViewModelDescriptor<TestViewModel3>(viewModelContext);

        stackPresenter.PushAll([viewModelDescriptor1, viewModelDescriptor2, viewModelDescriptor3, viewModelDescriptor1]);
        var result1 = stackPresenter.PopTo(
            predicate: viewModel => viewModel is TestViewModel1,
            fromTop: false,
            inclusive: true
        );
        Assert.IsTrue(result1);
        Assert.IsEmpty(stackPresenter.ViewModels);

        stackPresenter.PushAll([viewModelDescriptor1, viewModelDescriptor2, viewModelDescriptor3, viewModelDescriptor1]);
        var result2 = stackPresenter.PopTo(
            predicate: viewModel => viewModel is TestViewModel1,
            fromTop: true,
            inclusive: true
        );
        Assert.IsTrue(result1);
        Assert.HasCount(3, stackPresenter.ViewModels);
        Assert.AreEqual(typeof(TestViewModel3), stackPresenter.ViewModels[0].GetType());
        Assert.AreEqual(typeof(TestViewModel2), stackPresenter.ViewModels[1].GetType());
        Assert.AreEqual(typeof(TestViewModel1), stackPresenter.ViewModels[2].GetType());
    }

    [TestMethod]
    public void Presenter_Is_Disposed_When_ServiceScope_Is_Disposed()
    {
        using var context = new MvvmTestContext();
        var scope = context.ServiceProvider.CreateScope();
        var stackPresenterInScope = (StackPresenter)scope.ServiceProvider.GetRequiredService<IStackPresenter>();
        var stackPresenterInRoot = (StackPresenter)context.ServiceProvider.GetRequiredService<IStackPresenter>();

        var output = new List<string>();
        var viewModelTestContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1>(viewModelTestContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2>(viewModelTestContext);
        var viewModelDescriptor3 = new ViewModelDescriptor<TestViewModel3>(viewModelTestContext);

        stackPresenterInScope.PushAll([viewModelDescriptor1, viewModelDescriptor2, viewModelDescriptor3]);
        stackPresenterInRoot.PushAll([viewModelDescriptor1, viewModelDescriptor2, viewModelDescriptor3]);
        Assert.HasCount(3, stackPresenterInScope.ViewModels);
        Assert.HasCount(3, stackPresenterInRoot.ViewModels);

        scope.Dispose();
        Assert.IsEmpty(stackPresenterInScope.ViewModels);
        Assert.HasCount(3, stackPresenterInRoot.ViewModels);
    }

    [TestMethod]
    public void Stack_Property_Is_List_Ordered_From_Stack_Top_To_Stack_Bottom()
    {
        using var context = new MvvmTestContext();

        var output = new List<string>();
        var viewModelTestContext = new ViewModelTestContext(output);

        var stackPresenter = (StackPresenter)context.ServiceProvider.GetRequiredService<IStackPresenter>();
        stackPresenter.PushAll(
            [
                new ViewModelDescriptor<TestViewModel1>(viewModelTestContext),
                new ViewModelDescriptor<TestViewModel2>(viewModelTestContext),
                new ViewModelDescriptor<TestViewModel3>(viewModelTestContext)
            ]
        );

        Assert.HasCount(3, stackPresenter.ViewModels);
        Assert.IsInstanceOfType<TestViewModel3>(stackPresenter.ViewModels[0]);
        Assert.IsInstanceOfType<TestViewModel2>(stackPresenter.ViewModels[1]);
        Assert.IsInstanceOfType<TestViewModel1>(stackPresenter.ViewModels[2]);
    }

    [TestMethod]
    public void Push_Method_With_Template_Returns_View_With_Template()
    {
        using var context = new MvvmTestContext();

        var viewModelTestContext = new ViewModelTestContext([]);

        var stackPresenter = (StackPresenter)context.ServiceProvider.GetRequiredService<IStackPresenter>();
        var viewModel = stackPresenter.Push<TestViewModel1>(viewModelTestContext);

        Assert.IsNotNull(viewModel);
    }

    [TestMethod]
    public void Closable_Event_Removes_View_Model()
    {
        using var context = new MvvmTestContext();

        var viewModelTestContext = new ViewModelTestContext([]);

        var stackPresenter = (StackPresenter)context.ServiceProvider.GetRequiredService<IStackPresenter>();
        var viewModel = stackPresenter.Push<TestViewModel1>(viewModelTestContext);

        Assert.IsNotNull(stackPresenter.Current);

        viewModel.Close();

        Assert.IsNull(stackPresenter.Current);
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
