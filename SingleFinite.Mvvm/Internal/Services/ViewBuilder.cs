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
using SingleFinite.Mvvm.Services;

namespace SingleFinite.Mvvm.Internal.Services;

/// <summary>
/// Implementation of <see cref="IViewBuilder"/> that uses the service provider 
/// to build views.
/// </summary>
/// <param name="serviceProvider">Used to build views.</param>
internal sealed class ViewBuilder(
    IServiceProvider serviceProvider
) : IViewBuilder
{
    #region Methods

    /// <inheritdoc/>
    public IView BuildFromDescriptor(IViewModelDescriptor viewModelDescriptor) =>
        AssembleFromDescriptor(viewModelDescriptor).Also(it => it.Start()).View;

    /// <inheritdoc/>
    public IView<TViewModel> Build<TViewModel>(params object[] parameters)
        where TViewModel : IViewModel =>
        Assemble<TViewModel>(parameters).Also(it => it.Start()).View;

    /// <inheritdoc/>
    public IViewAssembleResult AssembleFromDescriptor(
        IViewModelDescriptor viewModelDescriptor
    ) => new ViewAssembleResult(
        serviceProvider: serviceProvider,
        viewModelDescriptor: viewModelDescriptor
    );

    /// <inheritdoc/>
    public IViewAssembleResult<TViewModel> Assemble<TViewModel>(
        params object[] parameters
    )
        where TViewModel : IViewModel => new ViewAssembleResult<TViewModel>(
            serviceProvider: serviceProvider,
            viewModelDescriptor: new ViewModelDescriptor<TViewModel>(parameters)
        );

    #endregion
}
