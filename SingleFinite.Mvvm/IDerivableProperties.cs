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
/// Interface for classes that support derived properties.  Derived properties
/// are properties that are derived from the value of other properties.
/// Typically a derived property is defined in a view and derives its value from
/// one or more properties in the view model.
/// </summary>
public interface IDerivableProperties
{
    /// <summary>
    /// This method creates a derived property.
    /// </summary>
    /// <param name="derivedPropertyOwner">
    /// The object that defines the derived property that is derived from the
    /// source property.
    /// </param>
    /// <param name="derivedPropertyName">
    /// The name of the derived property.
    /// </param>
    /// <param name="sourcePropertyNames">
    /// The names of the properties that the derived property derives its value
    /// from.
    /// </param>
    void AddDerivedProperty(
        object derivedPropertyOwner,
        string derivedPropertyName,
        params IEnumerable<string> sourcePropertyNames
    );

    /// <summary>
    /// Event that is raised when any source for a derived property has changed.
    /// The name of the derived property 
    /// Note that this event is raised when the source property has been
    /// changed even if the derived property doesn't itself result in a change.
    /// </summary>
    event PropertyChangedEventHandler? DerivedPropertyChanged;
}
