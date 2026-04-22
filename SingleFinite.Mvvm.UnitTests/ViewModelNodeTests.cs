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

using SingleFinite.Essentials;
using SingleFinite.Mvvm.Internal;
using SingleFinite.Mvvm.Internal.Services;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class ViewModelNodeTests(TestContext testContext)
{
    [TestMethod]
    public void Calling_Initialized_More_Than_Once_Throws()
    {
        var node = new ViewModelNode(new ScopeContext());
        node.Initialize(
            viewModel: new ExampleViewModel(),
            parent: null
        );

        Assert.Throws<InvalidOperationException>(() =>
        {
            node.Initialize(
                viewModel: new ExampleViewModel(),
                parent: null
            );
        });
    }

    [TestMethod]
    public void IsActiveToRoot_Change_From_Parent_Changes_Child()
    {
        var observedChanges = new List<bool>();
        var scopeContext = new ScopeContext();

        var rootParentNode = new ViewModelNode(scopeContext);
        rootParentNode.Initialize(
            viewModel: new ExampleViewModel(),
            parent: null
        );

        var firstParentNode = new ViewModelNode(scopeContext);
        firstParentNode.Initialize(
            viewModel: new ExampleViewModel(),
            parent: rootParentNode
        );

        var childNode = new ViewModelNode(scopeContext);
        childNode.Initialize(
            viewModel: new ExampleViewModel(),
            parent: firstParentNode
        );

        rootParentNode.ViewModel?.Activate();
        firstParentNode.ViewModel?.Activate();
        childNode.ViewModel?.Activate();

        childNode.IsActiveFromRootChanged
            .Observe()
            .OnEach(observedChanges.Add)
            .Until(testContext.CancellationToken);

        Assert.IsTrue(childNode.IsActiveFromRoot);

        rootParentNode.ViewModel?.Deactivate();

        Assert.HasCount(1, observedChanges);
        Assert.IsFalse(observedChanges[0]);
        Assert.IsFalse(childNode.IsActiveFromRoot);

        rootParentNode.ViewModel?.Activate();

        Assert.HasCount(2, observedChanges);
        Assert.IsTrue(observedChanges[1]);
        Assert.IsTrue(childNode.IsActiveFromRoot);

        scopeContext.Dispose();

        rootParentNode.ViewModel?.Deactivate();

        Assert.HasCount(2, observedChanges);
        Assert.IsTrue(childNode.IsActiveFromRoot);
    }

    #region Types

    private class ExampleViewModel : ViewModel
    {
    }

    #endregion
}
