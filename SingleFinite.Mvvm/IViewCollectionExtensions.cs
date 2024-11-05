// MIT License
// Copyright (c) 2024 Single Finite
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

using System.Reflection;

namespace SingleFinite.Mvvm;

/// <summary>
/// Extensions for the <see cref="IViewCollection"/> interface.
/// </summary>
public static class IViewCollectionExtensions
{
    #region Fields

    /// <summary>
    /// Filter used to find interfaces that are of type <see cref="IView{TViewModel}"/>.
    /// </summary>
    private static readonly TypeFilter s_viewTypeFilter = new(
        (type, _) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IView<>)
    );

    #endregion

    #region Methods

    /// <summary>
    /// Scan the given assemblies for <see cref="IView{TViewModel}"/> types and add <see cref="ViewDescriptor"/> entries for them in the view collection.
    /// </summary>
    /// <param name="viewCollection">The collection to add the scan results to.</param>
    /// <param name="assemblies">The assemblies to scan.</param>
    public static void Scan(
        this IViewCollection viewCollection,
        params Assembly[] assemblies
    )
    {
        foreach (var assembly in assemblies)
        {
            foreach (var definedType in assembly.DefinedTypes)
            {
                if (definedType.IsAbstract || definedType.IsInterface)
                    continue;

                var viewInterfaces = definedType.FindInterfaces(s_viewTypeFilter, null);
                if (viewInterfaces.Length != 1)
                    continue;

                var viewInterface = viewInterfaces.First();
                var viewModelType = viewInterface.GenericTypeArguments.First();

                viewCollection.Add(
                    new ViewDescriptor(
                        ViewModelType: viewModelType,
                        ViewType: definedType
                    )
                );
            }
        }
    }

    #endregion
}
