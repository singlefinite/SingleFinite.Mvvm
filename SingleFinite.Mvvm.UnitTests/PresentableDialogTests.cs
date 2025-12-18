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

using Microsoft.Extensions.DependencyInjection;
using SingleFinite.Essentials;
using SingleFinite.Mvvm.Internal.Services;
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class PresentableDialogTests
{
    [TestMethod]
    public void Dialog_View_Model_Has_Expected_Lifecycle()
    {
        using var context = new TestContext();

        var presentableDialog = new PresentableDialog(
            context.ServiceProvider.GetRequiredService<IViewBuilder>()
        );

        var output = new List<string>();

        var dialog1 = presentableDialog.Show<Dialog1Lifecycle>(output);

        Assert.IsNotNull(presentableDialog.Current);
        Assert.AreEqual(presentableDialog.Current.ViewModel, dialog1);
        Assert.HasCount(1, presentableDialog.ViewModels);
        Assert.AreEqual(dialog1, presentableDialog.ViewModels[0]);

        Assert.HasCount(2, output);
        Assert.AreEqual("1 Initialize", output[0]);
        Assert.AreEqual("1 Activate", output[1]);
        output.Clear();

        var dialog2 = presentableDialog.Show<Dialog2Lifecycle>(output);

        Assert.IsNotNull(presentableDialog.Current);
        Assert.AreEqual(presentableDialog.Current.ViewModel, dialog2);
        Assert.HasCount(2, presentableDialog.ViewModels);
        Assert.AreEqual(dialog2, presentableDialog.ViewModels[0]);
        Assert.AreEqual(dialog1, presentableDialog.ViewModels[1]);

        Assert.HasCount(3, output);
        Assert.AreEqual("2 Initialize", output[0]);
        Assert.AreEqual("1 Deactivate", output[1]);
        Assert.AreEqual("2 Activate", output[2]);
        output.Clear();

        presentableDialog.Close(dialog2);

        Assert.IsNotNull(presentableDialog.Current);
        Assert.AreEqual(presentableDialog.Current.ViewModel, dialog1);
        Assert.HasCount(1, presentableDialog.ViewModels);
        Assert.AreEqual(dialog1, presentableDialog.ViewModels[0]);

        Assert.HasCount(3, output);
        Assert.AreEqual("2 Deactivate", output[0]);
        Assert.AreEqual("2 Dispose", output[1]);
        Assert.AreEqual("1 Activate", output[2]);
        output.Clear();

        presentableDialog.Close(dialog1);

        Assert.IsNull(presentableDialog.Current);
        Assert.IsEmpty(presentableDialog.ViewModels);

        Assert.HasCount(2, output);
        Assert.AreEqual("1 Deactivate", output[0]);
        Assert.AreEqual("1 Dispose", output[1]);
        output.Clear();
    }

    [TestMethod]
    public void Changed_Event_Is_Raised()
    {
        using var context = new TestContext();

        var presentableDialog = new PresentableDialog(
            context.ServiceProvider.GetRequiredService<IViewBuilder>()
        );

        IPresentable.CurrentChangedEventArgs? observedArgs = null;
        presentableDialog.CurrentChanged
            .Observe()
            .OnEach(args => observedArgs = args);

        var output = new List<string>();

        Assert.IsNull(observedArgs);

        var viewModel1 = presentableDialog.Show<Dialog1Lifecycle>(output);

        Assert.IsNotNull(observedArgs);
        Assert.IsNotNull(observedArgs.View);
        Assert.AreEqual(viewModel1, observedArgs.View.ViewModel);
        Assert.IsTrue(observedArgs.IsNew);

        observedArgs = null;

        var viewModel2 = presentableDialog.Show<Dialog1Lifecycle>(output);

        Assert.IsNotNull(observedArgs);
        Assert.IsNotNull(observedArgs.View);
        Assert.AreEqual(viewModel2, observedArgs.View.ViewModel);
        Assert.IsTrue(observedArgs.IsNew);

        observedArgs = null;

        presentableDialog.Close(viewModel2);

        Assert.IsNotNull(observedArgs);
        Assert.IsNotNull(observedArgs.View);
        Assert.AreEqual(viewModel1, observedArgs.View.ViewModel);
        Assert.IsFalse(observedArgs.IsNew);
    }

    [TestMethod]
    public void Dialog_View_Model_Will_Dispose_All_When_Cleared()
    {
        using var context = new TestContext();

        var presentableDialog = new PresentableDialog(
            context.ServiceProvider.GetRequiredService<IViewBuilder>()
        );

        var dialog1 = presentableDialog.Show<Dialog1Lifecycle>(new List<string>());
        var dialog2 = presentableDialog.Show<Dialog1Lifecycle>(new List<string>());
        var dialog3 = presentableDialog.Show<Dialog1Lifecycle>(new List<string>());

        Assert.HasCount(3, presentableDialog.ViewModels);
        Assert.IsNotNull(presentableDialog.Current);

        Assert.IsFalse(dialog1.IsDisposed);
        Assert.IsFalse(dialog2.IsDisposed);
        Assert.IsFalse(dialog3.IsDisposed);

        presentableDialog.Clear();

        Assert.IsEmpty(presentableDialog.ViewModels);
        Assert.IsNull(presentableDialog.Current);

        Assert.IsTrue(dialog1.IsDisposed);
        Assert.IsTrue(dialog2.IsDisposed);
        Assert.IsTrue(dialog3.IsDisposed);
    }

    [TestMethod]
    public void Dispose_Will_Clear()
    {
        using var context = new TestContext();

        var presentableDialog = new PresentableDialog(
            context.ServiceProvider.GetRequiredService<IViewBuilder>()
        );

        var dialog1 = presentableDialog.Show<Dialog1Lifecycle>(new List<string>());
        var dialog2 = presentableDialog.Show<Dialog1Lifecycle>(new List<string>());
        var dialog3 = presentableDialog.Show<Dialog1Lifecycle>(new List<string>());

        Assert.HasCount(3, presentableDialog.ViewModels);
        Assert.IsNotNull(presentableDialog.Current);

        Assert.IsFalse(dialog1.IsDisposed);
        Assert.IsFalse(dialog2.IsDisposed);
        Assert.IsFalse(dialog3.IsDisposed);

        presentableDialog.Dispose();

        Assert.IsEmpty(presentableDialog.ViewModels);
        Assert.IsNull(presentableDialog.Current);

        Assert.IsTrue(dialog1.IsDisposed);
        Assert.IsTrue(dialog2.IsDisposed);
        Assert.IsTrue(dialog3.IsDisposed);
    }

    [TestMethod]
    public void Close_From_Middle_Of_Stack_Has_Expected_Lifecycle()
    {
        using var context = new TestContext();

        var presentableDialog = new PresentableDialog(
            context.ServiceProvider.GetRequiredService<IViewBuilder>()
        );

        var output = new List<string>();

        var dialog1 = presentableDialog.Show<Dialog1Lifecycle>(output);
        var dialog2 = presentableDialog.Show<Dialog2Lifecycle>(output);
        var dialog3 = presentableDialog.Show<Dialog3Lifecycle>(output);

        output.Clear();

        presentableDialog.Close(dialog2);

        Assert.IsNotNull(presentableDialog.Current);
        Assert.AreEqual(presentableDialog.Current.ViewModel, dialog3);
        Assert.HasCount(2, presentableDialog.ViewModels);
        Assert.AreEqual(dialog3, presentableDialog.ViewModels[0]);
        Assert.AreEqual(dialog1, presentableDialog.ViewModels[1]);

        Assert.HasCount(1, output);
        Assert.AreEqual("2 Dispose", output[0]);
    }

    #region Types

    private class Dialog1Lifecycle(List<string> output) : ViewModel
    {
        protected override void OnInitialize()
        {
            output.Add("1 Initialize");
        }

        protected override void OnActivate(CancellationToken cancellationToken)
        {
            output.Add("1 Activate");
        }

        protected override void OnDeactivate()
        {
            output.Add("1 Deactivate");
        }

        protected override void OnDispose()
        {
            output.Add("1 Dispose");
        }
    }

    private class Dialog1LifecycleView(Dialog1Lifecycle viewModel) : IView<Dialog1Lifecycle>
    {
        public Dialog1Lifecycle ViewModel => viewModel;
    }

    private class Dialog2Lifecycle(List<string> output) : ViewModel
    {
        protected override void OnInitialize()
        {
            output.Add("2 Initialize");
        }

        protected override void OnActivate(CancellationToken cancellationToken)
        {
            output.Add("2 Activate");
        }

        protected override void OnDeactivate()
        {
            output.Add("2 Deactivate");
        }

        protected override void OnDispose()
        {
            output.Add("2 Dispose");
        }
    }

    private class DialogLifecycleView(Dialog2Lifecycle viewModel) : IView<Dialog2Lifecycle>
    {
        public Dialog2Lifecycle ViewModel => viewModel;
    }

    private class Dialog3Lifecycle(List<string> output) : ViewModel
    {
        protected override void OnInitialize()
        {
            output.Add("3 Initialize");
        }

        protected override void OnActivate(CancellationToken cancellationToken)
        {
            output.Add("3 Activate");
        }

        protected override void OnDeactivate()
        {
            output.Add("3 Deactivate");
        }

        protected override void OnDispose()
        {
            output.Add("3 Dispose");
        }
    }

    private class Dialog3LifecycleView(Dialog3Lifecycle viewModel) : IView<Dialog3Lifecycle>
    {
        public Dialog3Lifecycle ViewModel => viewModel;
    }

    #endregion
}
