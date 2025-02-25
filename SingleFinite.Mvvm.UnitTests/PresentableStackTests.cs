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

using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using SingleFinite.Mvvm.Internal.Services;
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class PresentableStackTests
{
    [TestMethod]
    public void Lifecycle_Events_Raised_When_Expected()
    {
        using var context = new TestContext();
        var presentableStack = (PresentableStack)context.ServiceProvider.GetRequiredService<IPresentableStack>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1, ViewModelTestContext>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2, ViewModelTestContext>(viewModelContext);
        var viewModelDescriptor3 = new ViewModelDescriptor<TestViewModel3, ViewModelTestContext>(viewModelContext);

        presentableStack.Push(viewModelDescriptor1);
        Assert.AreEqual(2, output.Count);
        Assert.AreEqual("OnInit - TestViewModel1", output[0]);
        Assert.AreEqual("OnStart - TestViewModel1", output[1]);

        output.Clear();
        presentableStack.PushAll([viewModelDescriptor2, viewModelDescriptor3]);
        Assert.AreEqual(4, output.Count);
        Assert.AreEqual("OnInit - TestViewModel2", output[0]);
        Assert.AreEqual("OnInit - TestViewModel3", output[1]);
        Assert.AreEqual("OnStop - TestViewModel1", output[2]);
        Assert.AreEqual("OnStart - TestViewModel3", output[3]);

        output.Clear();
        presentableStack.PopTo<TestViewModel2>();
        Assert.AreEqual(3, output.Count);
        Assert.AreEqual("OnStop - TestViewModel3", output[0]);
        Assert.AreEqual("OnDispose - TestViewModel3", output[1]);
        Assert.AreEqual("OnStart - TestViewModel2", output[2]);

        output.Clear();
        presentableStack.Clear();
        Assert.AreEqual(3, output.Count);
        Assert.AreEqual("OnStop - TestViewModel2", output[0]);
        Assert.AreEqual("OnDispose - TestViewModel2", output[1]);
        Assert.AreEqual("OnDispose - TestViewModel1", output[2]);
    }

    [TestMethod]
    public void Add_To_Middle_Of_Stack()
    {
        using var context = new TestContext();
        var presentableStack = (PresentableStack)context.ServiceProvider.GetRequiredService<IPresentableStack>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1, ViewModelTestContext>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2, ViewModelTestContext>(viewModelContext);
        var viewModelDescriptor3 = new ViewModelDescriptor<TestViewModel3, ViewModelTestContext>(viewModelContext);

        presentableStack.PushAll([viewModelDescriptor1, viewModelDescriptor2]);
        Assert.AreEqual(3, output.Count);
        Assert.AreEqual("OnInit - TestViewModel1", output[0]);
        Assert.AreEqual("OnInit - TestViewModel2", output[1]);
        Assert.AreEqual("OnStart - TestViewModel2", output[2]);

        output.Clear();
        presentableStack.Add(1, viewModelDescriptor3);
        Assert.AreEqual(1, output.Count);
        Assert.AreEqual("OnInit - TestViewModel3", output[0]);

        Assert.IsTrue(presentableStack.Current?.ViewModel is TestViewModel2);
    }

    [TestMethod]
    public void Changed_Event_Is_Raised()
    {
        using var context = new TestContext();
        var presentableStack = (PresentableStack)context.ServiceProvider.GetRequiredService<IPresentableStack>();

        IPresentable.CurrentChangedEventArgs? observedArgs = null;
        presentableStack.CurrentChanged
            .Observe()
            .OnEach(args => observedArgs = args);

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1, ViewModelTestContext>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2, ViewModelTestContext>(viewModelContext);

        Assert.IsNull(observedArgs);

        var viewModel1 = presentableStack.Push(viewModelDescriptor1);

        Assert.IsNotNull(observedArgs);
        Assert.AreEqual(viewModel1, observedArgs.View?.ViewModel);
        Assert.IsTrue(observedArgs.IsNew);

        observedArgs = null;

        var viewModel2 = presentableStack.Push(viewModelDescriptor2);

        Assert.IsNotNull(observedArgs);
        Assert.AreEqual(viewModel2, observedArgs.View?.ViewModel);
        Assert.IsTrue(observedArgs.IsNew);

        observedArgs = null;

        presentableStack.Pop();

        Assert.IsNotNull(observedArgs);
        Assert.AreEqual(viewModel1, observedArgs.View?.ViewModel);
        Assert.IsFalse(observedArgs.IsNew);
    }

    [TestMethod]
    public void PopTo_Method_With_ViewModel_Type_Pops_Views()
    {
        using var context = new TestContext();
        var presentableStack = (PresentableStack)context.ServiceProvider.GetRequiredService<IPresentableStack>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1, ViewModelTestContext>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2, ViewModelTestContext>(viewModelContext);
        var viewModelDescriptor3 = new ViewModelDescriptor<TestViewModel3, ViewModelTestContext>(viewModelContext);

        presentableStack.PushAll([viewModelDescriptor1, viewModelDescriptor2, viewModelDescriptor3]);
        var result1 = presentableStack.PopTo<TestViewModel1>(inclusive: true);
        Assert.AreEqual(true, result1);
        Assert.AreEqual(0, presentableStack.ViewModels.Length);
        Assert.AreEqual(null, presentableStack.Current);

        presentableStack.PushAll([viewModelDescriptor1, viewModelDescriptor2]);
        var result2 = presentableStack.PopTo<TestViewModel3>(inclusive: true);
        Assert.AreEqual(false, result2);
        Assert.AreEqual(2, presentableStack.ViewModels.Length);
        Assert.IsNotNull(presentableStack.Current);

        var result3 = presentableStack.PopTo<TestViewModel1>(inclusive: true);
        Assert.AreEqual(true, result3);
        Assert.AreEqual(0, presentableStack.ViewModels.Length);
        Assert.AreEqual(null, presentableStack.Current);
    }

    [TestMethod]
    public void PopTo_Method_With_Query_Pops_Views()
    {
        using var context = new TestContext();
        var presentableStack = (PresentableStack)context.ServiceProvider.GetRequiredService<IPresentableStack>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1, ViewModelTestContext>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2, ViewModelTestContext>(viewModelContext);
        var viewModelDescriptor3 = new ViewModelDescriptor<TestViewModel3, ViewModelTestContext>(viewModelContext);

        presentableStack.PushAll([viewModelDescriptor1, viewModelDescriptor2, viewModelDescriptor3, viewModelDescriptor1]);
        var result1 = presentableStack.PopTo(
            predicate: viewModel => viewModel is TestViewModel1,
            fromTop: false,
            inclusive: true
        );
        Assert.AreEqual(true, result1);
        Assert.AreEqual(0, presentableStack.ViewModels.Length);

        presentableStack.PushAll([viewModelDescriptor1, viewModelDescriptor2, viewModelDescriptor3, viewModelDescriptor1]);
        var result2 = presentableStack.PopTo(
            predicate: viewModel => viewModel is TestViewModel1,
            fromTop: true,
            inclusive: true
        );
        Assert.AreEqual(true, result1);
        Assert.AreEqual(3, presentableStack.ViewModels.Length);
        Assert.AreEqual(presentableStack.ViewModels[0].GetType(), typeof(TestViewModel3));
        Assert.AreEqual(presentableStack.ViewModels[1].GetType(), typeof(TestViewModel2));
        Assert.AreEqual(presentableStack.ViewModels[2].GetType(), typeof(TestViewModel1));
    }

    [TestMethod]
    public void Presenter_Is_Disposed_When_ServiceScope_Is_Disposed()
    {
        using var context = new TestContext();
        var scope = context.ServiceProvider.CreateScope();
        var presentableStackInScope = (PresentableStack)scope.ServiceProvider.GetRequiredService<IPresentableStack>();
        var presentableStackInRoot = (PresentableStack)context.ServiceProvider.GetRequiredService<IPresentableStack>();

        var output = new List<string>();
        var viewModelTestContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1, ViewModelTestContext>(viewModelTestContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2, ViewModelTestContext>(viewModelTestContext);
        var viewModelDescriptor3 = new ViewModelDescriptor<TestViewModel3, ViewModelTestContext>(viewModelTestContext);

        presentableStackInScope.PushAll([viewModelDescriptor1, viewModelDescriptor2, viewModelDescriptor3]);
        presentableStackInRoot.PushAll([viewModelDescriptor1, viewModelDescriptor2, viewModelDescriptor3]);
        Assert.AreEqual(3, presentableStackInScope.ViewModels.Length);
        Assert.AreEqual(3, presentableStackInRoot.ViewModels.Length);

        scope.Dispose();
        Assert.AreEqual(0, presentableStackInScope.ViewModels.Length);
        Assert.AreEqual(3, presentableStackInRoot.ViewModels.Length);
    }

    [TestMethod]
    public void Stack_Property_Is_List_Ordered_From_Stack_Top_To_Stack_Bottom()
    {
        using var context = new TestContext();

        var output = new List<string>();
        var viewModelTestContext = new ViewModelTestContext(output);

        var presentableStack = (PresentableStack)context.ServiceProvider.GetRequiredService<IPresentableStack>();
        presentableStack.PushAll(
            [
                new ViewModelDescriptor<TestViewModel1, ViewModelTestContext>(viewModelTestContext),
                new ViewModelDescriptor<TestViewModel2, ViewModelTestContext>(viewModelTestContext),
                new ViewModelDescriptor<TestViewModel3, ViewModelTestContext>(viewModelTestContext)
            ]
        );

        Assert.AreEqual(3, presentableStack.ViewModels.Length);
        Assert.IsInstanceOfType<TestViewModel3>(presentableStack.ViewModels[0]);
        Assert.IsInstanceOfType<TestViewModel2>(presentableStack.ViewModels[1]);
        Assert.IsInstanceOfType<TestViewModel1>(presentableStack.ViewModels[2]);
    }

    [TestMethod]
    public void Push_Method_With_Template_Returns_View_With_Template()
    {
        using var context = new TestContext();

        var viewModelTestContext = new ViewModelTestContext([]);

        var presentableStack = (PresentableStack)context.ServiceProvider.GetRequiredService<IPresentableStack>();
        var viewModel = presentableStack.Push<TestViewModel1, ViewModelTestContext>(viewModelTestContext);

        Assert.IsNotNull(viewModel);
    }

    [TestMethod]
    public void Closable_Event_Removes_View_Model()
    {
        using var context = new TestContext();

        var viewModelTestContext = new ViewModelTestContext([]);

        var presentableStack = (PresentableStack)context.ServiceProvider.GetRequiredService<IPresentableStack>();
        var viewModel = presentableStack.Push<TestViewModel1, ViewModelTestContext>(viewModelTestContext);

        Assert.IsNotNull(presentableStack.Current);

        viewModel.Close();

        Assert.IsNull(presentableStack.Current);
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

    private class TestViewModel1(ViewModelTestContext context) : ViewModel<ViewModelTestContext>, IClosable
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
