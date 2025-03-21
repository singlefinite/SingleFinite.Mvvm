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

namespace SingleFinite.Mvvm.Services;

/// <summary>
/// Options for popping view models off of a <see cref="IPresentableStack"/>.
/// </summary>
public abstract record PopOptions
{
    #region Types

    /// <summary>
    /// Used to pop all view models off of the stack.
    /// </summary>
    internal record PopOptionsRemoveAll() : PopOptions;

    /// <summary>
    /// Used to pop a specific number of view models off of the stack.
    /// </summary>
    /// <param name="Count">The number of view models to pop.</param>
    internal record PopOptionsCount(int Count) : PopOptions;

    /// <summary>
    /// Used to find a specific view model and pop all view models on top of
    /// that view model from the stack.
    /// </summary>
    /// <param name="Predicate">
    /// The function used to identify the desired view model.
    /// </param>
    /// <param name="FromTop">
    /// When true, iterate from the top of the stack to the bottom when 
    /// searching for the view model.
    /// When false, iterate from the bottom of the stack to the top when 
    /// searching for the view mdoel.
    /// </param>
    /// <param name="Inclusive">
    /// If true the pop count should remove the identified view from the stack 
    /// and everything above it.
    /// If false the pop count should leave the identified view on the stack but
    /// remove everything above it.
    /// </param>
    internal record PopOptionsQuery(
        Func<IViewModel, bool> Predicate,
        bool FromTop,
        bool Inclusive
    ) : PopOptions;

    #endregion

    #region Constructors

    /// <summary>
    /// The private constructor prevents classes outside of PopOptions from 
    /// inheriting PopOptions.
    /// </summary>
    private PopOptions()
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Remove all view models from the stack.
    /// </summary>
    /// <returns>PopOptions for removing view models from the stack.</returns>
    public static PopOptions PopAll() => new PopOptionsRemoveAll();

    /// <summary>
    /// Pop a specific number of view models from the stack.
    /// </summary>
    /// <param name="count">
    /// The number of view models to pop from the stack.
    /// If the count is larger than the current count of the stack all view 
    /// models will be removed from the stack.
    /// </param>
    /// <returns>PopOptions for removing view models from the stack.</returns>
    public static PopOptions PopCount(int count) => new PopOptionsCount(count);

    /// <summary>
    /// Remove the top most view models until the first view model with the
    /// given type is found.  If a matching view model isn't found the stack
    /// will remain unchanged.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of view model to look for.
    /// </typeparam>
    /// <param name="fromTop">
    /// When true, iterate from the top of the stack to the bottom when 
    /// searching for a view.
    /// When false, iterate from the bottom of the stack to the top when 
    /// searching for a view.
    /// </param>
    /// <param name="inclusive">
    /// If true the pop count should remove the identified view from the stack 
    /// and everything above it.
    /// If false the pop count should leave the identified view on the stack but
    /// remove everything above it.
    /// </param>
    /// <returns>PopOptions for removing view models from the stack.</returns>
    public static PopOptions PopTo<TViewModel>(
        bool fromTop = true,
        bool inclusive = false
    ) => new PopOptionsQuery(
        Predicate: viewModel => viewModel.GetType() == typeof(TViewModel),
        FromTop: fromTop,
        Inclusive: inclusive
    );

    /// <summary>
    /// Pop to the first view model that satisfies the match condition.
    /// If a matching view model isn't found the stack will remain unchanged.
    /// </summary>
    /// <param name="predicate">
    /// The function used to identify the desired view model.
    /// </param>
    /// <param name="fromTop">
    /// When true, iterate from the top of the stack to the bottom when 
    /// searching for a view.
    /// When false, iterate from the bottom of the stack to the top when 
    /// searching for a view.
    /// </param>
    /// <param name="inclusive">
    /// If true the pop count should remove the identified view from the stack 
    /// and everything above it.
    /// If false the pop count should leave the identified view on the stack but
    /// remove everything above it.
    /// </param>
    /// <returns>PopOptions for removing view models from the stack.</returns>
    public static PopOptions PopTo(
        Func<IViewModel, bool> predicate,
        bool fromTop = true,
        bool inclusive = false
    ) => new PopOptionsQuery(
        Predicate: predicate,
        FromTop: fromTop,
        Inclusive: inclusive
    );

    #endregion
}
