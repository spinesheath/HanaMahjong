// Spines.Tenhou.Client.StateBase.cs
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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Spines.Utility;

namespace Spines.Tenhou.Client.LocalServer.States
{
  internal abstract class StateBase : IState
  {
    private readonly int _milliseconds;
    private readonly Stopwatch _stopwatch = new Stopwatch();

    protected StateBase()
      : this(5000)
    {
    }

    protected StateBase(int milliseconds)
    {
      _milliseconds = milliseconds;
      WaitForTimeOutAsync();
    }

    public bool IsFinal
    {
      get { return false; }
    }

    public abstract IState Process(Message message);

    public event EventHandler<StateTimedOutEventArgs> TimedOut;

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