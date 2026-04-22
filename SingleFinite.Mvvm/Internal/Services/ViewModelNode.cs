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
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.Internal.Services;

/// <summary>
/// This class holds a ViewModel and a reference to its parent.
/// </summary>
/// <param name="scopeContext">
/// This node will be cleaned up when the cancellation token is cancelled.
/// </param>
internal class ViewModelNode(IScopeContext scopeContext)
{
    #region Fields

    /// <summary>
    /// Set to true when this node has been initialized.
    /// </summary>
    private bool _isInitialized = false;

    #endregion

    #region Properties

    /// <summary>
    /// The view model for this node if there is one.
    /// </summary>
    public IViewModel? ViewModel { get; private set; }

    /// <summary>
    /// The parent for this node if there is one.
    /// </summary>
    public ViewModelNode? Parent { get; private set; }

    /// <summary>
    /// When this property is true it means the current view model and every
    /// view model from here to the root parent is active.
    /// </summary>
    public bool IsActiveFromRoot
    {
        get;
        private set
        {
            if (field == value)
                return;

            field = value;
            _isActiveFromRootChangedSource.Emit(value);
        }
    } = true;

    #endregion

    #region Methods

    /// <summary>
    /// Initialize this node.
    /// </summary>
    /// <param name="viewModel">The view model for this node.</param>
    /// <param name="parent">The parent of this node if there is one.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the node has already been initialized.
    /// </exception>
    public void Initialize(IViewModel viewModel, ViewModelNode? parent)
    {
        if (_isInitialized)
            throw new InvalidOperationException("This ViewModelNode has already been initialized.");

        ViewModel = viewModel;
        Parent = parent;
        _isInitialized = true;

        UpdateIsActiveFromRoot();

        viewModel.IsActiveChanged
            .Observe()
            .OnEach(UpdateIsActiveFromRoot)
            .Until(scopeContext.CancellationToken);

        parent?.IsActiveFromRootChanged
            .Observe()
            .OnEach(UpdateIsActiveFromRoot)
            .Until(scopeContext.CancellationToken);
    }

    /// <summary>
    /// Update the current value for IsActiveFromRoot.
    /// </summary>
    private void UpdateIsActiveFromRoot()
    {
        IsActiveFromRoot = (ViewModel?.IsActive ?? true) && (Parent?.IsActiveFromRoot ?? true);
    }

    #endregion

    #region Events

    /// <summary>
    /// Observable that emits when the IsActiveFromRoot property changes.
    /// </summary>
    public IEventObservable<bool> IsActiveFromRootChanged => _isActiveFromRootChangedSource.Observable;
    private readonly EventObservableSource<bool> _isActiveFromRootChangedSource = new();

    #endregion
}
