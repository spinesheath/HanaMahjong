// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Spines.Utility;

namespace Spines.Tenhou.Client.LocalServer.States
{
  internal abstract class StateBase : IState
  {
    public bool IsFinal
    {
      get { return false; }
    }

    public event EventHandler<StateTimedOutEventArgs> TimedOut;

    public abstract IState Process(Message message);

    protected StateBase()
      : this(5000)
    {
    }

    protected StateBase(int milliseconds)
    {
      _milliseconds = milliseconds;
      WaitForTimeOutAsync();
    }

    protected virtual IState GetTimedOutState()
    {
      return new FinalState();
    }

    protected void RestartTimer()
    {
      lock (_stopwatch)
      {
        _stopwatch.Restart();
      }
    }

    private readonly int _milliseconds;
    private readonly Stopwatch _stopwatch = new Stopwatch();

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
          Validate.InvokeSafely(TimedOut, this, new StateTimedOutEventArgs(GetTimedOutState()));
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
}