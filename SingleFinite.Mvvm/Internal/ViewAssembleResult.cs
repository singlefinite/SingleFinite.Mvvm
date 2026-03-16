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

using Microsoft.Extensions.DependencyInjection;
using SingleFinite.Essentials;
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.Internal;

/// <summary>
/// Used to build a view.
/// </summary>
internal class ViewAssembleResult : IViewAssembleResult
{
    #region Fields

    /// <summary>
    /// Set to true after the Start method has been called.
    /// </summary>
    private bool _isStarted = false;

    /// <summary>
    /// The scope for the view model.
    /// </summary>
    private readonly IServiceScope _viewModelScope;

    /// <summary>
    /// The view model.
    /// </summary>
    private readonly IViewModel _viewModel;

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="serviceProvider">Used to build the view.</param>
    /// <param name="viewModelDescriptor">The view to build.</param>
    public ViewAssembleResult(
        IServiceProvider serviceProvider,
        IViewModelDescriptor viewModelDescriptor
    )
    {
        _viewModelScope = serviceProvider.CreateLinkedScope();
        var builder = _viewModelScope.ServiceProvider.GetRequiredService<IBuilder>();

        // Create view model.
        //
        _viewModel = (IViewModel)builder.Build(
            instanceType: viewModelDescriptor.ViewModelType,
            viewModelDescriptor.ViewModelParameters
        );

        // Dispose of scope when view model is disposed.
        //
        _viewModel.Disposed
            .Observe()
            .OnEach(_viewModelScope.Dispose)
            .Once();

        // Create view.
        //
        var viewRegistry = serviceProvider.GetRequiredService<IViewRegistry>();
        var viewType = viewRegistry.GetViewType(viewModelDescriptor.ViewModelType);
        View = (IView)builder.Build(
            instanceType: viewType,
            _viewModel
        );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public IView View { get; }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public void Start()
    {
        if (_isStarted) return;

        _viewModel.Create();

        // Attach plugins if needed.
        //
        if (_viewModel is IPluginHost pluginHost)
        {
            var pluginLoader = _viewModelScope.ServiceProvider.GetRequiredService<IPluginLoader>();
            pluginLoader.LoadPlugins(pluginHost);
        }

        _isStarted = true;
    }

    #endregion
}

/// <summary>
/// Used to build a view.
/// </summary>
/// <param name="serviceProvider">Used to build the view.</param>
/// <param name="viewModelDescriptor">The view to build.</param>
internal class ViewAssembleResult<TViewModel>(
    IServiceProvider serviceProvider,
    IViewModelDescriptor viewModelDescriptor
) : ViewAssembleResult(serviceProvider, viewModelDescriptor),
    IViewAssembleResult<TViewModel>
    where TViewModel : IViewModel
{
    #region Properties

    /// <inheritdoc/>
    public new IView<TViewModel> View => (IView<TViewModel>)base.View;

    #endregion
}
