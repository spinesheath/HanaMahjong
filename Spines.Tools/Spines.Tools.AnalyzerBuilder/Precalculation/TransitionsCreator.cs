// Spines.Tools.AnalyzerBuilder.TransitionsCreator.cs
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
using System.Globalization;
using System.IO;
using System.Linq;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Tools.AnalyzerBuilder.Precalculation
{
  internal class TransitionsCreator
  {
    private readonly string _workingDirectory;

    /// <summary>
    /// Creates a new Instance of TransitionsCreator.
    /// </summary>
    /// <param name="workingDirectory">The directory where intermediate results are stored.</param>
    public TransitionsCreator(string workingDirectory)
    {
      _workingDirectory = workingDirectory;
    }

    public void CreateArrangementTransitions()
    {
      CreateTransitions("ArrangementTransitions.txt", () => new ArrangementWordCreator(_workingDirectory).CreateOrdered());
    }

    public void CreateSuitTransitions()
    {
      CreateTransitions("SuitTransitions.txt", () => new CompactAnalyzedDataCreator(_workingDirectory).CreateSuitWords());
    }

    public void CreateHonorTransitions()
    {
      CreateTransitions("HonorTransitions.txt", () => new CompactAnalyzedDataCreator(_workingDirectory).CreateHonorWords());
    }

    /// <summary>
    /// Creates the transitions file if it doesn't exist.
    /// </summary>
    public void CreateTransitions(string fileName, Func<IEnumerable<WordWithValue>> wordCreator)
    {
      var targetPath = Path.Combine(_workingDirectory, fileName);
      if (File.Exists(targetPath))
      {
        return;
      }

      var words = wordCreator();
      var compactedTransitions = GetCompactedTransitions(words);

      var lines = compactedTransitions.Select(t => t.ToString(CultureInfo.InvariantCulture));
      File.WriteAllLines(targetPath, lines);
    }

    private static IEnumerable<int> GetCompactedTransitions(IEnumerable<WordWithValue> words)
    {
      var classifierBuilder = new ClassifierFactory().Create(words);
      var transitions = classifierBuilder.CreateTransitions().ToList();
      var resultIndices = new HashSet<int>(classifierBuilder.GetResultIndexes());
      var alphabetSize = classifierBuilder.AlphabetSize;
      return CompactTransitions(transitions, resultIndices, alphabetSize);
    }

    /// <summary>
    /// Compacts transitions by eliminating trailing null tansitions.
    /// I.e if the last couple characters in the alphabet, for one state don't lead to another state, these cells can be eliminated from the table.
    /// Transitions that point to a later state are adjusted by the amount of eliminated cells.
    /// </summary>
    private static IEnumerable<int> CompactTransitions(IReadOnlyList<int> transitions, ICollection<int> resultIndices, int alphabetSize)
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
          if (transitions[transition] != 0 || resultIndices.Contains(transition)) // Results can be 0 but can't be skipped.
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
        if (clone[i] == 0) // nulls are not adjusted.
        {
          continue;
        }
        if (resultIndices.Contains(i)) // Results are not adjusted.
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