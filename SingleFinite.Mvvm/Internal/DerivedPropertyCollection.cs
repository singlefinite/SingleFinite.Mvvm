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
/// This class is used to keep track of derived properties.
/// </summary>
internal class DerivedPropertyCollection
{
    #region Fields

    /// <summary>
    /// Holds all of the derived properties stored by source property name.
    /// </summary>
    private readonly Dictionary<string, HashSet<DerivedProperty>> _derivedPropertiesBySourceProperty = [];

    #endregion

    #region Fields

    /// <summary>
    /// Add a derived property.
    /// </summary>
    /// <param name="sourcePropertyName">
    /// The name of the property that is the source for the derived property.
    /// </param>
    /// <param name="derivedPropetyOwner">
    /// The object that defines the derived property.
    /// </param>
    /// <param name="derivedPropertyName">
    /// The name of the derived property.
    /// </param>
    public void Add(
        string sourcePropertyName,
        object derivedPropetyOwner,
        string derivedPropertyName
    )
    {
        if (!_derivedPropertiesBySourceProperty.TryGetValue(sourcePropertyName, out var derivedProperties))
        {
            derivedProperties = [];
            _derivedPropertiesBySourceProperty.Add(sourcePropertyName, derivedProperties);
        }

        derivedProperties.Add(new(
            Owner: derivedPropetyOwner,
            PropertyName: derivedPropertyName
        ));
    }

    /// <summary>
    /// Raise the DerivedPropertyChangedEvent for all of the derived properties
    /// associated with the given source property.
    /// </summary>
    /// <param name="sourcePropertyNames">
    /// The names of the source properties whose derived properties should have
    /// the DerivedPropertyChanged event raised.
    /// </param>
    /// <param name="raiseEvent">
    /// The function used to raise the event.
    /// </param>
    public void RaiseDerivedPropertyChangedEvents(
        IEnumerable<string> sourcePropertyNames,
        Action<object?, PropertyChangedEventArgs> raiseEvent
    )
    {
        var allDerivedProperties = new HashSet<DerivedProperty>();

        foreach (var sourcePropertyName in sourcePropertyNames)
        {
            if (_derivedPropertiesBySourceProperty.TryGetValue(sourcePropertyName, out var derivedProperties))
            {
                foreach (var derivedProperty in derivedProperties)
                    allDerivedProperties.Add(derivedProperty);
            }
        }

        if (allDerivedProperties.Count == 0)
            return;

        foreach (var derivedProperty in allDerivedProperties)
        {
            raiseEvent(
                derivedProperty.Owner,
                new PropertyChangedEventArgs(derivedProperty.PropertyName)
            );
        }
    }

    #endregion

    #region Types

    /// <summary>
    /// A derived property.
    /// </summary>
    /// <param name="Owner">The owner of the derived property.</param>
    /// <param name="PropertyName">The name of the derived property.</param>
    private record DerivedProperty(object Owner, string PropertyName);

    #endregion
}
