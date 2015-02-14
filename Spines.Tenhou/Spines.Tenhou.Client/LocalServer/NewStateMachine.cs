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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Spines.Utility;

namespace Spines.Tenhou.Client.LocalServer
{
  internal interface INewState
  {
    bool IsFinal { get; }
    event EventHandler<StateTimedOutEventArgs> TimedOut;
    NewState Process(Message message);
  }

  internal class Message
  {
    public Message(string senderId, XElement content)
    {
      Content = content;
      SenderId = senderId;
    }

    public XElement Content { get; private set; }
    public string SenderId { get; private set; }
  }

  internal sealed class NewFinalState : INewState
  {
    public bool IsFinal
    {
      get { return true; }
    }

    public NewState Process(Message message)
    {
      throw new InvalidOperationException("FinalState can't process any messages.");
    }

    public event EventHandler<StateTimedOutEventArgs> TimedOut;
  }

  internal abstract class NewState : INewState
  {
    private readonly int _milliseconds;
    private readonly Stopwatch _stopwatch = new Stopwatch();

    protected NewState(int milliseconds)
    {
      _milliseconds = milliseconds;
      WaitForTimeOutAsync();
    }

    public bool IsFinal
    {
      get { return false; }
    }

    public abstract NewState Process(Message message);

    public event EventHandler<StateTimedOutEventArgs> TimedOut;
    protected abstract INewState GetTimedOutState();

    protected void RestartTimer()
    {
      lock (_stopwatch)
      {
        _stopwatch.Restart();
      }
    }

    private void WaitForTimeOut()
    {
      while (true)
      {
        int remaining;
        lock (_stopwatch)
        {
          remaining = (int) (_milliseconds - _stopwatch.ElapsedMilliseconds);
        }
        if (remaining <= 0)
        {
          EventUtility.CheckAndRaise(TimedOut, this, new StateTimedOutEventArgs(GetTimedOutState()));
          break;
        }
        Thread.Sleep(remaining);
      }
    }

    private async void WaitForTimeOutAsync()
    {
      RestartTimer();
      await Task.Run(() => WaitForTimeOut());
    }
  }

  internal class StateTimedOutEventArgs : EventArgs
  {
    public StateTimedOutEventArgs(INewState nextState)
    {
      NextState = nextState;
    }

    public INewState NextState { get; private set; }
  }

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
      DequeueOneAsync();
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
          _currentState = _currentState.Process(m);
        }
        else
        {
          throw new ClientException("Could not dequeue message.");
        }
        isFinal = _currentState.IsFinal;
      }
      if (isFinal)
      {
        EventUtility.CheckAndRaise(Finished, this);
      }
    }

    private async void DequeueOneAsync()
    {
      await Task.Run(() => DequeueOne());
    }

    private void OnTimedOut(object sender, StateTimedOutEventArgs e)
    {
      lock (_currentState)
      {
        _currentState.TimedOut -= OnTimedOut;
        _currentState = e.NextState;
        _currentState.TimedOut += OnTimedOut;
      }
    }
  }
}