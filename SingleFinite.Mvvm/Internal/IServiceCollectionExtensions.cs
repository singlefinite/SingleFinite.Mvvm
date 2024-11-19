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

using SingleFinite.Mvvm.Internal.Services;
using SingleFinite.Mvvm.Services;
using Microsoft.Extensions.DependencyInjection;

namespace SingleFinite.Mvvm.Internal;

/// <summary>
/// Extension methods for the <see cref="IServiceCollection"/> class.
/// </summary>
internal static class IServiceCollectionExtensions
{
    /// <summary>
    /// Register all services from this library with the given service 
    /// collection.
    /// </summary>
    /// <param name="services">
    /// The service collection to register services with.
    /// </param>
    /// <param name="host">The host that holds the application state.</param>
    /// <param name="views">
    /// The views collection that defines which views use which view models.
    /// </param>
    /// <param name="plugins">
    /// The plugins collection that defines which plugins are attached to which 
    /// plugin hosts.
    /// </param>
    /// <returns>
    /// The service collection that was passed in so a fluent style call chain 
    /// can be used to register services.
    /// </returns>
    internal static IServiceCollection AddMvvm(
        this IServiceCollection services,
        IAppHost host,
        IViewCollection views,
        IPluginCollection plugins
    ) => services
        .AddSingleton(host)
        .AddSingleton<IAppMainDispatcher, DedicatedThreadDispatcher>()
        .AddSingleton<IAppBackgroundDispatcher, ThreadPoolDispatcher>()
        .AddSingleton<IViewRegistry>(new ViewRegistry(views))
        .AddSingleton<IPluginRegistry>(new PluginRegistry(plugins))
        .AddSingleton<IPluginLoader, PluginLoader>()
        .AddSingleton<IDialogs, Dialogs>()
        .AddScoped<IBuilder, Builder>()
        .AddScoped<IViewBuilder, ViewBuilder>()
        .AddScoped<ICancellationTokenProvider, CancellationTokenProvider>()
        .AddScoped<IBackgroundDispatcher, BackgroundDispatcher>()
        .AddScoped<IMainDispatcher, MainDispatcher>()
        .AddScoped<IEventObserver, EventObserver>()
        .AddTransient<IPresenterFrame, PresenterFrame>()
        .AddTransient<IPresenterStack, PresenterStack>()
        .AddTransient<IPresenterDialog, PresenterDialog>();
}
