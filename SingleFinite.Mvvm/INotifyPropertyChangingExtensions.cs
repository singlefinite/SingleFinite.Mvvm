// MIT License
// Copyright (c) 2025 Single Finite
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
    extension<TComponent>(TComponent component)
        where TComponent : INotifyPropertyChanging
    {
        /// <summary>
        /// Observe when a property is changing.
        /// </summary>
        /// <returns>
        /// An observer that when disposed will unregister the callback.
        /// </returns>
        public Essentials.IObserver<string?> ObservePropertyChanging() =>
            Observable<string?>.Observe<PropertyChangingEventHandler>(
                register: handler => component.PropertyChanging += handler,
                unregister: handler => component.PropertyChanging -= handler,
                handler: raiseNext => (sender, args) => raiseNext(args.PropertyName)
            );

        /// <summary>
        /// Observe when the given property is changing and provide the current
        /// value to the observer.
        /// </summary>
        /// <typeparam name="TProperty">
        /// The type of the property value to observe.
        /// </typeparam>
        /// <param name="property">
        /// A function that selects the property to observe from the component
        /// instance.
        /// </param>
        /// <param name="propertyExpression">
        /// The source expression used to identify the property, typically
        /// provided by the compiler. This parameter is optional.
        /// </param>
        /// <returns>
        /// An observer that emits the property's value each time it changes.
        /// </returns>
        public Essentials.IObserver<TProperty> ObservePropertyChanging<TProperty>(
            Func<TComponent, TProperty> property,
            [CallerArgumentExpression(nameof(property))]
            string? propertyExpression = default
        )
        {
            var propertyName = INotifyPropertyChangedExtensions
                .ParsePropertyName(propertyExpression);

            return ObservePropertyChanging(component)
                .Where(changedPropertyName => changedPropertyName == propertyName)
                .Select(_ => property(component));
        }
    }
}
