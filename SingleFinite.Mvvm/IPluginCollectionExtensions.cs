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

using System.Reflection;

namespace SingleFinite.Mvvm;

/// <summary>
/// Extensions for the <see cref="IPluginCollection"/> interface.
/// </summary>
public static class IPluginCollectionExtensions
{
    #region Fields

    /// <summary>
    /// Filter used to find interfaces that are of type 
    /// <see cref="IPlugin{TViewModel}"/>.
    /// </summary>
    private static readonly TypeFilter s_pluginTypeFilter = new(
        (type, _) =>
            type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(IPlugin<>)
    );

    #endregion

    #region Methods

    /// <summary>
    /// Scan the given assemblies for <see cref="IPlugin{TViewModel}"/> types 
    /// and add <see cref="PluginDescriptor"/> entries for them in the view 
    /// collection.
    /// </summary>
    /// <param name="pluginCollection">
    /// The collection to add the scan results to.
    /// </param>
    /// <param name="assemblies">The assemblies to scan.</param>
    public static void Scan(
        this IPluginCollection pluginCollection,
        params IEnumerable<Assembly?> assemblies
    )
    {
        foreach (var assembly in assemblies)
        {
            if (assembly is null)
                continue;

            foreach (var definedType in assembly.DefinedTypes)
            {
                if (definedType.IsAbstract || definedType.IsInterface)
                    continue;

                var pluginInterfaces = definedType.FindInterfaces(
                    filter: s_pluginTypeFilter,
                    filterCriteria: null
                );

                if (pluginInterfaces.Length != 1)
                    continue;

                var pluginInterface = pluginInterfaces.First();
                var viewModelType = pluginInterface.GenericTypeArguments.First();

                pluginCollection.Add(
                    new PluginDescriptor(
                        PluginType: definedType,
                        PluginHostType: viewModelType
                    )
                );
            }
        }
    }

    /// <summary>
    /// Add a PluginDescriptor with the given Plugin type and PluginHost type.
    /// </summary>
    /// <typeparam name="TPlugin">The Plugin type to register.</typeparam>
    /// <typeparam name="TPluginHost">
    /// The PluginHost type to register.
    /// </typeparam>
    /// <param name="pluginCollection">
    /// The collection to add a PluginDescriptor to.
    /// </param>
    public static void Add<TPlugin, TPluginHost>(
        this IPluginCollection pluginCollection
    )
        where TPlugin : IPlugin<TPluginHost>
        where TPluginHost : IPluginHost
    {
        pluginCollection.Add(
            new(
                PluginType: typeof(TPlugin),
                PluginHostType: typeof(TPluginHost)
            )
        );
    }

    #endregion
}
