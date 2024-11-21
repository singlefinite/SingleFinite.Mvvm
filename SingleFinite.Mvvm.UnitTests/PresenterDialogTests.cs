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

using Microsoft.Extensions.DependencyInjection;
using SingleFinite.Mvvm.Internal.Services;
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class PresenterDialogTests
{
    [TestMethod]
    public void IsOpen_Is_True_While_Dialogs_Are_Not_Closed()
    {
        using var context = new TestContext();

        var presenterDialog = new PresenterDialog(
            context.ServiceProvider.GetRequiredService<IViewBuilder>()
        );

        Assert.IsFalse(presenterDialog.IsDialogOpen);

        var dialog1Context = presenterDialog.Show<Dialog1>();

        Assert.IsTrue(presenterDialog.IsDialogOpen);
        Assert.IsFalse(dialog1Context.Task.IsCompleted);

        var dialog2Context = presenterDialog.Show<Dialog2>();

        Assert.IsTrue(presenterDialog.IsDialogOpen);
        Assert.IsFalse(dialog2Context.Task.IsCompleted);

        dialog1Context.Close();

        Assert.IsTrue(presenterDialog.IsDialogOpen);
        Assert.IsTrue(dialog1Context.Task.IsCompleted);
        Assert.IsFalse(dialog2Context.Task.IsCompleted);

        dialog2Context.Close();

        Assert.IsFalse(presenterDialog.IsDialogOpen);
        Assert.IsTrue(dialog1Context.Task.IsCompleted);
        Assert.IsTrue(dialog2Context.Task.IsCompleted);
    }

    [TestMethod]
    public void Dialog_View_Model_Has_Expected_Lifecycle()
    {
        using var context = new TestContext();

        var presenterDialog = new PresenterDialog(
            context.ServiceProvider.GetRequiredService<IViewBuilder>()
        );

        var output = new List<string>();

        var dialogContext = presenterDialog.Show<DialogLifecycle, List<string>>(output);

        Assert.AreEqual(2, output.Count);
        Assert.AreEqual("Initialize", output[0]);
        Assert.AreEqual("Activate", output[1]);

        output.Clear();

        dialogContext.Close();

        Assert.AreEqual(2, output.Count);
        Assert.AreEqual("Deactivate", output[0]);
        Assert.AreEqual("Dispose", output[1]);
    }

    [TestMethod]
    public void Dialog_Can_Be_Closed_From_Within_View_Model()
    {
        using var context = new TestContext();

        var presenterDialog = new PresenterDialog(
            context.ServiceProvider.GetRequiredService<IViewBuilder>()
        );

        var output = new List<string>();

        var dialogContext = presenterDialog.Show<DialogLifecycle, List<string>>(output);

        Assert.AreEqual(2, output.Count);
        Assert.AreEqual("Initialize", output[0]);
        Assert.AreEqual("Activate", output[1]);

        output.Clear();

        dialogContext.View.ViewModel.CloseDialog();

        Assert.AreEqual(2, output.Count);
        Assert.AreEqual("Deactivate", output[0]);
        Assert.AreEqual("Dispose", output[1]);
    }

    #region Types

    private class Dialog1 : DialogViewModel
    {
        public void CloseDialog() => Close();
    }

    private class Dialog1View(Dialog1 dialog) : IView<Dialog1>
    {
        public Dialog1 ViewModel => dialog;
    }

    private class Dialog2 : DialogViewModel
    {
        public void CloseDialog() => Close();
    }

    private class Dialog2View(Dialog2 dialog) : IView<Dialog2>
    {
        public Dialog2 ViewModel => dialog;
    }

    private class DialogLifecycle(List<string> output) : DialogViewModel<List<string>>
    {
        protected override void OnInitialize()
        {
            output.Add("Initialize");
        }

        protected override void OnActivate(CancellationToken cancellationToken)
        {
            output.Add("Activate");
        }

        protected override void OnDeactivate()
        {
            output.Add("Deactivate");
        }

        protected override void OnDispose()
        {
            output.Add("Dispose");
        }

        public void CloseDialog() => Close();
    }

    private class DialogLifecycleView(DialogLifecycle viewModel) : IView<DialogLifecycle>
    {
        public DialogLifecycle ViewModel => viewModel;
    }

    #endregion
}
