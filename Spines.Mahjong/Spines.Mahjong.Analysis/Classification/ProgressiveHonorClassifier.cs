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

using System.Linq;

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// Returns the arrangement value of honors after the execution of a single action.
  /// </summary>
  internal struct ProgressiveHonorClassifier
  {
    /// <summary>
    /// Creates an instance of ProgressiveHonorClassifier with the same state as the current one.
    /// </summary>
    /// <returns>A new instance of ProgressiveHonorClassifier.</returns>
    public ProgressiveHonorClassifier Fork()
    {
      return new ProgressiveHonorClassifier {_current = _current};
    }

    /// <summary>
    /// Advances the current state by one and returns the arrangement value of the new state.
    /// </summary>
    /// <param name="actionId">The id of the action used to advance the state.</param>
    /// <returns>The arrangement value of the new state.</returns>
    /// <remarks>
    /// concealed, melded, action, actionId
    /// 0, 0, draw 0
    /// 1, 0, draw 1
    /// 2, 0, draw 2
    /// 3, 0, draw 3
    /// 0, 3, draw 4
    /// 1, 0, discard 5
    /// 2, 0, discard 6
    /// 3, 0, discard 7
    /// 4, 0, discard 8
    /// 1, 3, discard 9
    /// 2, 0, pon 10
    /// 3, 0, pon 11
    /// 3, 0, daiminkan 12
    /// 1, 3, chakan 13
    /// 4, 0, ankan 14
    /// </remarks>
    public int MoveNext(int actionId)
    {
      _current = Transitions[_current + actionId + 1];
      return Transitions[_current];
    }

    private static readonly ushort[] Transitions = Resource.Transitions("ProgressiveHonorStateMachine.txt").ToArray();
    private int _current;
  }
}