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
using Microsoft.Extensions.DependencyInjection;

namespace SingleFinite.Mvvm.Internal.Services;

/// <summary>
/// Implementation of <see cref="IViewBuilder"/> that uses the service provider 
/// to build view objects.
/// </summary>
/// <param name="serviceProvider">Used to build new objects.</param>
internal sealed class ViewBuilder(
    IServiceProvider serviceProvider
) : IViewBuilder
{
    #region Methods

    /// <inheritdoc/>
    public IView Build(IViewModelDescriptor viewModelDescriptor)
    {
        var viewModelScope = serviceProvider.CreateLinkedScope();
        var builder = viewModelScope.ServiceProvider.GetRequiredService<IBuilder>();

        // Create view model.
        //
        IViewModel viewModel;
        if (viewModelDescriptor.ViewModelContext is not null)
        {
            viewModel = (IViewModel)builder.Build(
                instanceType: viewModelDescriptor.ViewModelType,
                viewModelDescriptor.ViewModelContext
            );
        }
        else
        {
            viewModel = (IViewModel)builder.Build(
                instanceType: viewModelDescriptor.ViewModelType
            );
        }

        viewModel.Disposed.Register(
            callback: registration =>
            {
                registration.Dispose();
                viewModelScope.Dispose();
            }
        );

        // Create view.
        //
        var viewRegistry = serviceProvider.GetRequiredService<IViewRegistry>();
        var viewType = viewRegistry.GetViewType(viewModelDescriptor.ViewModelType);
        var view = (IView)builder.Build(
            instanceType: viewType,
            viewModel
        );

        viewModel.Initialize();

        // Attach plugins if needed.
        //
        if (viewModel is IPluginHost pluginHost)
        {
            var pluginLoader = viewModelScope.ServiceProvider.GetRequiredService<IPluginLoader>();
            pluginLoader.LoadPlugins(pluginHost);
        }

        return view;
    }

    /// <inheritdoc/>
    public IView<TViewModel> Build<TViewModel>()
        where TViewModel : IViewModel =>
        (IView<TViewModel>)Build(new ViewModelDescriptor<TViewModel>());

    /// <inheritdoc/>
    public IView<TViewModel> Build<TViewModel, TViewModelContext>(
        TViewModelContext context
    )
        where TViewModel : IViewModel<TViewModelContext> =>
        (IView<TViewModel>)Build(
            new ViewModelDescriptor<TViewModel, TViewModelContext>(context)
        );

    #endregion
}
