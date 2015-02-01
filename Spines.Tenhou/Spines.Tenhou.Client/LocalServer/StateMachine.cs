// Spines.Tenhou.Client.StateMachine.cs
// 
// Copyright (C) 2015  Johannes Heckl
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Spines.Tenhou.Client.LocalServer.States;
using Spines.Tenhou.Client.LocalServer.Transitions;
using Spines.Utility;

namespace Spines.Tenhou.Client.LocalServer
{
  internal class StateMachine<TSender, THost>
  {
    private readonly ConcurrentQueue<IStateTransition<TSender, THost>> _transitions =
      new ConcurrentQueue<IStateTransition<TSender, THost>>();
    private readonly THost _host;
    private IState<TSender, THost> _currentState;

    public StateMachine(THost host, IState<TSender, THost> firstState)
    {
      _host = host;
      _currentState = firstState;
      AdvanceStateAsync();
    }

    public event EventHandler Finished;

    public void ProcessMessage(TSender sender, XElement message)
    {
      lock (_currentState)
      {
        var transition = _currentState.Process(message);
        _currentState = transition.PrepareNextState(sender, _host);
        _transitions.Enqueue(transition);
      }
      ExecuteTransitions();
      if (_currentState.IsFinal)
      {
        EventUtility.CheckAndRaise(Finished, this);
      }
    }

    /// <summary>
    /// Asynchronously sends empty messages until a final state is reached.
    /// This is necessary to allow state transitions by timeout.
    /// </summary>
    private async void AdvanceStateAsync()
    {
      await Task.Run(() => SendEmptyMessages());
    }

    /// <summary>
    /// Executes all stored transitions in order.
    /// </summary>
    /// <remarks>Needs the lock or else some other thread might enter this method and mess with the order.</remarks>
    private void ExecuteTransitions()
    {
      lock (_transitions)
      {
        while (!_transitions.IsEmpty)
        {
          IStateTransition<TSender, THost> transition;
          if (!_transitions.TryDequeue(out transition))
          {
            break;
          }
          transition.Execute(_host);
        }
      }
    }

    /// <summary>
    /// Repeatedly sends empty messages until a final state is reached.
    /// This is necessary to allow state transitions by timeout.
    /// </summary>
    private void SendEmptyMessages()
    {
      while (!_currentState.IsFinal)
      {
        Thread.Sleep(500);
        lock (_currentState)
        {
          var transition = _currentState.ProcessEmpty();
          _currentState = transition.PrepareNextStateEmpty(_host);
          _transitions.Enqueue(transition);
        }
        ExecuteTransitions();
        if (_currentState.IsFinal)
        {
          EventUtility.CheckAndRaise(Finished, this);
        }
      }
    }
  }
}