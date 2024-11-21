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
    public IDialogContext Show(IViewModelDescriptor viewModelDescriptor) =>
        Show<IDialogContext>(
            createContext: () => new DialogContext(
                view: _viewBuilder.Build(viewModelDescriptor),
                isModal: false
            )
        );

    /// <inheritdoc/>
    public IDialogContext<TDialogViewModel> Show<TDialogViewModel>()
        where TDialogViewModel : IDialogViewModel =>
        Show<IDialogContext<TDialogViewModel>>(
            createContext: () => new DialogContext<TDialogViewModel>(
                view: _viewBuilder.Build<TDialogViewModel>(),
                isModal: false
            )
        );

    /// <inheritdoc/>
    public IDialogContext<TDialogViewModel> Show<TDialogViewModel, TDialogViewModelContext>(
        TDialogViewModelContext context
    )
        where TDialogViewModel : IDialogViewModel<TDialogViewModelContext> =>
        Show<IDialogContext<TDialogViewModel>>(
            createContext: () => new DialogContext<TDialogViewModel>(
                view: _viewBuilder.Build<TDialogViewModel, TDialogViewModelContext>(context),
                isModal: false
            )
        );

    /// <inheritdoc/>
    public IViewModel ShowModal(IViewModelDescriptor viewModelDescriptor) =>
        Show<IDialogContext>(
            createContext: () => new DialogContext(
                view: _viewBuilder.Build(viewModelDescriptor),
                isModal: true
            )
        ).View.ViewModel;

    /// <inheritdoc/>
    public TDialogViewModel ShowModal<TDialogViewModel>()
        where TDialogViewModel : IDialogViewModel =>
        Show<IDialogContext<TDialogViewModel>>(
            createContext: () => new DialogContext<TDialogViewModel>(
                view: _viewBuilder.Build<TDialogViewModel>(),
                isModal: true
            )
        ).View.ViewModel;

    /// <inheritdoc/>
    public TDialogViewModel ShowModal<TDialogViewModel, TDialogViewModelContext>(
        TDialogViewModelContext context
    )
        where TDialogViewModel : IDialogViewModel<TDialogViewModelContext> =>
        Show<IDialogContext<TDialogViewModel>>(
            createContext: () => new DialogContext<TDialogViewModel>(
                view: _viewBuilder.Build<TDialogViewModel, TDialogViewModelContext>(context),
                isModal: true
            )
        ).View.ViewModel;

    /// <summary>
    /// Show a dialog for a view.
    /// </summary>
    /// <typeparam name="TDialogContext">
    /// The type of dialog context that will be returned.
    /// </typeparam>
    /// <param name="createContext">
    /// Action that creates the dialog context.
    /// </param>
    /// <returns>The dialog context for the dialog being shown.</returns>
    private TDialogContext Show<TDialogContext>(
        Func<TDialogContext> createContext
    )
        where TDialogContext : IDialogContext
    {
        var dialogContext = createContext();

        var token = _transaction.Start();
        dialogContext.Closed.Register(token.Dispose);

        dialogContext.View.ViewModel.Activate();

        _dialogOpenedSource.RaiseEvent(dialogContext);

        return dialogContext;
    }

    #endregion

    #region Events

    /// <inheritdoc/>
    public EventToken<bool> IsDialogOpenChanged => _isDialogOpenChangedSource.Token;
    private readonly EventTokenSource<bool> _isDialogOpenChangedSource = new();

    /// <inheritdoc/>
    public EventToken<IDialogContext> DialogOpened => _dialogOpenedSource.Token;
    private readonly EventTokenSource<IDialogContext> _dialogOpenedSource = new();

    #endregion
}
