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

using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm;

/// <summary>
/// The result from assembling a view through an <see cref="IViewBuilder"/>.
/// </summary>
public interface IViewAssembleResult
{
    /// <summary>
    /// The view that was built.
    /// </summary>
    IView View { get; }

    /// <summary>
    /// This method calls the OnCreated method on the view model and loads
    /// plugins.
    /// </summary>
    void Start();
}

/// <summary>
/// The result from assembling a view through an <see cref="IViewBuilder"/>.
/// </summary>
/// <typeparam name="TViewModel">
/// The type of view model the view was assembled for.
/// </typeparam>
public interface IViewAssembleResult<TViewModel> : IViewAssembleResult
    where TViewModel : IViewModel
{
    /// <summary>
    /// The view that was built.
    /// </summary>
    new IView<TViewModel> View { get; }
}
