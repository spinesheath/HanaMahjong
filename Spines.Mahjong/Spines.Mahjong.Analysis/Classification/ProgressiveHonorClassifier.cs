// Spines.Mahjong.Analysis.ProgressiveHonorClassifier.cs
// 
// Copyright (C) 2017  Johannes Heckl
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

using System.Collections.Generic;
using System.Linq;

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// Returns the arrangement value of honors after the execution of a single action.
  /// </summary>
  internal class ProgressiveHonorClassifier
  {
    public int Value { get; private set; }

    /// <summary>
    /// Advances the current state by one and returns the arrangement value of the new state.
    /// </summary>
    /// <param name="actionId">The id of the action used to advance the state.</param>
    /// <returns>The arrangement value of the new state.</returns>
    public void MoveNext(int actionId)
    {
      _current = Transitions[_current + actionId + 1];
      Value = Transitions[_current];
    }

    /// <summary>
    /// Pushes the current state on an internal stack.
    /// </summary>
    public void Push()
    {
      _stack.Push(_current);
    }

    /// <summary>
    /// Resets the current state to the state on top of the internal stack.
    /// </summary>
    public void Pop()
    {
      _current = _stack.Pop();
    }

    private static readonly ushort[] Transitions = Parse.Transitions("ProgressiveHonorStateMachine.txt").ToArray();
    private int _current;
    private readonly Stack<int> _stack = new Stack<int>();
  }
}