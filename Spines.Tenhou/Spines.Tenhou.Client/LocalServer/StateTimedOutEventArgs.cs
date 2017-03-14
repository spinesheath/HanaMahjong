// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Spines.Tenhou.Client.LocalServer.States;

namespace Spines.Tenhou.Client.LocalServer
{
  internal class StateTimedOutEventArgs : EventArgs
  {
    public StateTimedOutEventArgs(IState nextState)
    {
      NextState = nextState;
    }

    public IState NextState { get; private set; }
  }
}