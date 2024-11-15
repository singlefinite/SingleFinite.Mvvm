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
        Assert.IsFalse(dialog1Context.Closed.IsCompleted);

        var dialog2Context = presenterDialog.Show<Dialog2>();

        Assert.IsTrue(presenterDialog.IsDialogOpen);
        Assert.IsFalse(dialog2Context.Closed.IsCompleted);

        dialog1Context.Close();

        Assert.IsTrue(presenterDialog.IsDialogOpen);
        Assert.IsTrue(dialog1Context.Closed.IsCompleted);
        Assert.IsFalse(dialog2Context.Closed.IsCompleted);

        dialog2Context.Close();

        Assert.IsFalse(presenterDialog.IsDialogOpen);
        Assert.IsTrue(dialog1Context.Closed.IsCompleted);
        Assert.IsTrue(dialog2Context.Closed.IsCompleted);
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

    #endregion
}
