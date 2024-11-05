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

namespace SingleFinite.Mvvm.Services;

/// <summary>
/// Options for popping views off of a <see cref="IPresenterStack"/>.
/// </summary>
public abstract record PopOptions
{
    #region Types

    /// <summary>
    /// Used to pop all views off of the stack.
    /// </summary>
    internal record PopOptionsRemoveAll() : PopOptions;

    /// <summary>
    /// Used to pop a specific number of views off of the stack.
    /// </summary>
    /// <param name="Count">The number of views to pop.</param>
    internal record PopOptionsCount(int Count) : PopOptions;

    /// <summary>
    /// Used to find a specific view and pop all views on top of that view from the stack.
    /// </summary>
    /// <param name="Predicate">
    /// The function used to identify the desired view.
    /// </param>
    /// <param name="FromTop">
    /// When true, iterate from the top of the stack to the bottom when searching for the view.
    /// When false, iterate from the bottom of the stack to the top when searching for the view.
    /// </param>
    /// <param name="Inclusive">
    /// If true all views above the matching view and the matching view itself will
    /// be removed from the stack.  If false only the views above the matching view
    /// will be removed and the matching view will remain on the stack.
    /// </param>
    internal record PopOptionsQuery(
        Func<IView, bool> Predicate,
        bool FromTop,
        bool Inclusive
    ) : PopOptions;

    #endregion

    #region Constructors

    /// <summary>
    /// The private constructor prevents classes outside of PopOptions from inheriting PopOptions.
    /// </summary>
    private PopOptions()
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Remove all views from the stack.
    /// </summary>
    /// <returns>The PopOptions for removing views from the stack.</returns>
    public static PopOptions PopAll() => new PopOptionsRemoveAll();

    /// <summary>
    /// Pop a specific number of views from the stack.
    /// </summary>
    /// <param name="count">
    /// The number of views to pop from the stack.
    /// If the count is larger than the current count of the stack all views will be removed from the stack.
    /// </param>
    /// <returns>The PopOptions for removing views from the stack.</returns>
    public static PopOptions PopCount(int count) => new PopOptionsCount(count);

    /// <summary>
    /// Remove the top most views until the first view with the given view model type is found.
    /// If a matching view model isn't found the stack will remain unchanged.
    /// </summary>
    /// <typeparam name="TViewModel">The type of view model to look for in the views.</typeparam>
    /// <param name="fromTop">
    /// When true, iterate from the top of the stack to the bottom when searching for the view.
    /// When false, iterate from the bottom of the stack to the top when searching for the view.
    /// </param>
    /// <param name="inclusive">
    /// If true all views above the matching view and the matching view itself will
    /// be removed from the stack.  If false only the views above the matching view
    /// will be removed and the matching view will remain on the stack.
    /// </param>
    /// <returns>The PopOptions for removing views from the stack.</returns>
    public static PopOptions PopTo<TViewModel>(
        bool fromTop = true,
        bool inclusive = false
    ) => new PopOptionsQuery(
        Predicate: view => view.ViewModel.GetType() == typeof(TViewModel),
        FromTop: fromTop,
        Inclusive: inclusive
    );

    /// <summary>
    /// Pop to the first view that satisfies the match condition.
    /// If a matching view isn't found the stack will remain unchanged.
    /// </summary>
    /// <param name="predicate">The function used to identify the desired view.</param>
    /// <param name="fromTop">
    /// When true, iterate from the top of the stack to the bottom when searching for the view.
    /// When false, iterate from the bottom of the stack to the top when searching for the view.
    /// </param>
    /// <param name="inclusive">
    /// If true all views above the matching view and the matching view itself will
    /// be removed from the stack.  If false only the views above the matching view
    /// will be removed and the matching view will remain on the stack.
    /// </param>
    /// <returns>The PopOptions for removing views from the stack.</returns>
    public static PopOptions PopTo(
        Func<IView, bool> predicate,
        bool fromTop = true,
        bool inclusive = false
    ) => new PopOptionsQuery(
        Predicate: predicate,
        FromTop: fromTop,
        Inclusive: inclusive
    );

    #endregion
}
