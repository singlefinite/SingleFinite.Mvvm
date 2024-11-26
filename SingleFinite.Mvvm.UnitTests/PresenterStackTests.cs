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
public class PresenterStackTests
{
    [TestMethod]
    public void Lifecycle_Events_Raised_When_Expected()
    {
        using var context = new TestContext();
        var presenterStack = (PresenterStack)context.ServiceProvider.GetRequiredService<IPresenterStack>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1, ViewModelTestContext>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2, ViewModelTestContext>(viewModelContext);
        var viewModelDescriptor3 = new ViewModelDescriptor<TestViewModel3, ViewModelTestContext>(viewModelContext);

        presenterStack.Push(viewModelDescriptor1);
        Assert.AreEqual(2, output.Count);
        Assert.AreEqual("OnInit - TestViewModel1", output[0]);
        Assert.AreEqual("OnStart - TestViewModel1", output[1]);

        output.Clear();
        presenterStack.PushAll([viewModelDescriptor2, viewModelDescriptor3]);
        Assert.AreEqual(4, output.Count);
        Assert.AreEqual("OnInit - TestViewModel2", output[0]);
        Assert.AreEqual("OnInit - TestViewModel3", output[1]);
        Assert.AreEqual("OnStop - TestViewModel1", output[2]);
        Assert.AreEqual("OnStart - TestViewModel3", output[3]);

        output.Clear();
        presenterStack.PopTo<TestViewModel2>();
        Assert.AreEqual(3, output.Count);
        Assert.AreEqual("OnStop - TestViewModel3", output[0]);
        Assert.AreEqual("OnDispose - TestViewModel3", output[1]);
        Assert.AreEqual("OnStart - TestViewModel2", output[2]);

        output.Clear();
        presenterStack.Clear();
        Assert.AreEqual(3, output.Count);
        Assert.AreEqual("OnStop - TestViewModel2", output[0]);
        Assert.AreEqual("OnDispose - TestViewModel2", output[1]);
        Assert.AreEqual("OnDispose - TestViewModel1", output[2]);
    }

    [TestMethod]
    public void Changed_Event_Is_Raised()
    {
        using var context = new TestContext();
        var presenterStack = (PresenterStack)context.ServiceProvider.GetRequiredService<IPresenterStack>();

        IPresenter.CurrentChangedEventArgs? observedArgs = null;
        presenterStack.CurrentChanged.Register(args => observedArgs = args);

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1, ViewModelTestContext>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2, ViewModelTestContext>(viewModelContext);

        Assert.IsNull(observedArgs);

        var viewModel1 = presenterStack.Push(viewModelDescriptor1);

        Assert.IsNotNull(observedArgs);
        Assert.AreEqual(viewModel1, observedArgs.View?.ViewModel);
        Assert.IsTrue(observedArgs.IsNew);

        observedArgs = null;

        var viewModel2 = presenterStack.Push(viewModelDescriptor2);

        Assert.IsNotNull(observedArgs);
        Assert.AreEqual(viewModel2, observedArgs.View?.ViewModel);
        Assert.IsTrue(observedArgs.IsNew);

        observedArgs = null;

        presenterStack.Pop();

        Assert.IsNotNull(observedArgs);
        Assert.AreEqual(viewModel1, observedArgs.View?.ViewModel);
        Assert.IsFalse(observedArgs.IsNew);
    }

    [TestMethod]
    public void PopTo_Method_With_ViewModel_Type_Pops_Views()
    {
        using var context = new TestContext();
        var presenterStack = (PresenterStack)context.ServiceProvider.GetRequiredService<IPresenterStack>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1, ViewModelTestContext>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2, ViewModelTestContext>(viewModelContext);
        var viewModelDescriptor3 = new ViewModelDescriptor<TestViewModel3, ViewModelTestContext>(viewModelContext);

        presenterStack.PushAll([viewModelDescriptor1, viewModelDescriptor2, viewModelDescriptor3]);
        var result1 = presenterStack.PopTo<TestViewModel1>(inclusive: true);
        Assert.AreEqual(true, result1);
        Assert.AreEqual(0, presenterStack.ViewModels.Length);
        Assert.AreEqual(null, presenterStack.Current);

        presenterStack.PushAll([viewModelDescriptor1, viewModelDescriptor2]);
        var result2 = presenterStack.PopTo<TestViewModel3>(inclusive: true);
        Assert.AreEqual(false, result2);
        Assert.AreEqual(2, presenterStack.ViewModels.Length);
        Assert.IsNotNull(presenterStack.Current);

        var result3 = presenterStack.PopTo<TestViewModel1>(inclusive: true);
        Assert.AreEqual(true, result3);
        Assert.AreEqual(0, presenterStack.ViewModels.Length);
        Assert.AreEqual(null, presenterStack.Current);
    }

