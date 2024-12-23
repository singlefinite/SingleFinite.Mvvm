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

using System.ComponentModel;

namespace SingleFinite.Mvvm.Internal;

/// <summary>
/// This class is used to keep track of mapped properties.
/// </summary>
internal class PropertyMapper
{
    #region Fields

    /// <summary>
    /// Holds all of the mapped properties stored by source property name.
    /// </summary>
    private readonly Dictionary<string, HashSet<MappedProperty>> _mappedPropertiesBySourceProperty = [];

    #endregion

    #region Fields

    /// <summary>
    /// Add a property mapping.
    /// </summary>
    /// <param name="sourcePropertyName">The source property to map to.</param>
    /// <param name="mappedObject">
    /// The object that defines the mapped property.
    /// </param>
    /// <param name="mappedPropertyName">
    /// The name of the mapped property.
    /// </param>
    public void Add(
        string sourcePropertyName,
        object mappedObject,
        string mappedPropertyName
    )
    {
        if (!_mappedPropertiesBySourceProperty.TryGetValue(sourcePropertyName, out var mappedProperties))
        {
            mappedProperties = [];
            _mappedPropertiesBySourceProperty.Add(sourcePropertyName, mappedProperties);
        }

        mappedProperties.Add(new(
            Owner: mappedObject,
            PropertyName: mappedPropertyName
        ));
    }

    /// <summary>
    /// Raise the MappedPropertyChangedEvent for all of the mapped properties
    /// associated with the given source property.
    /// </summary>
    /// <param name="sourcePropertyNames">
    /// The names of the source properties whose mapped properties should have
    /// the MappedPropertyChanged event raised.
    /// </param>
    /// <param name="raiseEvent">
    /// The function used to raise the event.
    /// </param>
    public void RaiseMappedPropertyChangedEvents(
        IEnumerable<string> sourcePropertyNames,
        Action<object?, PropertyChangedEventArgs> raiseEvent
    )
    {
        var allMappedProperties = new HashSet<MappedProperty>();

        foreach (var sourcePropertyName in sourcePropertyNames)
        {
            if (_mappedPropertiesBySourceProperty.TryGetValue(sourcePropertyName, out var mappedProperties))
            {
                foreach (var mappedProperty in mappedProperties)
                    allMappedProperties.Add(mappedProperty);
            }
        }

        if (allMappedProperties.Count == 0)
            return;

        foreach (var mappedProperty in allMappedProperties)
        {
            raiseEvent(
                mappedProperty.Owner,
                new PropertyChangedEventArgs(mappedProperty.PropertyName)
            );
        }
    }

    #endregion

    #region Types

    /// <summary>
    /// A mapped property.
    /// </summary>
    /// <param name="Owner">The owner of the mapped property.</param>
    /// <param name="PropertyName">The name of the mapped property.</param>
    private record MappedProperty(object Owner, string PropertyName);

    #endregion
}
