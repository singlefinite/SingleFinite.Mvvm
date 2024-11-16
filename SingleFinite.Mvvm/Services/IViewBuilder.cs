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
/// This service is used to build <see cref="IView"/> objects.
/// </summary>
public interface IViewBuilder
{
    /// <summary>
    /// Build a new view using the provided view model descriptor.
    /// </summary>
    /// <param name="viewModelDescriptor">
    /// The view model descriptor to build a view for.
    /// </param>
    /// <returns>The newly built view.</returns>
    IView Build(IViewModelDescriptor viewModelDescriptor);

    /// <summary>
    /// Build a new view using the provided type parameter.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of view model to build for the view.
    /// </typeparam>
    /// <returns>The newly built view.</returns>
    IView<TViewModel> Build<TViewModel>()
        where TViewModel : IViewModel;

    /// <summary>
    /// Build a new view using the provided type parameters.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of view model to build for the view.
    /// </typeparam>
    /// <typeparam name="TViewModelContext">
    /// The type of context that will be provided to the view model that is 
    /// built for the view.
    /// </typeparam>
    /// <param name="context">
    /// The context to provide to the view model that is built for the view.
    /// </param>
    /// <returns>The newly built view.</returns>
    IView<TViewModel> Build<TViewModel, TViewModelContext>(TViewModelContext context)
        where TViewModel : IViewModel<TViewModelContext>;
}
