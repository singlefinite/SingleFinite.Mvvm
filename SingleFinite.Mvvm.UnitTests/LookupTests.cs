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
using SingleFinite.Mvvm.Services.Presenters;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class LookupTests
{
    [TestMethod]
    public async Task LookupAtRoot()
    {
        using var context = await MvvmTestContext.CreateAsync();
        var presenter = context.ServiceProvider.GetRequiredService<IItemPresenter>();

        var root = presenter.Set<RootViewModel>();
        var parent = root.Parent;
        var child = parent?.Child;

        Assert.IsNotNull(child);

        Assert.AreEqual(root, child.Example1.Get());
        Assert.AreEqual(parent, child.Example2.Get());
        Assert.IsFalse(child.Example3.TryGet(out var _));
        Assert.Throws<InvalidOperationException>(child.Example3.Get);
    }

    private class RootViewModel(IItemPresenter presenter) : ViewModel, IExample1, IExample2
    {
        public ParentViewModel? Parent { get; private set; }

        protected override void OnCreated()
        {
            Parent = presenter.Set<ParentViewModel>();
        }
    }

    private class ParentViewModel(IItemPresenter presenter) : ViewModel, IExample2
    {
        public ChildViewModel? Child { get; private set; }

        protected override void OnCreated()
        {
            Child = presenter.Set<ChildViewModel>();
        }
    }

    private class ChildViewModel(
        ILookup<IExample1> example1,
        ILookup<IExample2> example2,
        ILookup<IExample3> example3
    ) : ViewModel, IExample2
    {
        public ILookup<IExample1> Example1 => example1;
        public ILookup<IExample2> Example2 => example2;
        public ILookup<IExample3> Example3 => example3;
    }

    private interface IExample1
    {
    }

    private interface IExample2
    {
    }

    private interface IExample3
    {
    }

    private class RootView(RootViewModel viewModel) : IView<RootViewModel>
    {
        public RootViewModel ViewModel => viewModel;
    }

    private class ParentView(ParentViewModel viewModel) : IView<ParentViewModel>
    {
        public ParentViewModel ViewModel => viewModel;
    }

    private class ChildView(ChildViewModel viewModel) : IView<ChildViewModel>
    {
        public ChildViewModel ViewModel => viewModel;
    }
}
