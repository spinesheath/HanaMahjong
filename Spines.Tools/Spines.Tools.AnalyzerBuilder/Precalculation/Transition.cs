// Spines.Tools.AnalyzerBuilder.Transition.cs
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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Spines.Tools.AnalyzerBuilder.Precalculation
{
  internal class Transition
  {
    /// <summary>
    /// Counts the number of null transitions per character in the alphabet.
    /// </summary>
    public static IEnumerable<int> CountNullTransitions(IReadOnlyList<int> transitions, int alphabetSize, Func<int, bool> isNull, Func<int, bool> isResult)
    {
      var nullTransitions = new int[alphabetSize];
      for (var i = 0; i < transitions.Count; i++)
      {
        if (isResult(i))
        {
          continue;
        }
        if (isNull(i))
        {
          nullTransitions[i % alphabetSize] += 1;
        }
      }
      return nullTransitions;
    }

    /// <summary>
    /// Compacts transitions by eliminating trailing null tansitions.
    /// I.e if the last couple characters in the alphabet, for one state don't lead to another state, these cells can be
    /// eliminated from the table.
    /// Transitions that point to a later state are adjusted by the amount of eliminated cells.
    /// </summary>
    public static IEnumerable<int> Compact(IReadOnlyCollection<int> transitions, int alphabetSize,
      Func<int, bool> isNull, Func<int, bool> isResult)
    {
      // Build a set of indices that can be skipped and a table of offsets for adjusting the remaining transitions.
      var skippedIndices = new HashSet<int>();
      var offsetMap = new int[transitions.Count];
      var skipTotal = 0;
      for (var i = 0; i < transitions.Count; i += alphabetSize)
      {
        // Count the trailing nulls.
        var transitionsToKeep = alphabetSize;
        for (; transitionsToKeep > 0; transitionsToKeep--)
        {
          var transition = i + transitionsToKeep - 1;
          if (!isNull(transition) || isResult(transition)) // Results and normal transitions can't be skipped.
          {
            break;
          }
          skippedIndices.Add(transition);
        }
        // Build the set and table.
        var toSkip = alphabetSize - transitionsToKeep;
        skipTotal += toSkip;
        for (var j = 0; j < alphabetSize; ++j)
        {
          var index = i + alphabetSize + j;
          if (index >= offsetMap.Length)
          {
            break;
          }
          offsetMap[index] = skipTotal;
        }
      }

      // Adjust the remaining transitions.
      var clone = transitions.ToList();
      for (var i = 0; i < clone.Count; ++i)
      {
        if (isNull(i) || isResult(i)) // nulls and results are not adjusted.
        {
          continue;
        }

        clone[i] -= offsetMap[clone[i]];
      }

      // Copy into a new list while skipping eliminated cells.
      return clone.Where((t, i) => !skippedIndices.Contains(i)).ToList();
    }
  }
}