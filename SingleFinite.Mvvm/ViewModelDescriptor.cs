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
    /// Optional context to provide to the view model that will be built for the
    /// view.
    /// </summary>
    object? ViewModelContext { get; }
}

/// <summary>
/// Implementation of <see cref="IViewModelDescriptor"/>.
/// </summary>
/// <param name="ViewModelType">
/// The type of view model to build a view for.
/// </param>
/// <param name="ViewModelContext">
/// Optional context to provide to the view model that will be built for the
/// view.
/// </param>
public record ViewModelDescriptor(
    Type ViewModelType,
    object? ViewModelContext = default
) : IViewModelDescriptor;

/// <summary>
/// Implementation of <see cref="IViewModelDescriptor"/> that uses a type 
/// parameter to specify the view model type.
/// </summary>
/// <typeparam name="TViewModel">
/// The type that will be returned with the <see cref="ViewModelType"/> 
/// property.
/// </typeparam>
public record ViewModelDescriptor<TViewModel>() : IViewModelDescriptor
    where TViewModel : IViewModel
{
    /// <summary>
    /// The type specified with the TViewModel type parameter.
    /// </summary>
    public Type ViewModelType { get; } = typeof(TViewModel);

    /// <summary>
    /// This will always be null.  If you need to specify a view model 
    /// descriptor that takes context use the 
    /// <see cref="ViewModelDescriptor{TViewModel, TViewModelContext}"/> record 
    /// instead.
    /// </summary>
    public virtual object? ViewModelContext { get; } = default;
}

/// <summary>
/// Implementation of <see cref="IViewModelDescriptor"/> that uses type 
/// parameters to specify types.
/// </summary>
/// <typeparam name="TViewModel">
/// The type that will be returned with the ViewModelType property.
/// </typeparam>
/// <typeparam name="TViewModelContext">
/// The type of context that will be provided to the view model that is built 
/// for the view.
/// </typeparam>
/// <param name="Context">
/// The context to provide to the view model that is built for the view.
/// </param>
public record ViewModelDescriptor<TViewModel, TViewModelContext>(
    TViewModelContext Context
) : ViewModelDescriptor<TViewModel>
    where TViewModel : IViewModel<TViewModelContext>
{
    /// <summary>
    /// This will return the passed in <see cref="Context"/> parameter.
    /// </summary>
    public override object? ViewModelContext { get; } = Context;
}
