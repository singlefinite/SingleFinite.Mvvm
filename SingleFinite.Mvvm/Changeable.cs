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
public abstract partial class Changeable :
    INotifyPropertyChanged,
    INotifyPropertyChanging,
    IDerivableProperties
{
    #region Fields

    /// <summary>
    /// Used to prevent PropertyChanged events from being raised until this 
    /// object has finished being changed.
    /// </summary>
    private readonly Transaction _transaction = new();

    /// <summary>
    /// Buffer that holds actions for raising PropertyChanged events when an 
    /// open transaction is closed.
    /// </summary>
    private readonly ActionBuffer<string> _propertyChangedBuffer = new();

    /// <summary>
    /// Flag that gets set to true while the Change method is executing.
    /// </summary>
    private bool _isChanging = false;

    /// <summary>
    /// Holds derived properties.
    /// </summary>
    private readonly DerivedPropertyCollection _derivedProperties = new();

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public Changeable()
    {
        _transaction.Closed
            .Observe()
            .OnEach(OnTransactionClosed);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Call the OnChanged method with the PropertyChanged events disabled.
    /// If this method is called while a previous call is still in progress, the
    /// new call will be ignored.
    /// </summary>
    protected void Change()
    {
        if (_isChanging)
            return;

        try
        {
            _isChanging = true;
            OnChanged();
        }
        finally
        {
            _isChanging = false;
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
    protected virtual void OnChanged()
    {
    }

    /// <summary>
    /// Raise any pending PropertyChanged and DerivedPropertyChanged events when
    /// a transaction has been closed.
    /// </summary>
    private void OnTransactionClosed()
    {
        var propertyNames = _propertyChangedBuffer.Keys;
        _propertyChangedBuffer.Flush();

        RaiseDerivedPropertyChanged(propertyNames);
    }

    /// <summary>
    /// Raise the PropertyChanging event.
    /// </summary>
    /// <param name="propertyName">
    /// The property name to provide with the PropertyChanging event args.
    /// </param>
    private void RaisePropertyChanging(string propertyName)
    {
        var args = new PropertyChangingEventArgs(propertyName);

        _propertyChanging?.Invoke(
            sender: this,
            e: args
        );
    }

    /// <summary>
    /// Raise the PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">
    /// The property name to provide with the PropertyChanged event args.
    /// </param>
    private void RaisePropertyChanged(string propertyName)
    {
        var args = new PropertyChangedEventArgs(propertyName);

        _propertyChanged?.Invoke(
            sender: this,
            e: new PropertyChangedEventArgs(propertyName)
        );
    }

    /// <summary>
    /// Raise the DerivedPropertyChanged event.
    /// </summary>
    /// <param name="propertyNames">
    /// The property names of all the source properties whose derived properties
    /// will have the DerivedPropertyChanged event raised.
    /// </param>
    private void RaiseDerivedPropertyChanged(IEnumerable<string> propertyNames)
    {
        _derivedProperties.RaiseDerivedPropertyChangedEvents(
            sourcePropertyNames: propertyNames,
            raiseEvent: (sender, args) => _derivedPropertyChanged?.Invoke(sender, args)
        );
    }

    /// <summary>
    /// Update the given field with the new value and raise the
    /// PropertyChanging and PropertyChanged events.  If the current value is 
    /// equal to the new value the currentValue will not be updated and the 
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
    /// that if this object is in the process of being changed this action will
    /// only be called the first time.  If this method is called again with the
    /// same property name while the object is still in the process of being
    /// changed is in progress it will not be invoked again.
    /// </param>
    /// <param name="onPropertyChanged">
    /// If the new value changes the property, invoke this action after changing
    /// the value and raising the PropertyChanged event.  Note that if this
    /// object is in the process of being changed this action will not be
    /// invoked until the the object has finished being changed.
    /// </param>
    /// <param name="name">
    /// The name provided with the PropertyChanging and PropertyChanged event 
    /// args.  Leave unset to use the name of the calling member.
    /// </param>
    protected void ChangeProperty<TValue>(
        ref TValue field,
        TValue value,
        Action? onPropertyChanging = default,
        Action? onPropertyChanged = default,
        [CallerMemberName] string? name = default
    )
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(name);

        if (object.Equals(field, value))
            return;

        using var token = _transaction.IsOpen ? null : _transaction.Start();

        void OnNotify()
        {
            RaisePropertyChanged(name);
            onPropertyChanged?.Invoke();
        }

        if (_propertyChangedBuffer.AddOrReplace(name, OnNotify))
        {
            RaisePropertyChanging(name);
            onPropertyChanging?.Invoke();
        }

        field = value;

        if (token is not null)
            Change();
    }

    /// <inheritdoc/>
    public void AddDerivedProperty(
        object derivedPropertyOwner,
        string derivedPropertyName,
        params IEnumerable<string> sourcePropertyNames
    )
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(derivedPropertyName);

        foreach (var sourcePropertyName in sourcePropertyNames)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(sourcePropertyName);

            _derivedProperties.Add(
                sourcePropertyName,
                derivedPropertyOwner,
                derivedPropertyName
            );
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// Raised when a property value is about to be changed.
    /// </summary>
    event PropertyChangingEventHandler? INotifyPropertyChanging.PropertyChanging
    {
        add { _propertyChanging += value; }
        remove { _propertyChanging -= value; }
    }
    private PropertyChangingEventHandler? _propertyChanging;

    /// <summary>
    /// Raised when a property value has changed.
    /// </summary>
    event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
    {
        add { _propertyChanged += value; }
        remove { _propertyChanged -= value; }
    }
    private PropertyChangedEventHandler? _propertyChanged;

    /// <inheritdoc/>
    event PropertyChangedEventHandler? IDerivableProperties.DerivedPropertyChanged
    {
        add { _derivedPropertyChanged += value; }
        remove { _derivedPropertyChanged -= value; }
    }
    private PropertyChangedEventHandler? _derivedPropertyChanged;

    #endregion
}
