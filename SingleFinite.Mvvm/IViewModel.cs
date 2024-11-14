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
/// Interface for a view model.
/// </summary>
public interface IViewModel : IDisposable
{
    /// <summary>
    /// Called immediately after this view model is created.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if this object has been disposed.
    /// </exception>
    void Initialize();

    /// <summary>
    /// Called after the view has been added to the display.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if this object has been disposed.
    /// </exception>
    void Activate();

    /// <summary>
    /// Called after the view has been removed from the display.
    /// <exception cref="ObjectDisposedException">
    /// Thrown if this object has been disposed.
    /// </exception>
    /// </summary>
    void Deactivate();

    /// <summary>
    /// Event raised when this view model has been initialized.
    /// </summary>
    EventToken Initialized { get; }

    /// <summary>
    /// Event raised when this view model has been activated.
    /// </summary>
    EventToken Activated { get; }

    /// <summary>
    /// Event raised when this view model has been deactivated.
    /// </summary>
    EventToken Deactivated { get; }

    /// <summary>
    /// Event raised when this view model has been disposed.
    /// </summary>
    EventToken Disposed { get; }
}

/// <summary>
/// Interface for a view model that accepts context.
/// </summary>
/// <typeparam name="TContext">
/// The type of context that must be provided to the view model.
/// </typeparam>
public interface IViewModel<TContext> : IViewModel
{
}
