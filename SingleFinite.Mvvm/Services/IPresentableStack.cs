// MIT License
// Copyright (c) 2026 Single Finite
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
public interface IPresentableStack : IPresentable
{
    /// <summary>
    /// The current view models in the stack.
    /// The top of the stack is the view model at index 0.
    /// </summary>
    IViewModel[] ViewModels { get; }

    /// <summary>
    /// Create a view model and push it onto the top of the stack.
    /// </summary>
    /// <param name="viewModelDescriptor">
    /// Describes the view model to build.
    /// </param>
    /// <param name="popOptions">
    /// Options for popping view models off the stack before pushing the new
    /// view model on.
    /// </param>
    /// <returns>The newly created view model.</returns>
    IViewModel Push(
        IViewModelDescriptor viewModelDescriptor,
        PopOptions? popOptions = default
    );


    /// <summary>
    /// Create a view model and push it onto the top of the stack.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of view model to build.
    /// </typeparam>
    /// <param name="parameters">
    /// The parameters that will be provided to the view model.
    /// </param>
    /// <returns>The newly created view model.</returns>
    TViewModel Push<TViewModel>(
        params object[] parameters
    )
        where TViewModel : IViewModel;

    /// <summary>
    /// Create a view model and push it onto the top of the stack.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of view model to build.
    /// </typeparam>
    /// <param name="popOptions">
    /// Options for popping view models off the stack before pushing the new
    /// view model on.
    /// </param>
    /// <param name="parameters">
    /// The parameters that will be provided to the view model.
    /// </param>
    /// <returns>The newly created view model.</returns>
    TViewModel Push<TViewModel>(
        PopOptions? popOptions = default,
        params object[] parameters
    )
        where TViewModel : IViewModel;

    /// <summary>
    /// Create view models and push them onto the top of the stack.
    /// </summary>
    /// <param name="viewModelDescriptors">
    /// The description of view models to build.
    /// </param>
    /// <returns>The newly created view models.</returns>
    IViewModel[] PushAll(
        params IEnumerable<IViewModelDescriptor> viewModelDescriptors
    );

    /// <summary>
    /// Create view models and push them onto the top of the stack.
    /// </summary>
    /// <param name="popOptions">
    /// Options for popping view models off the stack before pushing the new view
    /// models on.
    /// </param>
    /// <param name="viewModelDescriptors">
    /// The description of view models to build.
    /// </param>
    /// <returns>The newly created view models.</returns>
    IViewModel[] PushAll(
        PopOptions? popOptions = default,
        params IEnumerable<IViewModelDescriptor> viewModelDescriptors
    );

    /// <summary>
    /// Create a view model and add it to the stack at the given index.
    /// </summary>
    /// <param name="index">The index to add the view model at.</param>
    /// <param name="viewModelDescriptor">
    /// Describes the view model to build.
    /// </param>
    /// <param name="popOptions">
    /// Options for popping view models off the stack before pushing the new
    /// view model on.
    /// </param>
    /// <returns>The newly created view model.</returns>
    IViewModel Add(
        int index,
        IViewModelDescriptor viewModelDescriptor,
        PopOptions? popOptions = default
    );

    /// <summary>
    /// Create a view model and add it to the stack at the given index.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of view model to build.
    /// </typeparam>
    /// <param name="index">The index to add the view model at.</param>
    /// <param name="parameters">
    /// The parameters that will be provided to the view model.
    /// </param>
    /// <returns>The newly created view model.</returns>
    TViewModel Add<TViewModel>(
        int index,
        params object[] parameters
    )
        where TViewModel : IViewModel;

    /// <summary>
    /// Create a view model and add it to the stack at the given index.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of view model to build.
    /// </typeparam>
    /// <param name="index">The index to add the view model at.</param>
    /// <param name="popOptions">
    /// Options for popping view models off the stack before pushing the new
    /// view model on.
    /// </param>
    /// <param name="parameters">
    /// The parameters that will be provided to the view model.
    /// </param>
    /// <returns>The newly created view model.</returns>
    TViewModel Add<TViewModel>(
        int index,
        PopOptions? popOptions = default,
        params object[] parameters
    )
        where TViewModel : IViewModel;

    /// <summary>
    /// Create view models and add them to the stack at the given index.
    /// </summary>
    /// <param name="index">The index to add the view model at.</param>
    /// <param name="viewModelDescriptors">
    /// The description of view models to build.
    /// </param>
    /// <returns>The newly created view models.</returns>
    IViewModel[] AddAll(
        int index,
        params IEnumerable<IViewModelDescriptor> viewModelDescriptors
    );

    /// <summary>
    /// Create view models and add them to the stack at the given index.
    /// </summary>
    /// <param name="index">The index to add the view model at.</param>
    /// <param name="popOptions">
    /// Options for popping view models off the stack before pushing the new view
    /// models on.
    /// </param>
    /// <param name="viewModelDescriptors">
    /// The description of view models to build.
    /// </param>
    /// <returns>The newly created view models.</returns>
    IViewModel[] AddAll(
        int index,
        PopOptions? popOptions = default,
        params IEnumerable<IViewModelDescriptor> viewModelDescriptors
    );

    /// <summary>
    /// Remove the top most view model from the stack.
    /// If the stack is empty this method will have no effect.
    /// </summary>
    /// <returns>true if a view model was removed from the stack.</returns>
    bool Pop();

    /// <summary>
    /// Remove the top most view models until the view model with the given type 
    /// is found.  If a matching view model isn't found the stack will remain
    /// unchanged.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of view model to look for in the stack.
    /// </typeparam>
    /// <param name="fromTop">
    /// When true, iterate from the top of the stack to the bottom when 
    /// searching for the view model.
    /// When false, iterate from the bottom of the stack to the top when 
    /// searching for the view mdoel.
    /// </param>
    /// <param name="inclusive">
    /// If true all view models above the matching view model and the matching
    /// view model itself will be removed from the stack.  If false only the
    /// view models above the matching view model will be removed and the
    /// matching view model will remain on the stack.
    /// </param>
    /// <returns>true if a matching view was found.</returns>
    bool PopTo<TViewModel>(
        bool fromTop = true,
        bool inclusive = false
    ) where TViewModel : IViewModel;

    /// <summary>
    /// Pop to the first view model that satisfies the match condition.
    /// If a matching view model isn't found the stack will remain unchanged.
    /// </summary>
    /// <param name="predicate">
    /// The function used to determine if a view model is a match.
    /// </param>
    /// <param name="fromTop">
    /// When true, iterate from the top of the stack to the bottom when 
    /// searching for the view model.
    /// When false, iterate from the bottom of the stack to the top when 
    /// searching for the view mdoel.
    /// </param>
    /// <param name="inclusive">
    /// Indicates if the matching view model should also be popped.
    /// </param>
    /// <returns>
    /// true if a match was found and the stack was changed, otherwise false.
    /// </returns>
    bool PopTo(
        Func<IViewModel, bool> predicate,
        bool fromTop = true,
        bool inclusive = false
    );

    /// <summary>
    /// Remove all view models from the stack.
    /// </summary>
    void Clear();

    /// <summary>
    /// Remove the given view models from the stack.
    /// </summary>
    /// <param name="viewModels">
    /// The view models to remove from the stack.
    /// </param>
    void Close(params IEnumerable<IViewModel> viewModels);
}
