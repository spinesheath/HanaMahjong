// Spines.Tenhou.Client.LimitedTimeState.cs
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

using System.Diagnostics;
using System.Xml.Linq;
using Spines.Tenhou.Client.LocalServer.Transitions;

namespace Spines.Tenhou.Client.LocalServer.States
{
  /// <summary>
  /// Base class for states that can time out.
  /// </summary>
  internal abstract class LimitedTimeState<TSender, THost> : IState<TSender, THost>
  {
    private readonly int _milliseconds;
    private readonly Stopwatch _stopwatch = new Stopwatch();

    /// <summary>
    /// Creates a new instance of LimitedTimeState that times out after 10 seconds.
    /// </summary>
    protected LimitedTimeState()
    {
      _milliseconds = 10000;
      _stopwatch.Start();
    }

    /// <summary>
    /// Creates a new instance of LimitedTimeState that times out after a given amount of time.
    /// </summary>
    /// <param name="milliseconds">Milliseconds until timeout.</param>
    protected LimitedTimeState(int milliseconds)
    {
      _milliseconds = milliseconds;
      _stopwatch.Start();
    }

    public abstract IStateTransition<TSender, THost> Process(XElement message);

    public bool IsFinal
    {
      get { return false; }
    }

    public IStateTransition<TSender, THost> ProcessEmpty()
    {
      return IsTimedOut() ? CreateTimeOutTransition() : new DoNothingTransition<TSender, THost>(this);
    }

    protected abstract IStateTransition<TSender, THost> CreateTimeOutTransition();

    /// <summary>
    /// Resets the timer of the state.
    /// </summary>
    protected void ResetTimer()
    {
      _stopwatch.Restart();
    }

    /// <summary>
    /// Checks if the time limit has been exceeded.
    /// </summary>
    /// <returns>True if the state has timed out, false otherwise.</returns>
    private bool IsTimedOut()
    {
      return _stopwatch.ElapsedMilliseconds > _milliseconds;
    }
  }
}