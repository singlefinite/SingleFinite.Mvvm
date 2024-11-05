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
/// A presenter for a single <see cref="IView"/> object.  The frame will create and invoke the lifecycle methods
/// when a view model is added and when it is removed from the frame.
/// </summary>
public interface IPresenterFrame : IPresenter
{
    /// <summary>
    /// Create a view and put it on the frame.  If there is already a view on the frame it will be removed before
    /// the new view is added.
    /// </summary>
    /// <param name="viewModelDescriptor">Describes the view to build.</param>
    /// <returns>The newly created view.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if this presenter has been disposed.</exception>
    IView Set(IViewModelDescriptor viewModelDescriptor);

    /// <summary>
    /// Create a view and put it on the frame.  If there is already a view on the frame it will be removed before
    /// the new view is added.
    /// </summary>
    /// <typeparam name="TViewModel">The type of view model to create a view for.</typeparam>
    /// <returns>The newly created view.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if this presenter has been disposed.</exception>
    IView Set<TViewModel>()
        where TViewModel : IViewModel;

    /// <summary>
    /// Create a view and put it on the frame.  If there is already a view on the frame it will be removed before
    /// the new view is added.
    /// </summary>
    /// <typeparam name="TViewModel">The type of view model to create a view for.</typeparam>
    /// <typeparam name="TViewModelContext">The type of context to be provided to the view model.</typeparam>
    /// <param name="context">The context that will be provided to the view model.</param>
    /// <returns>The newly created view.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if this presenter has been disposed.</exception>
    IView Set<TViewModel, TViewModelContext>(TViewModelContext context)
        where TViewModel : IViewModel<TViewModelContext>;

    /// <summary>
    /// Remove the current view if there is one.
    /// </summary>
    void Clear();
}
