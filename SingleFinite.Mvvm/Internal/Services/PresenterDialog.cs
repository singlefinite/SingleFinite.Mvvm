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

using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.Internal.Services;

/// <summary>
/// Implementation of <see cref="IPresenterDialog"/>.
/// </summary>
internal class PresenterDialog : IPresenterDialog
{
    #region Fields

    /// <summary>
    /// Transaction used to keep track of open windows.
    /// </summary>
    private readonly Transaction _transaction = new();

    /// <summary>
    /// Used to build views displayed in dialogs.
    /// </summary>
    private readonly IViewBuilder _viewBuilder;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="viewBuilder">Used to build views.</param>
    public PresenterDialog(
        IViewBuilder viewBuilder
    )
    {
        _viewBuilder = viewBuilder;
        _transaction.IsOpenChanged.Register(isOpen => IsDialogOpen = isOpen);
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public bool IsDialogOpen
    {
        get;
        private set
        {
            if (field == value)
                return;

            field = value;
            _isDialogOpenChangedSource.RaiseEvent(field);
        }
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public IDialogContext Show(IViewModelDescriptor viewModelDescriptor)
    {
        var view = _viewBuilder.Build(viewModelDescriptor);
        var viewModel = (IDialogViewModel)view.ViewModel;

        var closedSource = new TaskCompletionSource();
        var isOpenDisposable = _transaction.Start();

        var context = new DialogContext(
            view: view,
            closed: closedSource.Task,
            close: () =>
            {
                view.ViewModel.Deactivate();
                view.ViewModel.Dispose();
                closedSource.SetResult();
                isOpenDisposable.Dispose();
            }
        );

        viewModel.Closed.Register(disposable =>
        {
            context.Close();
            disposable.Dispose();
        });

        viewModel.Activate();

        _dialogOpenedSource.RaiseEvent(view);

        return context;
    }

    /// <inheritdoc/>
    public IDialogContext Show<TViewModel>()
        where TViewModel : IDialogViewModel =>
        Show(new ViewModelDescriptor<TViewModel>());

    /// <inheritdoc/>
    public IDialogContext Show<TViewModel, TViewModelContext>(
        TViewModelContext context
    )
        where TViewModel : IDialogViewModel<TViewModelContext> =>
        Show(new ViewModelDescriptor<TViewModel, TViewModelContext>(context));

    #endregion

    #region Events

    /// <inheritdoc/>
    public EventToken<bool> IsDialogOpenChanged => _isDialogOpenChangedSource.Token;
    private readonly EventTokenSource<bool> _isDialogOpenChangedSource = new();

    /// <inheritdoc/>
    public EventToken<IView> DialogOpened => _dialogOpenedSource.Token;
    private readonly EventTokenSource<IView> _dialogOpenedSource = new();

    #endregion
}
