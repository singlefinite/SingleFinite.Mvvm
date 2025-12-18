// MIT License
// Copyright (c) 2025 Single Finite
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
using SingleFinite.Mvvm.Services;
using Microsoft.Extensions.DependencyInjection;
using SingleFinite.Essentials;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class ViewModelTests
{
    [TestMethod]
    public void Dispose_Method_Disposes_All_Descendants()
    {
        using var context = new TestContext();
        var scope = context.ServiceProvider.CreateLinkedScope();

        var viewModel1 = new NestingViewModel(scope.ServiceProvider.GetRequiredService<IPresentableStack>());
        var viewModel2 = viewModel1.CreateChild("second");
        var viewModel3 = viewModel2.CreateChild("third");

        Assert.IsFalse(viewModel1.IsDisposed);
        Assert.IsFalse(viewModel2.IsDisposed);
        Assert.IsFalse(viewModel3.IsDisposed);

        viewModel1.ClearChildren();

        Assert.IsFalse(viewModel1.IsDisposed);
        Assert.IsTrue(viewModel2.IsDisposed);
        Assert.IsTrue(viewModel3.IsDisposed);
    }

    [TestMethod]
    public void Lifecycle_Events_Fire_In_Expected_Order()
    {
        var onInitializeCount = 0;
        var onActivateCount = 0;
        var onDeactivateCount = 0;
        var onDisposeCount = 0;
        var isActiveChanged = new List<bool>();

        var viewModel = new SimpleViewModel();
        var viewModelInterface = (IViewModel)viewModel;

        viewModel.Initialized
            .Observe()
            .OnEach(() => onInitializeCount++);
        viewModel.Activated
            .Observe()
            .OnEach(() => onActivateCount++);
        viewModel.Deactivated
            .Observe()
            .OnEach(() => onDeactivateCount++);
        viewModel.Disposed
            .Observe()
            .OnEach(() => onDisposeCount++);
        viewModel.IsActiveChanged
            .Observe()
            .OnEach(isActiveChanged.Add);

        Assert.AreEqual(0, onInitializeCount);
        Assert.AreEqual(0, onActivateCount);
        Assert.AreEqual(0, onDeactivateCount);
        Assert.AreEqual(0, onDisposeCount);
        Assert.IsEmpty(isActiveChanged);

        Assert.IsFalse(viewModel.IsActive);
        Assert.IsFalse(viewModel.IsDisposed);

        viewModelInterface.Initialize();

        Assert.AreEqual(1, onInitializeCount);
        Assert.AreEqual(0, onActivateCount);
        Assert.AreEqual(0, onDeactivateCount);
        Assert.AreEqual(0, onDisposeCount);
        Assert.IsEmpty(isActiveChanged);

        Assert.IsFalse(viewModel.IsActive);
        Assert.IsFalse(viewModel.IsDisposed);

        viewModelInterface.Initialize();

        Assert.AreEqual(1, onInitializeCount);
        Assert.AreEqual(0, onActivateCount);
        Assert.AreEqual(0, onDeactivateCount);
        Assert.AreEqual(0, onDisposeCount);
        Assert.IsEmpty(isActiveChanged);

        Assert.IsFalse(viewModel.IsActive);
        Assert.IsFalse(viewModel.IsDisposed);

        viewModelInterface.Activate();

        Assert.AreEqual(1, onInitializeCount);
        Assert.AreEqual(1, onActivateCount);
        Assert.AreEqual(0, onDeactivateCount);
        Assert.AreEqual(0, onDisposeCount);

        Assert.HasCount(1, isActiveChanged);
        Assert.IsTrue(isActiveChanged[0]);
        isActiveChanged.Clear();

        Assert.IsTrue(viewModel.IsActive);
        Assert.IsFalse(viewModel.IsDisposed);

        viewModelInterface.Activate();

        Assert.AreEqual(1, onInitializeCount);
        Assert.AreEqual(1, onActivateCount);
        Assert.AreEqual(0, onDeactivateCount);
        Assert.AreEqual(0, onDisposeCount);
        Assert.IsEmpty(isActiveChanged);

        Assert.IsTrue(viewModel.IsActive);
        Assert.IsFalse(viewModel.IsDisposed);

        viewModelInterface.Deactivate();

        Assert.AreEqual(1, onInitializeCount);
        Assert.AreEqual(1, onActivateCount);
        Assert.AreEqual(1, onDeactivateCount);
        Assert.AreEqual(0, onDisposeCount);

        Assert.HasCount(1, isActiveChanged);
        Assert.IsFalse(isActiveChanged[0]);
        isActiveChanged.Clear();

        Assert.IsFalse(viewModel.IsActive);
        Assert.IsFalse(viewModel.IsDisposed);

        viewModelInterface.Deactivate();

        Assert.AreEqual(1, onInitializeCount);
        Assert.AreEqual(1, onActivateCount);
        Assert.AreEqual(1, onDeactivateCount);
        Assert.AreEqual(0, onDisposeCount);
        Assert.IsEmpty(isActiveChanged);

        Assert.IsFalse(viewModel.IsActive);
        Assert.IsFalse(viewModel.IsDisposed);

        viewModelInterface.Dispose();

        Assert.AreEqual(1, onInitializeCount);
        Assert.AreEqual(1, onActivateCount);
        Assert.AreEqual(1, onDeactivateCount);
        Assert.AreEqual(1, onDisposeCount);
        Assert.IsEmpty(isActiveChanged);

        Assert.IsFalse(viewModel.IsActive);
        Assert.IsTrue(viewModel.IsDisposed);

        viewModelInterface.Dispose();

        Assert.AreEqual(1, onInitializeCount);
        Assert.AreEqual(1, onActivateCount);
        Assert.AreEqual(1, onDeactivateCount);
        Assert.AreEqual(1, onDisposeCount);
        Assert.IsEmpty(isActiveChanged);

        Assert.IsFalse(viewModel.IsActive);
        Assert.IsTrue(viewModel.IsDisposed);
    }

    #region Types

    private class SimpleViewModel : ViewModel
    {
        public int Number
        {
            get;
            set => ChangeProperty(ref field, value);
        }
    }

    private class SimpleView(SimpleViewModel viewModel) : IView<SimpleViewModel>
    {
        public SimpleViewModel ViewModel { get; } = viewModel;
    }

    private class NestingView(NestingViewModel viewModel) : IView<NestingViewModel>
    {
        public NestingViewModel ViewModel => viewModel;
    }

    private class NestingViewModel(IPresentableStack presenterStack) : ViewModel
    {
        private string _name = "first";
        public string Name => _name;

        public NestingViewModel CreateChild(string name)
        {
            var child = presenterStack.Push<NestingViewModel>();
            child._name = name;
            return child;
        }

        public void ClearChildren()
        {
            presenterStack.Clear();
        }
    }

    #endregion
}
