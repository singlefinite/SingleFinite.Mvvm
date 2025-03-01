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

namespace SingleFinite.Mvvm;

/// <summary>
/// Used to specify the view model type and context to build an 
/// <seealso cref="IView"/> with.
/// </summary>
public interface IViewModelDescriptor
{
    /// <summary>
    /// The type of view model to build a view for.
    /// </summary>
    Type ViewModelType { get; }

    /// <summary>
    /// Optional parameters to provide to the view model that will be built for
    /// the view.
    /// </summary>
    object[] ViewModelParameters { get; }
}

/// <summary>
/// Implementation of <see cref="IViewModelDescriptor"/>.
/// </summary>
/// <param name="ViewModelType">
/// The type of view model to build a view for.
/// </param>
/// <param name="ViewModelParameters">
/// Optional parameters to provide to the view model that will be built for the
/// view.
/// </param>
public record ViewModelDescriptor(
    Type ViewModelType,
    object[] ViewModelParameters
) : IViewModelDescriptor;

/// <summary>
/// Implementation of <see cref="IViewModelDescriptor"/> that uses a type 
/// parameter to specify the view model type.
/// </summary>
/// <typeparam name="TViewModel">
/// The type that will be returned with the <see cref="ViewModelType"/> 
/// property.
/// </typeparam>
/// <param name="Parameters">
/// Optional parameters to provide to the view model that will be built for the
/// view.
/// </param>
public record ViewModelDescriptor<TViewModel>(
    params object[] Parameters
) : IViewModelDescriptor
    where TViewModel : IViewModel
{
    /// <inheritdoc/>
    public Type ViewModelType { get; } = typeof(TViewModel);

    /// <inheritdoc/>
    public virtual object[] ViewModelParameters { get; } = Parameters;
}
