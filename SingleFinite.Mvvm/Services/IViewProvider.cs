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
/// Interface that provides <see cref="IView"/> instances for
/// <see cref="IViewModel"/> types.
/// </summary>
public interface IViewProvider
{
    /// <summary>
    /// Provide a view using the given view model descriptor.
    /// </summary>
    /// <param name="viewModelDescriptor">
    /// The view model descriptor to provide a view for.
    /// </param>
    /// <returns>The view.</returns>
    IView ProvideFromDescriptor(
        IViewModelDescriptor viewModelDescriptor
    );

    /// <summary>
    /// Provide a view using the given type parameter.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of view model to provide a view for.
    /// </typeparam>
    /// <param name="parameters">
    /// The parameters to provide to the view model that is built.
    /// </param>
    /// <returns>The view.</returns>
    IView<TViewModel> Provide<TViewModel>(params object[] parameters)
        where TViewModel : IViewModel;
}
