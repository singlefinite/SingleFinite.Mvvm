// MIT License
// Copyright (c) 2025 Single Finite
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

namespace SingleFinite.Mvvm.Internal;

/// <summary>
/// Helper extensions for working with IViewModel objects.
/// </summary>
internal static class IViewModelExtensions
{
    /// <summary>
    /// Invoke the ILifecycle.Initialize method.
    /// </summary>
    /// <param name="viewModel">The view model to invoke the method on.</param>
    public static void Initialize(this IViewModel viewModel) =>
        (viewModel as ILifecycleAware)?.Initialize();

    /// <summary>
    /// Invoke the ILifecycle.Activate method.
    /// </summary>
    /// <param name="viewModel">The view model to invoke the method on.</param>
    public static void Activate(this IViewModel viewModel) =>
        (viewModel as ILifecycleAware)?.Activate();

    /// <summary>
    /// Invoke the ILifecycle.Deactivate method.
    /// </summary>
    /// <param name="viewModel">The view model to invoke the method on.</param>
    public static void Deactivate(this IViewModel viewModel) =>
        (viewModel as ILifecycleAware)?.Deactivate();

    /// <summary>
    /// Invoke the ILifecycle.Dispose method.
    /// </summary>
    /// <param name="viewModel">The view model to invoke the method on.</param>
    public static void Dispose(this IViewModel viewModel) =>
        (viewModel as ILifecycleAware)?.Dispose();
}
