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
/// The source for an <see cref="EventToken"/>.  Instances of this class can be 
/// kept private within its owner while the EventToken is shared publicly 
/// outside of the owner.  Since events are only raised through this class it 
/// prevents events from being raised outside of the owning class.
/// </summary>
public sealed class EventTokenSource
{
    #region Properties

    /// <summary>
    /// The token provided by this class.
    /// </summary>
    public EventToken Token { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public EventTokenSource()
    {
        Token = new(this);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Raise the event for the Token.
    /// </summary>
    public void RaiseEvent() => EventRaised?.Invoke();

    #endregion

    #region Events

    /// <summary>
    /// Event that is raised when the RaiseEvent method is invoked.
    /// </summary>
    internal event Action? EventRaised;

    #endregion
}

/// <summary>
/// The source for an <see cref="EventToken"/>.  Instances of this class can be 
/// kept private within its owner while the EventToken is shared publicly 
/// outside of the owner.  Since events are only raised through this class it 
/// prevents events from being raised outside of the owning class.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with the event.
/// </typeparam>
public sealed class EventTokenSource<TArgs>
{
    #region Properties

    /// <summary>
    /// The token provided by this class.
    /// </summary>
    public EventToken<TArgs> Token { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public EventTokenSource()
    {
        Token = new(this);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Raise the event for the Token.
    /// </summary>
    /// <param name="args">The arguments to pass with the event.</param>
    public void RaiseEvent(TArgs args) => EventRaised?.Invoke(args);

    #endregion

    #region Events

    /// <summary>
    /// Event that is raised when the RaiseEvent method is invoked.
    /// </summary>
    internal event Action<TArgs>? EventRaised;

    #endregion
}

/// <summary>
/// The source for an <see cref="EventToken"/>.  Instances of this class can be 
/// kept private within its owner while the EventToken is shared publicly 
/// outside of the owner.  Since events are only raised through this class it 
/// prevents events from being raised outside of the owning class.
/// </summary>
/// <typeparam name="TSender">
/// The type of sender that raises the event.
/// </typeparam>
/// <typeparam name="TArgs">
/// The type of arguments passed with the event.
/// </typeparam>
public sealed class EventTokenSource<TSender, TArgs>
{
    #region Properties

    /// <summary>
    /// The token provided by this class.
    /// </summary>
    public EventToken<TSender, TArgs> Token { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public EventTokenSource()
    {
        Token = new(this);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Raise the event for the Token.
    /// </summary>
    /// <param name="sender">The object that is raising the event.</param>
    /// <param name="args">The arguments to pass with the event.</param>
    public void RaiseEvent(TSender sender, TArgs args) =>
        EventRaised?.Invoke(sender, args);

    #endregion

    #region Events

    /// <summary>
    /// Event that is raised when the RaiseEvent method is invoked.
    /// </summary>
    internal event Action<TSender, TArgs>? EventRaised;

    #endregion
}
