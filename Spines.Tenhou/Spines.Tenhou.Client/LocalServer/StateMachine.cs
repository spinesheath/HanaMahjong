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
using Spines.Utility;

namespace Spines.Tenhou.Client.LocalServer
{
  internal class StateMachine<THost>
  {
    private readonly ConcurrentQueue<IStateTransition<THost>> _transitions =
      new ConcurrentQueue<IStateTransition<THost>>();
    private readonly THost _host;
    private IState<THost> _currentState;

    public StateMachine(THost host, IState<THost> firstState)
    {
      _host = host;
      _currentState = firstState;
      AdvanceStateAsync();
    }

    public event EventHandler Finished;

    public void ProcessMessage(XElement message)
    {
      Process(s => s.Process(message));
    }

    /// <summary>
    /// Advances to the next state and stores the transition.
    /// </summary>
    /// <param name="transitionGetter">Used to get the state transmission from the current state.</param>
    private void AdvanceState(Func<IState<THost>, IStateTransition<THost>> transitionGetter)
    {
      lock (_currentState)
      {
        var transition = transitionGetter(_currentState);
        _currentState = transition.NextState;
        _transitions.Enqueue(transition);
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
          IStateTransition<THost> transition;
          if (!_transitions.TryDequeue(out transition))
          {
            break;
          }
          transition.Execute(_host);
        }
      }
    }

    /// <summary>
    /// Advances the current state and executes transitions.
    /// </summary>
    /// <param name="transitionGetter">Used to get the state transmission from the current state.</param>
    private void Process(Func<IState<THost>, IStateTransition<THost>> transitionGetter)
    {
      AdvanceState(transitionGetter);
      ExecuteTransitions();
      if (_currentState.IsFinal)
      {
        EventUtility.CheckAndRaise(Finished, this);
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
        Thread.Sleep(100);
        Process(s => s.ProcessEmpty());
      }
    }
  }
}