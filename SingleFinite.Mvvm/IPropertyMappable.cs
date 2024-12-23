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

namespace SingleFinite.Mvvm;

/// <summary>
/// Interface for classes that support mapped properties.  Mapped properties are
/// defined outside of the class an are typically used to convert a source
/// property back and forth between a view and its view model.
/// </summary>
public interface IPropertyMappable
{
    /// <summary>
    /// This method creates a mapped property.  The MappedPropertyChanged event
    /// will be raised with the details for the mapped property whenever any of
    /// the source properties are changed.
    /// </summary>
    /// <param name="mappedObject">
    /// The object that defines the mapped property that is based off of the
    /// source property.
    /// </param>
    /// <param name="mappedPropertyName">
    /// The name of the property that will be the mapped property.
    /// </param>
    /// <param name="sourcePropertyNames">
    /// The names of the properties that will be mapped to and be the source for
    /// the mapped property.
    /// </param>
    void MapProperty(
        object mappedObject,
        string mappedPropertyName,
        params IEnumerable<string> sourcePropertyNames
    );

    /// <summary>
    /// Event that is raised when a source property that has been mapped to has
    /// been changed.  The mapped object and property will be passed with the
    /// event.  Not that this event is raised when the source property has been
    /// changed even if the mapped property doesn't itself result in a change.
    /// </summary>
    event PropertyChangedEventHandler? MappedPropertyChanged;
}