    [TestMethod]
    public void PopTo_Method_With_Query_Pops_Views()
    {
        using var context = new TestContext();
        var presenterStack = (PresenterStack)context.ServiceProvider.GetRequiredService<IPresenterStack>();

        var output = new List<string>();
        var viewModelContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1, ViewModelTestContext>(viewModelContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2, ViewModelTestContext>(viewModelContext);
        var viewModelDescriptor3 = new ViewModelDescriptor<TestViewModel3, ViewModelTestContext>(viewModelContext);

        presenterStack.PushAll([viewModelDescriptor1, viewModelDescriptor2, viewModelDescriptor3, viewModelDescriptor1]);
        var result1 = presenterStack.PopTo(
            predicate: viewModel => viewModel is TestViewModel1,
            fromTop: false,
            inclusive: true
        );
        Assert.AreEqual(true, result1);
        Assert.AreEqual(0, presenterStack.ViewModels.Length);

        presenterStack.PushAll([viewModelDescriptor1, viewModelDescriptor2, viewModelDescriptor3, viewModelDescriptor1]);
        var result2 = presenterStack.PopTo(
            predicate: viewModel => viewModel is TestViewModel1,
            fromTop: true,
            inclusive: true
        );
        Assert.AreEqual(true, result1);
        Assert.AreEqual(3, presenterStack.ViewModels.Length);
        Assert.AreEqual(presenterStack.ViewModels[0].GetType(), typeof(TestViewModel3));
        Assert.AreEqual(presenterStack.ViewModels[1].GetType(), typeof(TestViewModel2));
        Assert.AreEqual(presenterStack.ViewModels[2].GetType(), typeof(TestViewModel1));
    }

    [TestMethod]
    public void Presenter_Is_Disposed_When_ServiceScope_Is_Disposed()
    {
        using var context = new TestContext();
        var scope = context.ServiceProvider.CreateScope();
        var presenterStackInScope = (PresenterStack)scope.ServiceProvider.GetRequiredService<IPresenterStack>();
        var presenterStackInRoot = (PresenterStack)context.ServiceProvider.GetRequiredService<IPresenterStack>();

        var output = new List<string>();
        var viewModelTestContext = new ViewModelTestContext(output);
        var viewModelDescriptor1 = new ViewModelDescriptor<TestViewModel1, ViewModelTestContext>(viewModelTestContext);
        var viewModelDescriptor2 = new ViewModelDescriptor<TestViewModel2, ViewModelTestContext>(viewModelTestContext);
        var viewModelDescriptor3 = new ViewModelDescriptor<TestViewModel3, ViewModelTestContext>(viewModelTestContext);

        presenterStackInScope.PushAll([viewModelDescriptor1, viewModelDescriptor2, viewModelDescriptor3]);
        presenterStackInRoot.PushAll([viewModelDescriptor1, viewModelDescriptor2, viewModelDescriptor3]);
        Assert.AreEqual(3, presenterStackInScope.ViewModels.Length);
        Assert.AreEqual(3, presenterStackInRoot.ViewModels.Length);

        scope.Dispose();
        Assert.AreEqual(0, presenterStackInScope.ViewModels.Length);
        Assert.AreEqual(3, presenterStackInRoot.ViewModels.Length);
    }

    [TestMethod]
    public void Stack_Property_Is_List_Ordered_From_Stack_Top_To_Stack_Bottom()
    {
        using var context = new TestContext();

        var output = new List<string>();
        var viewModelTestContext = new ViewModelTestContext(output);

        var presenterStack = (PresenterStack)context.ServiceProvider.GetRequiredService<IPresenterStack>();
        presenterStack.PushAll(
            [
                new ViewModelDescriptor<TestViewModel1, ViewModelTestContext>(viewModelTestContext),
                new ViewModelDescriptor<TestViewModel2, ViewModelTestContext>(viewModelTestContext),
                new ViewModelDescriptor<TestViewModel3, ViewModelTestContext>(viewModelTestContext)
            ]
        );

        Assert.AreEqual(3, presenterStack.ViewModels.Length);
        Assert.IsInstanceOfType<TestViewModel3>(presenterStack.ViewModels[0]);
        Assert.IsInstanceOfType<TestViewModel2>(presenterStack.ViewModels[1]);
        Assert.IsInstanceOfType<TestViewModel1>(presenterStack.ViewModels[2]);
    }

    [TestMethod]
    public void Push_Method_With_Template_Returns_View_With_Template()
    {
        using var context = new TestContext();

        var viewModelTestContext = new ViewModelTestContext([]);

        var presenterStack = (PresenterStack)context.ServiceProvider.GetRequiredService<IPresenterStack>();
        var viewModel = presenterStack.Push<TestViewModel1, ViewModelTestContext>(viewModelTestContext);

        Assert.IsNotNull(viewModel);
    }

    [TestMethod]
    public void Closable_Event_Removes_View_Model()
    {
        using var context = new TestContext();

        var viewModelTestContext = new ViewModelTestContext([]);

        var presenterStack = (PresenterStack)context.ServiceProvider.GetRequiredService<IPresenterStack>();
        var viewModel = presenterStack.Push<TestViewModel1, ViewModelTestContext>(viewModelTestContext);

        Assert.IsNotNull(presenterStack.Current);

        viewModel.Close();

        Assert.IsNull(presenterStack.Current);
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

        public EventToken<IClosable> Closed => _closedSource.Token;
        private readonly EventTokenSource<IClosable> _closedSource = new();
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
