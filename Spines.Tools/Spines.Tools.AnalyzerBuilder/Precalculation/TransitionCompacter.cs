// Spines.Tools.AnalyzerBuilder.TransitionCompacter.cs
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
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Tools.AnalyzerBuilder.Precalculation
{
  internal class TransitionCompacter
  {
    /// <summary>
    /// Compacts transitions by eliminating trailing null tansitions.
    /// I.e if the last couple characters in the alphabet, for one state don't lead to another state, these cells can be
    /// eliminated from the table.
    /// Transitions that point to a later state are adjusted by the amount of eliminated cells.
    /// </summary>
    public TransitionCompacter(IStateMachineBuilder builder)
    {
      // Build a set of indices that can be skipped and a table of offsets for adjusting the remaining transitions.
      var skippedIndices = new HashSet<int>();
      var offsetMap = new int[builder.Transitions.Count];
      var skipTotal = 0;
      for (var i = 0; i < builder.Transitions.Count; i += builder.AlphabetSize)
      {
        // Count the trailing nulls.
        var transitionsToKeep = builder.AlphabetSize;
        for (; transitionsToKeep > 0; transitionsToKeep--)
        {
          var transition = i + transitionsToKeep - 1;
          if (!builder.IsNull(transition) || builder.IsResult(transition))
            // Results and normal transitions can't be skipped.
          {
            break;
          }
          skippedIndices.Add(transition);
        }
        // Build the set and table.
        var toSkip = builder.AlphabetSize - transitionsToKeep;
        skipTotal += toSkip;
        for (var j = 0; j < builder.AlphabetSize; ++j)
        {
          var index = i + builder.AlphabetSize + j;
          if (index >= offsetMap.Length)
          {
            break;
          }
          offsetMap[index] = skipTotal;
        }
      }

      // Adjust the remaining transitions.
      var clone = builder.Transitions.ToList();
      for (var i = 0; i < clone.Count; ++i)
      {
        if (builder.IsNull(i) || builder.IsResult(i)) // nulls and results are not adjusted.
        {
          continue;
        }

        clone[i] -= offsetMap[clone[i]];
      }

      // Copy into a new list while skipping eliminated cells.
      Transitions = clone.Where((t, i) => !skippedIndices.Contains(i)).ToList();

      // Create offsets.
      var offsets = new List<int>();
      for (var i = 0; i < builder.Transitions.Count; i += builder.AlphabetSize)
      {
        offsets.Add(offsetMap[i]);
      }
      Offsets = offsets;
    }

    /// <summary>
    /// The transitions after compaction.
    /// </summary>
    public IReadOnlyList<int> Transitions { get; }

    /// <summary>
    /// Mapping from stateId before compaction to offset of state after compaction.
    /// </summary>
    public IReadOnlyList<int> Offsets { get; }
  }
}