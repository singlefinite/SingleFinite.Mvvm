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

using SingleFinite.Mvvm.Internal;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class ViewStackTests
{
    [TestMethod]
    public void Lifecycle_Events_Raised_As_Expected()
    {
        var output = new List<string>();
        var viewStack = new ViewStack();
        var view1 = new TestView(new TestViewModel("1", output));
        var view2 = new TestView(new TestViewModel("2", output));
        var view3 = new TestView(new TestViewModel("3", output));

        Assert.IsNull(viewStack.Current);
        Assert.AreEqual(0, viewStack.Views.Length);
        Assert.AreEqual(0, viewStack.ViewModels.Length);

        viewStack.Push([view1], 0);

        Assert.AreEqual(view1, viewStack.Current);
        Assert.AreEqual(1, viewStack.Views.Length);
        Assert.AreEqual(1, viewStack.ViewModels.Length);

        Assert.AreEqual(1, output.Count);
        Assert.AreEqual("1 Activate", output[0]);
        output.Clear();

        viewStack.Push([view2], 0);

        Assert.AreEqual(view2, viewStack.Current);
        Assert.AreEqual(2, viewStack.Views.Length);
        Assert.AreEqual(2, viewStack.ViewModels.Length);

        Assert.AreEqual(2, output.Count);
        Assert.AreEqual("1 Deactivate", output[0]);
        Assert.AreEqual("2 Activate", output[1]);
        output.Clear();

        viewStack.Push([view3], 0);

        Assert.AreEqual(view3, viewStack.Current);
        Assert.AreEqual(3, viewStack.Views.Length);
        Assert.AreEqual(3, viewStack.ViewModels.Length);

        Assert.AreEqual(2, output.Count);
        Assert.AreEqual("2 Deactivate", output[0]);
        Assert.AreEqual("3 Activate", output[1]);
        output.Clear();

        viewStack.Pop(1);

        Assert.AreEqual(view2, viewStack.Current);
        Assert.AreEqual(2, viewStack.Views.Length);
        Assert.AreEqual(2, viewStack.ViewModels.Length);

        Assert.AreEqual(3, output.Count);
        Assert.AreEqual("3 Deactivate", output[0]);
        Assert.AreEqual("3 Dispose", output[1]);
        Assert.AreEqual("2 Activate", output[2]);
        output.Clear();

        viewStack.Pop(1);

        Assert.AreEqual(view1, viewStack.Current);
        Assert.AreEqual(1, viewStack.Views.Length);
        Assert.AreEqual(1, viewStack.ViewModels.Length);

        Assert.AreEqual(3, output.Count);
        Assert.AreEqual("2 Deactivate", output[0]);
        Assert.AreEqual("2 Dispose", output[1]);
        Assert.AreEqual("1 Activate", output[2]);
        output.Clear();

        viewStack.Pop(1);

        Assert.IsNull(viewStack.Current);
        Assert.AreEqual(0, viewStack.Views.Length);
        Assert.AreEqual(0, viewStack.ViewModels.Length);

        Assert.AreEqual(2, output.Count);
        Assert.AreEqual("1 Deactivate", output[0]);
        Assert.AreEqual("1 Dispose", output[1]);
        output.Clear();
    }

    [TestMethod]
    public void Push_Multiple_Views_Has_Expected_Lifecycle_Events()
    {
        var output = new List<string>();
        var viewStack = new ViewStack();
        var view1 = new TestView(new TestViewModel("1", output));
        var view2 = new TestView(new TestViewModel("2", output));
        var view3 = new TestView(new TestViewModel("3", output));

        viewStack.Push([view1, view2, view3], 0);

        Assert.AreEqual(view3, viewStack.Current);
        Assert.AreEqual(3, viewStack.Views.Length);
        Assert.AreEqual(3, viewStack.ViewModels.Length);
        Assert.AreEqual(view3, viewStack.Views[0]);
        Assert.AreEqual(view2, viewStack.Views[1]);
        Assert.AreEqual(view1, viewStack.Views[2]);
        Assert.AreEqual(view3.ViewModel, viewStack.Views[0].ViewModel);
        Assert.AreEqual(view2.ViewModel, viewStack.Views[1].ViewModel);
        Assert.AreEqual(view1.ViewModel, viewStack.Views[2].ViewModel);

        Assert.AreEqual(1, output.Count);
        Assert.AreEqual("3 Activate", output[0]);
    }

    [TestMethod]
    public void Pop_Multiple_Views_Has_Expected_Lifecycle_Events()
    {
        var output = new List<string>();
        var viewStack = new ViewStack();
        var view1 = new TestView(new TestViewModel("1", output));
        var view2 = new TestView(new TestViewModel("2", output));
        var view3 = new TestView(new TestViewModel("3", output));

        viewStack.Push([view1, view2, view3], 0);
        output.Clear();

        viewStack.Pop(2);

        Assert.AreEqual(view1, viewStack.Current);
        Assert.AreEqual(1, viewStack.Views.Length);
        Assert.AreEqual(1, viewStack.ViewModels.Length);
        Assert.AreEqual(view1, viewStack.Views[0]);
        Assert.AreEqual(view1.ViewModel, viewStack.Views[0].ViewModel);

        Assert.AreEqual(4, output.Count);
        Assert.AreEqual("3 Deactivate", output[0]);
        Assert.AreEqual("3 Dispose", output[1]);
        Assert.AreEqual("2 Dispose", output[2]);
        Assert.AreEqual("1 Activate", output[3]);
    }

    [TestMethod]
    public void Close_Removes_ViewModel_From_Top()
    {
        var output = new List<string>();
        var viewStack = new ViewStack();
        var view1 = new TestView(new TestViewModel("1", output));
        var view2 = new TestView(new TestViewModel("2", output));
        var view3 = new TestView(new TestViewModel("3", output));

        viewStack.Push([view1, view2, view3], 0);
        output.Clear();

        viewStack.Close(view3.ViewModel);

        Assert.AreEqual(view2, viewStack.Current);
        Assert.AreEqual(2, viewStack.Views.Length);
        Assert.AreEqual(2, viewStack.ViewModels.Length);

        Assert.AreEqual(3, output.Count);
        Assert.AreEqual("3 Deactivate", output[0]);
        Assert.AreEqual("3 Dispose", output[1]);
        Assert.AreEqual("2 Activate", output[2]);
    }

    [TestMethod]
    public void Close_Removes_ViewModel_From_Middle()
    {
        var output = new List<string>();
        var viewStack = new ViewStack();
        var view1 = new TestView(new TestViewModel("1", output));
        var view2 = new TestView(new TestViewModel("2", output));
        var view3 = new TestView(new TestViewModel("3", output));

        viewStack.Push([view1, view2, view3], 0);
        output.Clear();

        viewStack.Close(view2.ViewModel);

        Assert.AreEqual(view3, viewStack.Current);
        Assert.AreEqual(2, viewStack.Views.Length);
        Assert.AreEqual(2, viewStack.ViewModels.Length);

        Assert.AreEqual(1, output.Count);
        Assert.AreEqual("2 Dispose", output[0]);
    }

    [TestMethod]
    public void Close_Removes_ViewModel_From_Bottom()
    {
        var output = new List<string>();
        var viewStack = new ViewStack();
        var view1 = new TestView(new TestViewModel("1", output));
        var view2 = new TestView(new TestViewModel("2", output));
        var view3 = new TestView(new TestViewModel("3", output));

        viewStack.Push([view1, view2, view3], 0);
        output.Clear();

        viewStack.Close(view1.ViewModel);

        Assert.AreEqual(view3, viewStack.Current);
        Assert.AreEqual(2, viewStack.Views.Length);
        Assert.AreEqual(2, viewStack.ViewModels.Length);

        Assert.AreEqual(1, output.Count);
        Assert.AreEqual("1 Dispose", output[0]);
    }

    [TestMethod]
    public void Close_Multiple_Removes_Multiple_ViewModels()
    {
        var output = new List<string>();
        var viewStack = new ViewStack();
        var view1 = new TestView(new TestViewModel("1", output));
        var view2 = new TestView(new TestViewModel("2", output));
        var view3 = new TestView(new TestViewModel("3", output));
        var view4 = new TestView(new TestViewModel("4", output));

        viewStack.Push([view1, view2, view3, view4], 0);
        output.Clear();

        viewStack.Close(view3.ViewModel, view1.ViewModel);

        Assert.AreEqual(view4, viewStack.Current);
        Assert.AreEqual(2, viewStack.Views.Length);
        Assert.AreEqual(2, viewStack.ViewModels.Length);
        Assert.AreEqual(view4, viewStack.Views[0]);
        Assert.AreEqual(view2, viewStack.Views[1]);
        Assert.AreEqual(view4.ViewModel, viewStack.Views[0].ViewModel);
        Assert.AreEqual(view2.ViewModel, viewStack.Views[1].ViewModel);

        Assert.AreEqual(2, output.Count);
        Assert.AreEqual("1 Dispose", output[0]);
        Assert.AreEqual("3 Dispose", output[1]);
    }

    [TestMethod]
    public void Clear_Removes_All_Views()
    {
        var output = new List<string>();
        var viewStack = new ViewStack();
        var view1 = new TestView(new TestViewModel("1", output));
        var view2 = new TestView(new TestViewModel("2", output));
        var view3 = new TestView(new TestViewModel("3", output));

        viewStack.Push([view1, view2, view3], 0);
        output.Clear();

        viewStack.Clear();

        Assert.IsNull(viewStack.Current);
        Assert.AreEqual(0, viewStack.Views.Length);
        Assert.AreEqual(0, viewStack.ViewModels.Length);

        Assert.AreEqual(4, output.Count);
        Assert.AreEqual("3 Deactivate", output[0]);
        Assert.AreEqual("3 Dispose", output[1]);
        Assert.AreEqual("2 Dispose", output[2]);
        Assert.AreEqual("1 Dispose", output[3]);
    }

    [TestMethod]
    public void Closable_Event_Removes_ViewModel()
    {
        var output = new List<string>();
        var viewStack = new ViewStack();
        var view1 = new TestView(new TestViewModel("1", output));
        var view2 = new TestView(new TestViewModel("2", output));
        var view3 = new TestView(new TestViewModel("3", output));

        viewStack.Push([view1, view2, view3], 0);
        output.Clear();

        view2.ViewModel.Close();

        Assert.AreEqual(view3, viewStack.Current);
        Assert.AreEqual(2, viewStack.Views.Length);
        Assert.AreEqual(2, viewStack.ViewModels.Length);
        Assert.AreEqual(view3, viewStack.Views[0]);
        Assert.AreEqual(view1, viewStack.Views[1]);
        Assert.AreEqual(view3.ViewModel, viewStack.Views[0].ViewModel);
        Assert.AreEqual(view1.ViewModel, viewStack.Views[1].ViewModel);

        Assert.AreEqual(1, output.Count);
        Assert.AreEqual("2 Dispose", output[0]);
    }

    [TestMethod]
    public void Current_Changed_Events_Are_Raised()
    {
        var output = new List<string>();
        var viewStack = new ViewStack();
        viewStack.CurrentChanged.Register(
            args => output.Add($"{args?.View},{args?.IsNew}")
        );
        var view1 = new TestView(new TestViewModel("1", []));
        var view2 = new TestView(new TestViewModel("2", []));
        var view3 = new TestView(new TestViewModel("3", []));

        viewStack.Push([view1, view2, view3], 0);

        Assert.AreEqual(1, output.Count);
        Assert.AreEqual("View:ViewModel:3,True", output[0]);
        output.Clear();

        viewStack.Pop(1);

        Assert.AreEqual(1, output.Count);
        Assert.AreEqual("View:ViewModel:2,False", output[0]);
        output.Clear();

        viewStack.Clear();

        Assert.AreEqual(1, output.Count);
        Assert.AreEqual(",False", output[0]);
        output.Clear();
    }

    [TestMethod]
    public void Push_Empty_Collection_Has_No_Effect()
    {
        var output = new List<string>();
        var viewStack = new ViewStack();
        viewStack.CurrentChanged.Register(view => output.Add(view?.ToString() ?? "null"));
        var view1 = new TestView(new TestViewModel("1", []));
        var view2 = new TestView(new TestViewModel("2", []));
        var view3 = new TestView(new TestViewModel("3", []));

        viewStack.Push([view1, view2, view3], 0);
        output.Clear();

        viewStack.Push([], 2);

        Assert.AreEqual(0, output.Count);
        Assert.AreEqual(view3, viewStack.Current);
    }

    #region Types

    private class TestViewModel(string name, List<string> output) : ViewModel, IClosable
    {
        public string Name => name;

        protected override void OnActivate(CancellationToken cancellationToken)
        {
            output.Add($"{name} Activate");
        }

        protected override void OnDeactivate()
        {
            output.Add($"{name} Deactivate");
        }

        protected override void OnDispose()
        {
            output.Add($"{name} Dispose");
        }

        public override string ToString() => $"ViewModel:{name}";

        public void Close() => _closedSource.RaiseEvent(this);

        public EventToken<IClosable> Closed => _closedSource.Token;
        private readonly EventTokenSource<IClosable> _closedSource = new();
    }

    private class TestView(TestViewModel viewModel) : IView<TestViewModel>
    {
        public TestViewModel ViewModel => viewModel;

        public override string ToString() => $"View:{viewModel}";
    }

    #endregion
}
