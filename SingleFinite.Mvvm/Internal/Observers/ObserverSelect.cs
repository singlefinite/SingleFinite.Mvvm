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

namespace SingleFinite.Mvvm.Internal.Observers;

/// <summary>
/// Observer that selects a value for an observed event to pass down the
/// observer chain.
/// </summary>
/// <typeparam name="TArgsOut">
/// The type of value that will be selected and passed down the observer chain.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
/// <param name="selector">The callback used to select the value.</param>
internal class ObserverSelect<TArgsOut>(
    IObserver parent,
    Func<TArgsOut> selector
) :
    ObserverBase(parent),
    IObserver<TArgsOut>
{
    #region Methods

    /// <summary>
    /// Select the value to pass down the observer chain.
    /// </summary>
    /// <returns>
    /// This method always returns false since the selected value will be passed
    /// on through a new event that is specific to the value type.
    /// </returns>
    protected override bool OnEvent()
    {
        var value = selector();
        MappedNext?.Invoke(value);
        return false;
    }

    #endregion

    #region Events

    /// <summary>
    /// This event is raised when a value is selected for an observed event to
    /// pass down the observer chain.
    /// </summary>
    event Action<TArgsOut> IObserver<TArgsOut>.Next
    {
        add => MappedNext += value;
        remove => MappedNext -= value;
    }
    private event Action<TArgsOut>? MappedNext;

    #endregion
}

/// <summary>
/// Observer that selects a value for an observed event to pass down the
/// observer chain.
/// </summary>
/// <typeparam name="TArgsIn">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <typeparam name="TArgsOut">
/// The type of value that will be selected and passed down the observer chain.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
/// <param name="callback">The callback used to select the value.</param>
internal class ObserverSelect<TArgsIn, TArgsOut>(
    IObserver<TArgsIn> parent,
    Func<TArgsIn, TArgsOut> callback
) :
    ObserverBase<TArgsIn>(parent),
    IObserver<TArgsOut>
{
    #region Methods

    /// <summary>
    /// Select the value to pass down the observer chain.
    /// </summary>
    /// <param name="args">
    /// The arguments passed with the observed event that will be passed to the
    /// callback used to select the value.
    /// </param>
    /// <returns>
    /// This method always returns false since the selected value will be passed
    /// on through a new event that is specific to the value type.
    /// </returns>
    protected override bool OnEvent(TArgsIn args)
    {
        var value = callback(args);
        MappedNext?.Invoke(value);
        return false;
    }

    #endregion

    #region Events

    /// <summary>
    /// This event is raised when a value is selected for an observed event to
    /// pass down the observer chain.
    /// </summary>
    event Action<TArgsOut> IObserver<TArgsOut>.Next
    {
        add => MappedNext += value;
        remove => MappedNext -= value;
    }
    private event Action<TArgsOut>? MappedNext;

    #endregion
}
