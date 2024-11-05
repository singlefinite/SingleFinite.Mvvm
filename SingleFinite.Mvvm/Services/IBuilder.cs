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

using System.Diagnostics.CodeAnalysis;

namespace SingleFinite.Mvvm.Services;

/// <summary>
/// This service is used to build objects by providing constructor arguments from the <see cref="IServiceProvider"/> in the
/// same scope this service belongs to.
/// </summary>
public interface IBuilder
{
    /// <summary>
    /// Instantiate a type with constructor arguments provided directly and/or from the <see cref="IServiceProvider"/> in the
    /// same scope this service belongs to.
    /// </summary>
    /// <typeparam name="TType">The type of object to build.</typeparam>
    /// <param name="parameters">Constructor arguments not provided by the <see cref="IServiceProvider"/>.</param>
    /// <returns>The newly built object.</returns>
    TType Build<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TType>(params object[] parameters);

    /// <summary>
    /// Instantiate a type with constructor arguments provided directly and/or from the <see cref="IServiceProvider"/> in the
    /// same scope this service belongs to.
    /// </summary>
    /// <param name="instanceType">The type of object to build.</param>
    /// <param name="parameters">Constructor arguments not provided by the <see cref="IServiceProvider"/>.</param>
    /// <returns>The newly built object.</returns>
    object Build([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type instanceType, params object[] parameters);
}
