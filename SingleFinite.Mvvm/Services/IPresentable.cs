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

using SingleFinite.Essentials;

namespace SingleFinite.Mvvm.Services;

/// <summary>
/// A service that presents a view.
/// </summary>
public interface IPresentable
{
    /// <summary>
    /// The current view to present.
    /// </summary>
    IView? Current { get; }

    /// <summary>
    /// Event that is raised when the current view has changed.
    /// </summary>
    Observable<CurrentChangedEventArgs> CurrentChanged { get; }

    /// <summary>
    /// Arguments for the CurrentChanged event.
    /// </summary>
    /// <param name="view">The current view.</param>
    /// <param name="isNew">
    /// If the current view became the current view because it was just added,
    /// this is true.  If the current view became the current view because
    /// other views were removed, this is false.
    /// </param>
    public class CurrentChangedEventArgs(
        IView? view,
        bool isNew
    ) : EventArgs
    {
        /// <summary>
        /// The current view.
        /// </summary>
        public IView? View => view;

        /// <summary>
        /// If the current view became the current view because it was just added,
        /// this is true.  If the current view became the current view because
        /// other views were removed, this is false.
        /// </summary>
        public bool IsNew => isNew;
    }
}
