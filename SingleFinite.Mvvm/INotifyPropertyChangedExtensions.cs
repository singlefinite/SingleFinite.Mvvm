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
using System.Text.RegularExpressions;
using SingleFinite.Essentials;

namespace SingleFinite.Mvvm;

/// <summary>
/// Extension functions for the INotifyPropertyChanged class.
/// </summary>
public static partial class INotifyPropertyChangedExtensions
{
    /// <summary>
    /// Observe when a property is changed.
    /// </summary>
    /// <param name="component">
    /// The component to listen for property changes on.
    /// </param>
    /// <returns>
    /// An observer that when disposed will unregister the callback.
    /// </returns>
    public static Essentials.IObserver<string?> ObservePropertyChanged(
        this INotifyPropertyChanged component
    ) => Observable<string?>.Observe<PropertyChangedEventHandler>(
        register: handler => component.PropertyChanged += handler,
        unregister: handler => component.PropertyChanged -= handler,
        handler: raiseNext => (sender, args) => raiseNext(args.PropertyName)
    );

    /// <summary>
    /// Observe when the given property is changed.
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
    public static Essentials.IObserver<string?> ObservePropertyChanged(
        this INotifyPropertyChanged component,
        Func<object?> property,
        [CallerArgumentExpression(nameof(property))]
        string? propertyExpression = default
    )
    {
        var propertyName = ParsePropertyName(propertyExpression);
        return ObservePropertyChanged(component)
            .Where(changedPropertyName => changedPropertyName == propertyName);
    }

    /// <summary>
    /// Parse the property name from the given expression.
    /// </summary>
    /// <param name="propertyExpression">
    /// The expression to parse the property name from.
    /// The expected format for the expression is '() => owner.property' or
    /// '() => owner.property = something'.
    /// </param>
    /// <returns>The property name parsed from the expression.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the expression is not in the expected format.
    /// </exception>
    internal static string ParsePropertyName(string? propertyExpression)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(
            propertyExpression,
            nameof(propertyExpression)
        );

        var matchValue = PropertyNameRegex().Match(propertyExpression)?.Value;
        if (string.IsNullOrEmpty(matchValue))
        {
            throw new ArgumentException(
                message: "expression must be in the form of 'object.property'",
                paramName: nameof(propertyExpression)
            );
        }

        return matchValue;
    }

    /// <summary>
    /// Regular expression used to parse a property name out of an expression.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex("(?<=\\.)\\w+")]
    private static partial Regex PropertyNameRegex();
}
