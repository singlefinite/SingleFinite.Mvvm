﻿// MIT License
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

using System.ComponentModel;

namespace SingleFinite.Mvvm;

/// <summary>
/// A view is responsible for displaying information to the user and collecting 
/// user input.
/// </summary>
public interface IView : INotifyPropertyChanged
{
    /// <summary>
    /// The view model that provides information for the view and handles 
    /// processing of the user input.
    /// </summary>
    IViewModel ViewModel { get; }

    /// <summary>
    /// Default implementation for <see cref="INotifyPropertyChanged"/> that
    /// supports <see cref="IDerivableProperties"/> if the ViewModel implements
    /// the interface.
    /// </summary>
    event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
    {
        add
        {
            if (ViewModel is IDerivableProperties derivableProperties)
                derivableProperties.DerivedPropertyChanged += value;
        }
        remove
        {
            if (ViewModel is IDerivableProperties derivableProperties)
                derivableProperties.DerivedPropertyChanged -= value;
        }
    }
}

/// <summary>
/// A view is responsible for displaying information to the user and collecting 
/// user input.
/// </summary>
/// <typeparam name="TViewModel">
/// The type of view model for the view.
/// </typeparam>
public interface IView<TViewModel> : IView where TViewModel : IViewModel
{
    /// <summary>
    /// The view model that provides information for the view and handles 
    /// processing of the user input.
    /// </summary>
    IViewModel IView.ViewModel => ViewModel;

    /// <summary>
    /// The view model that provides information for the view and handles 
    /// processing of the user input.
    /// </summary>
    new TViewModel ViewModel { get; }
}
