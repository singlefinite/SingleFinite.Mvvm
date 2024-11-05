// MIT License
// Copyright (c) 2024 SingleFinite
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

namespace SingleFinite.Mvvm;

/// <summary>
/// A view model.  View models are created through dependency injection with each view model getting
/// it's own dependency injection scope.  When the view model is disposed the dependency injection
/// scope will be disposed.
/// </summary>
public abstract class ViewModel : Observable, IViewModel
{
    #region Finalizers

    /// <summary>
    /// Call Dispose.
    /// </summary>
    ~ViewModel() => Dispose(false);

    #endregion

    #region Fields

    /// <summary>
    /// Set to true when this object has been initialized.
    /// </summary>
    private bool _isInitialized = false;

    /// <summary>
    /// Set to true when this object is activated and false when it is deactivated.
    /// </summary>
    private bool _isActive = false;

    /// <summary>
    /// Set to true when this object has been disposed.
    /// </summary>
    private bool _isDisposed = false;

    /// <summary>
    /// Holds token that is provided with the OnActivated method and which will be cancelled when
    /// the view model is deactivated.
    /// </summary>
    private CancellationTokenSource? _activeCancellationTokenSource = null;

    #endregion

    #region Properties

    /// <summary>
    /// Indicates if this view model has been initialized.
    /// </summary>
    protected bool IsInitialized => _isInitialized;

    /// <summary>
    /// Indicates if this view model is currently active.
    /// </summary>
    public bool IsActive => _isActive;

    /// <summary>
    /// Indicates if this view model has been disposed.
    /// </summary>
    public bool IsDisposed => _isDisposed;

    #endregion

    #region Methods

    /// <inheritdoc/>
    void IViewModel.Initialize()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        if (_isInitialized) return;

        _isInitialized = true;
        OnInitialize();
        _initializedEventTokenSource.RaiseEvent();
    }

    /// <inheritdoc/>
    void IViewModel.Activate()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        if (_isActive) return;

        _activeCancellationTokenSource = new();
        _isActive = true;
        OnActivate(_activeCancellationTokenSource.Token);
        _activatedEventTokenSource.RaiseEvent();
    }

    /// <inheritdoc/>
    void IViewModel.Deactivate()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        if (!_isActive) return;

        _activeCancellationTokenSource?.Cancel();
        _activeCancellationTokenSource?.Dispose();
        _activeCancellationTokenSource = null;

        _isActive = false;
        OnDeactivate();
        _deactivatedEventTokenSource.RaiseEvent();
    }

    /// <summary>
    /// Dispose of the scope that belongs to this view model.
    /// </summary>
    void IDisposable.Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose of the view model.
    /// </summary>
    /// <param name="isDisposing">
    /// true when called from Dispose.
    /// false when called from finalizer.
    /// </param>
    protected virtual void Dispose(bool isDisposing)
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        if (isDisposing)
            OnDispose();

        _disposedEventTokenSource.RaiseEvent();
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
    /// <param name="cancellationToken">A cancellation token that gets cancelled when the view model is deactivated.</param>
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
    public EventToken Initialized => _initializedEventTokenSource.Token;
    private readonly EventTokenSource _initializedEventTokenSource = new();

    /// <inheritdoc/>
    public EventToken Activated => _activatedEventTokenSource.Token;
    private readonly EventTokenSource _activatedEventTokenSource = new();

    /// <inheritdoc/>
    public EventToken Deactivated => _deactivatedEventTokenSource.Token;
    private readonly EventTokenSource _deactivatedEventTokenSource = new();

    /// <inheritdoc/>
    public EventToken Disposed => _disposedEventTokenSource.Token;
    private readonly EventTokenSource _disposedEventTokenSource = new();

    #endregion
}

/// <summary>
/// A view model with context.
/// </summary>
/// <typeparam name="TContext">The context for the view model.</typeparam>
public abstract class ViewModel<TContext> : ViewModel, IViewModel<TContext>
{
}
