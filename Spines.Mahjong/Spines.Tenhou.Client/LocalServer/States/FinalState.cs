// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Spines.Tenhou.Client.LocalServer.States
{
  internal sealed class FinalState : IState
  {
    public bool IsFinal
    {
      get { return true; }
    }

    public event EventHandler<StateTimedOutEventArgs> TimedOut
    {
      add { }
      remove { }
    }

    public IState Process(Message message)
    {
      throw new InvalidOperationException();
    }
  }
}