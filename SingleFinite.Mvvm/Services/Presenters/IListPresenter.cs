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

using SingleFinite.Essentials;

namespace SingleFinite.Mvvm.Services.Presenters;

/// <summary>
/// A presenter for a list of <see cref="IView"/> objects.  The presenter is
/// responsible for creating and invoking the lifecycle methods for the view
/// models it manages.
/// </summary>
public interface IListPresenter : IPresenter
{
    /// <summary>
    /// The index of the view model that is the current view model.  If there is
    /// no current view model this will be -1.
    /// </summary>
    int CurrentIndex { get; }

    /// <summary>
    /// The current view models in the list.
    /// </summary>
    IViewModel[] ViewModels { get; }

    /// <summary>
    /// Make the view model at the given index the current view model.
    /// </summary>
    /// <param name="index">
    /// The index of the view model to make current.  If this is -1 Current will
    /// be set to null.
    /// </param>
    void SetCurrentIndex(int index);

    /// <summary>
    /// Make the given view model the current view model.
    /// </summary>
    /// <param name="viewModel">
    /// The view model to make current.  It must be found in the ViewModels
    /// collection for this object.
    /// </param>
    void SetCurrent(IViewModel? viewModel);

    /// <summary>
    /// Make the first view model in the list of the given type the current view
    /// model.  If there is no view model of the given type in the list then
    /// the Current will be set to null..
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of view model to make the current view model.
    /// </typeparam>
    void SetCurrent<TViewModel>()
        where TViewModel : IViewModel;

    /// <summary>
    /// Create a view model and add it to the list at the given index.
    /// </summary>
    /// <param name="index">The index to add the view model at.</param>
    /// <param name="viewModelDescriptor">
    /// Describes the view model to build.
    /// </param>
    /// <returns>The newly created view model.</returns>
    IViewModel Add(
        int index,
        IViewModelDescriptor viewModelDescriptor
    );

    /// <summary>
    /// Create a view model and add it to the list at the given index.
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
    /// Create a view model and add it to the end of the list.
    /// </summary>
    /// <param name="viewModelDescriptor">
    /// Describes the view model to build.
    /// </param>
    /// <returns>The newly created view model.</returns>
    IViewModel Add(
        IViewModelDescriptor viewModelDescriptor
    );

    /// <summary>
    /// Create a view model and add it to the end of the list.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of view model to build.
    /// </typeparam>
    /// <param name="parameters">
    /// The parameters that will be provided to the view model.
    /// </param>
    /// <returns>The newly created view model.</returns>
    TViewModel Add<TViewModel>(
        params object[] parameters
    )
        where TViewModel : IViewModel;

    /// <summary>
    /// Create view models and add them to the list at the given index.
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
    /// Create view models and add them to the end of the list.
    /// </summary>
    /// <param name="viewModelDescriptors">
    /// The description of view models to build.
    /// </param>
    /// <returns>The newly created view models.</returns>
    IViewModel[] AddAll(
        params IEnumerable<IViewModelDescriptor> viewModelDescriptors
    );

    /// <summary>
    /// Remove the view model at the given index.
    /// </summary>
    /// <param name="index">The index of the view model to remove.</param>
    void Remove(int index);

    /// <summary>
    /// Remove the given view models from the list.
    /// </summary>
    /// <param name="viewModels">
    /// The view models to remove from the list.
    /// </param>
    void Remove(params IEnumerable<IViewModel> viewModels);

    /// <summary>
    /// Remove all view models from the list.
    /// </summary>
    void Clear();

    /// <summary>
    /// When the CurrentIndex value changes this observable will emit the new
    /// value.
    /// </summary>
    IEventObservable<int> CurrentIndexChanged { get; }
}
