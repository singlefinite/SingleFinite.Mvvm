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

using SingleFinite.Essentials;

namespace SingleFinite.Mvvm;

/// <summary>
/// A view model.  View models are created through dependency injection with 
/// each view model getting it's own dependency injection scope.  When the view 
/// model is disposed the dependency injection scope will be disposed.
/// </summary>
public abstract class ViewModel :
    Changeable,
    IViewModel,
    ILifecycleAware
{
    #region Fields

    /// <summary>
    /// Holds token that is provided with the OnActivated method and which will 
    /// be cancelled when the view model is deactivated.
    /// </summary>
    private CancellationTokenSource? _activeCancellationTokenSource = null;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public ViewModel()
    {
        _disposeState = new(
            owner: this,
            onDispose: OnDispose
        );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    DisposeState IDisposeObservable.DisposeState => _disposeState;
    private readonly DisposeState _disposeState;

    /// <summary>
    /// Indicates if this view model has been initialized.
    /// </summary>
    public bool IsInitialized { get; private set; }

    /// <summary>
    /// Indicates if this view model is currently active.
    /// </summary>
    public bool IsActive
    {
        get => field;
        private set
        {
            if (field == value)
                return;

            field = value;
            _isActiveChangedSource.Emit(field);
        }
    }

    /// <summary>
    /// Indicates if this view model has been disposed.
    /// </summary>
    public bool IsDisposed => _disposeState.IsDisposed;

    #endregion

    #region Methods

    /// <inheritdoc/>
    void ILifecycleAware.Initialize()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        if (IsInitialized) return;

        IsInitialized = true;
        OnInitialize();
        _initializedSource.Emit();
    }

    /// <inheritdoc/>
    void ILifecycleAware.Activate()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        if (IsActive) return;

        _activeCancellationTokenSource = new();
        IsActive = true;
        OnActivate(_activeCancellationTokenSource.Token);
        _activatedSource.Emit();
    }

    /// <inheritdoc/>
    void ILifecycleAware.Deactivate()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        if (!IsActive) return;

        _activeCancellationTokenSource?.Cancel();
        _activeCancellationTokenSource?.Dispose();
        _activeCancellationTokenSource = null;

        IsActive = false;
        OnDeactivate();
        _deactivatedSource.Emit();
    }

    /// <summary>
    /// Called immediately after this view model is created.
    /// </summary>
    protected virtual void OnInitialize()
    {
    }

    /// <summary>
    /// Called after the view has been added to the display.
    /// </summary>
    /// <param name="cancellationToken">
    /// A cancellation token that gets cancelled when the view model is 
    /// deactivated.
    /// </param>
    protected virtual void OnActivate(CancellationToken cancellationToken)
    {
    }

    /// <summary>
    /// Called after the view has been removed from the display.
    /// </summary>
    protected virtual void OnDeactivate()
    {
    }

    /// <summary>
    /// Called when this view model is disposed.
    /// </summary>
    protected virtual void OnDispose()
    {
    }

    #endregion

    #region Events

    /// <inheritdoc/>
    public Observable Initialized => _initializedSource.Observable;
    private readonly ObservableSource _initializedSource = new();

    /// <inheritdoc/>
    public Observable Activated => _activatedSource.Observable;
    private readonly ObservableSource _activatedSource = new();

    /// <inheritdoc/>
    public Observable Deactivated => _deactivatedSource.Observable;
    private readonly ObservableSource _deactivatedSource = new();

    /// <inheritdoc/>
    public Observable Disposed => _disposeState.Disposed;

    /// <inheritdoc/>
    public Observable<bool> IsActiveChanged => _isActiveChangedSource.Observable;
    private ObservableSource<bool> _isActiveChangedSource = new();

    #endregion
}
