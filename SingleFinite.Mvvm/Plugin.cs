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

using SingleFinite.Essentials;
using SingleFinite.Mvvm.Internal;

namespace SingleFinite.Mvvm;

/// <summary>
/// Base class for a plugin.
/// </summary>
/// <typeparam name="TPluginHost">
/// The type of view model that hosts the plugin.
/// </typeparam>
public abstract class Plugin<TPluginHost> : ViewModel, IPlugin<TPluginHost>
    where TPluginHost : IPluginHost
{
    #region Properties

    /// <summary>
    /// The view model that hosts the plugin.  It gets set right before the 
    /// initialize method is called.
    /// If this property is accessed before the initialize method is called an 
    /// exception will be thrown.
    /// </summary>
    protected TPluginHost PluginHost => _pluginHost.ThrowIfNull();
    private TPluginHost? _pluginHost;

    #endregion

    #region Methods

    /// <inheritdoc/>
    void IPlugin.Load(IPluginHost pluginHost)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        if (_pluginHost is not null)
            throw new InvalidOperationException(
                "The plugin has already been loaded in a plugin host."
            );
        if (pluginHost is not TPluginHost typedHost)
            throw new NotSupportedException(
                $"The plugin host must be of type {typeof(TPluginHost)}."
            );

        _pluginHost = typedHost;
    }

    #endregion
}
