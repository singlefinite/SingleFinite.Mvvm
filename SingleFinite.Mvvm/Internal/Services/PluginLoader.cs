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

using SingleFinite.Mvvm.Services;
using Microsoft.Extensions.DependencyInjection;

namespace SingleFinite.Mvvm.Internal.Services;

/// <summary>
/// Implementation of <see cref="IPluginLoader"/>.
/// </summary>
/// <param name="pluginRegistry">Used to get the registered plugins.</param>
/// <param name="serviceProvider">Used to build plugins.</param>
internal class PluginLoader(
    IPluginRegistry pluginRegistry,
    IServiceProvider serviceProvider
) : IPluginLoader
{
    #region Methods

    /// <inheritdoc/>
    public void LoadPlugins(IPluginHost pluginHost)
    {
        if (pluginHost is IPlugin)
        {
            throw new ArgumentException(
                message: "A plugin host cannot also be a plugin.",
                paramName: nameof(pluginHost)
            );
        }

        foreach (var descriptor in pluginRegistry.GetPlugins(pluginHost))
        {
            var pluginScope = serviceProvider.CreateLinkedScope();
            var eventObserver = pluginScope.ServiceProvider.GetRequiredService<IEventObserver>();
            var builder = pluginScope.ServiceProvider.GetRequiredService<IBuilder>();
            var plugin = (IPlugin)builder.Build(descriptor.PluginType);

            eventObserver.Observe(
                token: pluginHost.Activated,
                callback: plugin.Activate
            );

            eventObserver.Observe(
                token: pluginHost.Deactivated,
                callback: plugin.Deactivate
            );

            eventObserver.Observe(
                token: pluginHost.Disposed,
                callback: plugin.Dispose
            );

            plugin.Load(pluginHost);
            plugin.Initialize();
        }
    }

    #endregion
}
