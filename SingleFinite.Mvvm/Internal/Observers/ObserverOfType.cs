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

namespace SingleFinite.Mvvm.Internal.Observers;

/// <summary>
/// Observer that casts arguments passed with an observed event into another
/// specified type before passing them on to observers down the chain.  If the
/// arguments can't be cast to the specified type the event is not passed down
/// the observer chain.
/// </summary>
/// <typeparam name="TArgsIn">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <typeparam name="TArgsOut">
/// The type to cast arguments passed with observed events into.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
internal class ObserverOfType<TArgsIn, TArgsOut>(
    IObserver<TArgsIn> parent
) :
    ObserverBase<TArgsIn>(parent),
    IObserver<TArgsOut>
{
    #region Methods

    /// <summary>
    /// Try to cast the passed in arguments to the specified type and then pass
    /// the newly typed arguments down the observer chain.
    /// </summary>
    /// <param name="args">The passed in arguments.</param>
    /// <returns>
    /// This method always returns false since arguments that are cast into a
    /// new type will be passed on through a new event that is specific to the
    /// cast type.
    /// </returns>
    protected override bool OnEvent(TArgsIn args)
    {
        if (args is TArgsOut outArgs)
            MappedNext?.Invoke(outArgs);

        return false;
    }

    #endregion

    #region Events

    /// <summary>
    /// This event is raised when arguments for an observed event are
    /// successfully cast into the specified type.
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
/// Observer that casts the sender and arguments passed with an observed event
/// into other specified types before passing them on to observers down the
/// chain.  If the sender or arguments can't be cast to the specified types the
/// event is not passed down the observer chain.
/// </summary>
/// <typeparam name="TSenderIn">
/// The type of sender passed with observed events.
/// </typeparam>
/// <typeparam name="TArgsIn">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <typeparam name="TSenderOut">
/// The type to cast the sender passed with observed events into.
/// </typeparam>
/// <typeparam name="TArgsOut">
/// The type to cast arguments passed with observed events into.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
internal class ObserverOfType<TSenderIn, TArgsIn, TSenderOut, TArgsOut>(
    IObserver<TSenderIn, TArgsIn> parent
) :
    ObserverBase<TSenderIn, TArgsIn>(parent),
    IObserver<TSenderOut, TArgsOut>
{
    #region Methods

    /// <summary>
    /// Try to cast the passed in sender and arguments to the specified types
    /// and then pass the newly typed arguments down the observer chain.
    /// </summary>
    /// <param name="sender">The passed in sender.</param>
    /// <param name="args">The passed in arguments.</param>
    /// <returns>
    /// This method always returns false since the sender and arguments that are
    /// cast into new types will be passed on through a new event that is
    /// specific to the cast types.
    /// </returns>
    protected override bool OnEvent(TSenderIn sender, TArgsIn args)
    {
        if (sender is TSenderOut outSender &&  args is TArgsOut outArgs)
            MappedNext?.Invoke(outSender, outArgs);

        return false;
    }

    #endregion

    #region Events

    /// <summary>
    /// This event is raised when arguments for an observed event are
    /// successfully cast into the specified type.
    /// </summary>
    event Action<TSenderOut, TArgsOut> IObserver<TSenderOut, TArgsOut>.Next
    {
        add => MappedNext += value;
        remove => MappedNext -= value;
    }
    private event Action<TSenderOut, TArgsOut>? MappedNext;

    #endregion
}
