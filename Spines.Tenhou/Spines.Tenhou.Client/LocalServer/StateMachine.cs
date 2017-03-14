// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using Spines.Tenhou.Client.LocalServer.States;
using Spines.Utility;

namespace Spines.Tenhou.Client.LocalServer
{
  // TODO get rid of thread stuff here and use a message queue between this and the senders
  internal class StateMachine
  {
    public StateMachine(IState firstState)
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

    private readonly ConcurrentQueue<Message> _messages = new ConcurrentQueue<Message>();
    private IState _currentState;

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
        Validate.InvokeSafely(Finished, this);
      }
    }

    private void OnTimedOut(object sender, StateTimedOutEventArgs e)
    {
      lock (_currentState)
      {
        UpdateCurrentState(e.NextState);
      }
    }

    private void UpdateCurrentState(IState nextState)
    {
      _currentState.TimedOut -= OnTimedOut;
      _currentState = nextState;
      _currentState.TimedOut += OnTimedOut;
    }
  }
}