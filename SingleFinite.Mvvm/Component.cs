﻿// MIT License
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

using System.ComponentModel;
using System.Runtime.CompilerServices;
using SingleFinite.Mvvm.Internal;

namespace SingleFinite.Mvvm;

/// <summary>
/// This class implements the <see cref="INotifyPropertyChanged"/> and 
/// <see cref="INotifyPropertyChanging"/> interfaces and provides methods that 
/// inheriting classes can use to raise PropertyChanged and PropertyChanging 
/// events.
/// </summary>
public abstract class Component :
    INotifyPropertyChanged,
    INotifyPropertyChanging
{
    #region Fields

    /// <summary>
    /// Used to prevent PropertyChanged events from being raised until this 
    /// object has finished being updated.
    /// </summary>
    private readonly Transaction _transaction = new();

    /// <summary>
    /// Buffer that holds actions for raising PropertyChanged events when an 
    /// open transaction is closed.
    /// </summary>
    private readonly ActionBuffer<string> _transactionBuffer = new();

    /// <summary>
    /// Flag that gets set to true while the Update method is executing.
    /// </summary>
    private bool _isUpdating = false;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public Component()
    {
        _transaction.Closed.Observe(OnTransactionClosed);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Call the OnUpdated method with the PropertyChanged events disabled.
    /// If this method is called while a previous call is still in progress, the
    /// new call will be ignored.
    /// </summary>
    protected void Update()
    {
        if (_isUpdating)
            return;

        try
        {
            _isUpdating = true;
            OnUpdated();
        }
        finally
        {
            _isUpdating = false;
        }
    }

    /// <summary>
    /// This method is called whenever one or more PropertyChanged events are 
    /// raised by this object.  To prevent infinite recursion, the 
    /// PropertyChanged events that are raised as a result of changes to 
    /// properties made by this method will be suppressed until this method has 
    /// exited.  When the suppressed PropertyChanged events are raised they will
    /// not trigger this method being called again.
    /// </summary>
    protected virtual void OnUpdated()
    {
    }

    /// <summary>
    /// Raise any pending PropertyChanged events when a transaction has been 
    /// closed.
    /// </summary>
    private void OnTransactionClosed()
    {
        _transactionBuffer.Flush();
    }

    /// <summary>
    /// Raise the PropertyChanging event.
    /// </summary>
    /// <param name="name">
    /// The name to provide with the PropertyChanging event args.
    /// </param>
    private void RaisePropertyChanging(string name)
    {
        PropertyChanging?.Invoke(
            sender: this,
            e: new PropertyChangingEventArgs(name)
        );
    }

    /// <summary>
    /// Raise the PropertyChanged event.
    /// </summary>
    /// <param name="name">
    /// The name to provide with the PropertyChanged event args.
    /// </param>
    private void RaisePropertyChanged(string name)
    {
        PropertyChanged?.Invoke(
            sender: this,
            e: new PropertyChangedEventArgs(name)
        );
    }

    /// <summary>
    /// Update the given currentValue with the newValue and raise the
    /// PropertyChanging and PropertyChanged events.  If the currentValue is 
    /// equal to the newValue the currentValue will not be updated and the 
    /// PropertyChanging and PropertyChanged events will not be raised.
    /// </summary>
    /// <typeparam name="TValue">The type of value.</typeparam>
    /// <param name="field">
    /// The reference to the property field which may be changed.
    /// </param>
    /// <param name="value">The new value to set the property field to.</param>
    /// <param name="onPropertyChanging">
    /// If the new value will change the property, invoke this action before 
    /// changing the value and after raising the PropertyChanging event.  Note 
    /// that if a transaction is open this action will only be called the first 
    /// time.  If this method is called again with the same property name while 
    /// the same transaction is open it will not be called again.
    /// </param>
    /// <param name="onPropertyChanged">
    /// If the new value changes the property, invoke this action after changing
    /// the value and raising the PropertyChanged event.  Note that if a 
    /// transaction is open this action will not be invoked until the 
    /// transaction has been closed.
    /// </param>
    /// <param name="name">
    /// The name provided with the PropertyChanging and PropertyChanged event 
    /// args.  Leave unset to use the name of the calling member.
    /// </param>
    protected void ChangeProperty<TValue>(
        ref TValue field,
        TValue value,
        Action? onPropertyChanging = null,
        Action? onPropertyChanged = null,
        [CallerMemberName] string? name = default
    )
    {
        ArgumentNullException.ThrowIfNull(name);

        if (object.Equals(field, value))
            return;

        if (_transaction.IsOpen)
        {
            void OnNotify()
            {
                RaisePropertyChanged(name);
                onPropertyChanged?.Invoke();
            };

            if (_transactionBuffer.AddOrReplace(name, OnNotify))
            {
                RaisePropertyChanging(name);
                onPropertyChanging?.Invoke();
            }

            field = value;
        }
        else
        {
            using var token = _transaction.Start();

            RaisePropertyChanging(name);
            onPropertyChanging?.Invoke();

            field = value;

            Update();

            RaisePropertyChanged(name);
            onPropertyChanged?.Invoke();
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// Raised when a property value is about to be changed.
    /// </summary>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <summary>
    /// Raised when a property value has changed.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion
}