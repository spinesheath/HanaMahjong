// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Spines.Tenhou.Client.LocalServer.States
{
  internal interface IState
  {
    bool IsFinal { get; }
    event EventHandler<StateTimedOutEventArgs> TimedOut;
    IState Process(Message message);
  }
}