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
using System.Runtime.CompilerServices;
using SingleFinite.Essentials;

namespace SingleFinite.Mvvm;

/// <summary>
/// Extension functions for the INotifyPropertyChanging class.
/// </summary>
public static partial class INotifyPropertyChangingExtensions
{
    /// <summary>
    /// Observe when a property is changing.
    /// </summary>
    /// <param name="component">
    /// The component to listen for property changes on.
    /// </param>
    /// <returns>
    /// An observer that when disposed will unregister the callback.
    /// </returns>
    public static Essentials.IObserver<string?> ObservePropertyChanging(
        this INotifyPropertyChanging component
    ) => Observable<string?>.Observe<PropertyChangingEventHandler>(
        register: handler => component.PropertyChanging += handler,
        unregister: handler => component.PropertyChanging -= handler,
        handler: raiseNext => (sender, args) => raiseNext(args.PropertyName)
    );

    /// <summary>
    /// Observe when the given property is changing.
    /// </summary>
    /// <param name="component">
    /// The component to listen for property changes on.
    /// </param>
    /// <param name="property">
    /// An expression in the form of `() => component.property` that identifies
    /// the property to listen for changes on.
    /// </param>
    /// <param name="propertyExpression">
    /// When left as null the compiler will set this from the property argument.
    /// </param>
    /// <returns>
    /// An observer that when disposed will unregister the callback.
    /// </returns>
    public static Essentials.IObserver<string?> ObservePropertyChanging(
        this INotifyPropertyChanging component,
        Func<object?> property,
        [CallerArgumentExpression(nameof(property))]
        string? propertyExpression = default
    )
    {
        var propertyName = INotifyPropertyChangedExtensions.ParsePropertyName(propertyExpression);
        return ObservePropertyChanging(component)
            .Where(changedPropertyName => changedPropertyName == propertyName);
    }
}
