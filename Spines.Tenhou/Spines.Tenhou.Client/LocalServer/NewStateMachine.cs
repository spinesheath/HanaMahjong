// Spines.Tenhou.Client.NewStateMachine.cs
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
using Spines.Tenhou.Client.LocalServer.States;
using Spines.Utility;

namespace Spines.Tenhou.Client.LocalServer
{
  internal class NewStateMachine
  {
    private readonly ConcurrentQueue<Message> _messages = new ConcurrentQueue<Message>();
    private INewState _currentState;

    public NewStateMachine(INewState firstState)
    {
      _currentState = firstState;
      _currentState.TimedOut += OnTimedOut;
    }

    public event EventHandler Finished;

    public void Process(Message message)
    {
      _messages.Enqueue(message);
      DequeueOne();
    }

    private void DequeueOne()
    {
      bool isFinal;
      lock (_currentState)
      {
        if (_currentState.IsFinal)
        {
          return;
        }
        Message m;
        if (_messages.TryDequeue(out m))
        {
          UpdateCurrentState(_currentState.Process(m));
        }
        else
        {
          throw new ClientException("Could not dequeue message.");
        }
        isFinal = _currentState.IsFinal;
      }
      if (isFinal)
      {
        // TODO cleanup, like leaving queues and matches
        EventUtility.CheckAndRaise(Finished, this);
      }
    }

    private void OnTimedOut(object sender, StateTimedOutEventArgs e)
    {
      lock (_currentState)
      {
        UpdateCurrentState(e.NextState);
      }
    }

    private void UpdateCurrentState(INewState nextState)
    {
      _currentState.TimedOut -= OnTimedOut;
      _currentState = nextState;
      _currentState.TimedOut += OnTimedOut;
    }
  }
}