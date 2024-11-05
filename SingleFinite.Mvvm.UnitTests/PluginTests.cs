// MIT License
// Copyright (c) 2024 Single Finite
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR 
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using SingleFinite.Mvvm.Services;
using Microsoft.Extensions.DependencyInjection;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class PluginTests
{
    /// <summary>
    /// Verify the lifecycle methods get called when expected.
    /// </summary>
    [TestMethod]
    public void LifeCycleEventsCalled()
    {
        using var context = new TestContext();
        var presenter = context.ServiceProvider.GetRequiredService<IPresenterFrame>();
        var view = presenter.Set<ExamplePluginHost>();
        var viewModel = (ExamplePluginHost)view.ViewModel;
        var lines = viewModel.Lines;

        Assert.AreEqual(6, lines.Count);
        Assert.AreEqual("ExamplePluginHost OnInitialize", lines[0]);
        Assert.AreEqual("ExamplePluginA OnInitialize", lines[1]);
        Assert.AreEqual("ExamplePluginB OnInitialize", lines[2]);
        Assert.AreEqual("ExamplePluginHost OnActivate", lines[3]);
        Assert.AreEqual("ExamplePluginA OnActivate", lines[4]);
        Assert.AreEqual("ExamplePluginB OnActivate", lines[5]);

        lines.Clear();
        presenter.Clear();

        Assert.AreEqual(6, lines.Count);
        Assert.AreEqual("ExamplePluginHost OnDeactivate", lines[0]);
        Assert.AreEqual("ExamplePluginA OnDeactivate", lines[1]);
        Assert.AreEqual("ExamplePluginB OnDeactivate", lines[2]);
        Assert.AreEqual("ExamplePluginHost OnDispose", lines[3]);
        Assert.AreEqual("ExamplePluginA OnDispose", lines[4]);
        Assert.AreEqual("ExamplePluginB OnDispose", lines[5]);
    }

    /// <summary>
    /// Class used for unit tests.
    /// </summary>
    private class ExamplePluginHost : ViewModel, IPluginHost
    {
        public List<string> Lines { get; } = [];

        protected override void OnInitialize() =>
            Lines.Add("ExamplePluginHost OnInitialize");

        protected override void OnActivate(CancellationToken cancellationToken) =>
            Lines.Add("ExamplePluginHost OnActivate");

        protected override void OnDeactivate() =>
            Lines.Add("ExamplePluginHost OnDeactivate");

        protected override void OnDispose() =>
            Lines.Add("ExamplePluginHost OnDispose");
    }

    /// <summary>
    /// Class used for unit tests.
    /// </summary>
    private class ExamplePluginA : Plugin<ExamplePluginHost>
    {
        protected override void OnInitialize() =>
            PluginHost.Lines.Add("ExamplePluginA OnInitialize");

        protected override void OnActivate(CancellationToken cancellationToken) =>
            PluginHost.Lines.Add("ExamplePluginA OnActivate");

        protected override void OnDeactivate() =>
            PluginHost.Lines.Add("ExamplePluginA OnDeactivate");

        protected override void OnDispose() =>
            PluginHost.Lines.Add("ExamplePluginA OnDispose");
    }

    /// <summary>
    /// Class used for unit tests.
    /// </summary>
    private class ExamplePluginB : Plugin<ExamplePluginHost>
    {
        protected override void OnInitialize() =>
            PluginHost.Lines.Add("ExamplePluginB OnInitialize");

        protected override void OnActivate(CancellationToken cancellationToken) =>
            PluginHost.Lines.Add("ExamplePluginB OnActivate");

        protected override void OnDeactivate() =>
            PluginHost.Lines.Add("ExamplePluginB OnDeactivate");

        protected override void OnDispose() =>
            PluginHost.Lines.Add("ExamplePluginB OnDispose");
    }

    /// <summary>
    /// Class used for unit tests.
    /// </summary>
    private class ExamplePluginHostView : IView<ExamplePluginHost>
    {
        public ExamplePluginHostView(ExamplePluginHost viewModel)
        {
            ViewModel = viewModel;
        }

        public ExamplePluginHost ViewModel { get; }
    }
}
