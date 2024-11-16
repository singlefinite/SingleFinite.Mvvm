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

namespace SingleFinite.Mvvm.Services;

/// <summary>
/// A presenter for <see cref="IView"/> objects in a stack.  The stack will 
/// create and invoke the lifecycle methods for the view models that are part of
/// the stack.
/// </summary>
public interface IPresenterStack : IPresenter
{
    /// <summary>
    /// The current views in the stack.
    /// The top of the stack is the view at index 0.
    /// </summary>
    IView[] Stack { get; }

    /// <summary>
    /// Create a view and push it onto the top of the stack.
    /// </summary>
    /// <param name="viewModelDescriptor">Describes the view to build.</param>
    /// <param name="popOptions">
    /// Options for popping views off the stack before pushing the new views on.
    /// </param>
    /// <returns>The newly created view.</returns>
    IView Push(
        IViewModelDescriptor viewModelDescriptor,
        PopOptions? popOptions = null
    );

    /// <summary>
    /// Create a view and push it onto the top of the stack.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of view model to create a view for.
    /// </typeparam>
    /// <param name="popOptions">
    /// Options for popping views off the stack before pushing the new views on.
    /// </param>
    /// <returns>The newly created view.</returns>
    IView<TViewModel> Push<TViewModel>(PopOptions? popOptions = null)
        where TViewModel : IViewModel;

    /// <summary>
    /// Create a view and push it onto the top of the stack.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of view model to create a view for.
    /// </typeparam>
    /// <typeparam name="TViewModelContext">
    /// The type of context to be provided to the view model.
    /// </typeparam>
    /// <param name="context">
    /// The context that will be provided to the view model.
    /// </param>
    /// <param name="popOptions">
    /// Options for popping views off the stack before pushing the new views on.
    /// </param>
    /// <returns>The newly created view.</returns>
    IView<TViewModel> Push<TViewModel, TViewModelContext>(
        TViewModelContext context,
        PopOptions? popOptions = null
    )
        where TViewModel : IViewModel<TViewModelContext>;

    /// <summary>
    /// Create views and push them onto the top of the stack.
    /// </summary>
    /// <param name="viewModelDescriptors">
    /// The description of views to push onto the top of the stack.
    /// </param>
    /// <param name="popOptions">
    /// Options for popping views off the stack before pushing the new views on.
    /// </param>
    /// <returns>The newly created views.</returns>
    IView[] PushAll(
        IEnumerable<IViewModelDescriptor> viewModelDescriptors,
        PopOptions? popOptions = null
    );

    /// <summary>
    /// Remove the top most view from the stack.
    /// If the stack is empty this method will have no effect.
    /// </summary>
    /// <returns>true if a view was removed from the stack.</returns>
    bool Pop();

    /// <summary>
    /// Remove the top most views until the view with the given view model type 
    /// is found.
    /// If a matching view model isn't found the stack will remain unchanged.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of view model to look for in the views.
    /// </typeparam>
    /// <param name="fromTop">
    /// When true, iterate from the top of the stack to the bottom when 
    /// searching for the view.
    /// When false, iterate from the bottom of the stack to the top when 
    /// searching for the view.
    /// </param>
    /// <param name="inclusive">
    /// If true all views above the matching view and the matching view itself 
    /// will be removed from the stack.  If false only the views above the 
    /// matching view will be removed and the matching view will remain on the 
    /// stack.
    /// </param>
    /// <returns>true if a matching view was found.</returns>
    bool PopTo<TViewModel>(
        bool fromTop = true,
        bool inclusive = false
    ) where TViewModel : IViewModel;

    /// <summary>
    /// Pop to the first view that satisfies the match condition.
    /// If a matching view isn't found the stack will remain unchanged.
    /// </summary>
    /// <param name="predicate">
    /// The function used to determine if a view is a match.
    /// </param>
    /// <param name="fromTop">
    /// When true, iterate from the top of the stack to the bottom when 
    /// searching for the view.
    /// When false, iterate from the bottom of the stack to the top when 
    /// searching for the view.
    /// </param>
    /// <param name="inclusive">
    /// Indicates if the matching view should also be popped.
    /// </param>
    /// <returns>
    /// true if a match was found and the stack was changed, otherwise false.
    /// </returns>
    bool PopTo(
        Func<IView, bool> predicate,
        bool fromTop = true,
        bool inclusive = false
    );

    /// <summary>
    /// Remove all views from the stack.
    /// </summary>
    void PopAll();
}
