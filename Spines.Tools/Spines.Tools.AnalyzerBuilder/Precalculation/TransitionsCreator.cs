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
      var arrangementTransitionsPath = Path.Combine(_workingDirectory, "ArrangementTransitions.txt");
      if (File.Exists(arrangementTransitionsPath))
      {
        return;
      }

      var words = new ArrangementWordCreator(_workingDirectory).CreateOrdered().ToList();
      var compactedTransitions = GetCompactedTransitions(words);

      var lines = compactedTransitions.Select(t => t.ToString(CultureInfo.InvariantCulture));
      File.WriteAllLines(arrangementTransitionsPath, lines);
    }

    public void CreateSuitTransitions()
    {
      var targetPath = Path.Combine(_workingDirectory, "SuitTransitions.txt");
      if (File.Exists(targetPath))
      {
        return;
      }

      var words = new CompactAnalyzedDataCreator(_workingDirectory).CreateSuitWords();
      var compactedTransitions = GetCompactedTransitions(words);

      var lines = compactedTransitions.Select(t => t.ToString(CultureInfo.InvariantCulture));
      File.WriteAllLines(targetPath, lines);
    }

    public void CreateHonorTransitions()
    {
      var targetPath = Path.Combine(_workingDirectory, "HonorTransitions.txt");
      if (File.Exists(targetPath))
      {
        return;
      }

      var words = new CompactAnalyzedDataCreator(_workingDirectory).CreateHonorWords();
      var compactedTransitions = GetCompactedTransitions(words);

      var lines = compactedTransitions.Select(t => t.ToString(CultureInfo.InvariantCulture));
      File.WriteAllLines(targetPath, lines);
    }

    private static IEnumerable<int> GetCompactedTransitions(IEnumerable<WordWithValue> words)
    {
      var classifierBuilder = new ClassifierFactory().Create(words);
      var transitions = classifierBuilder.CreateTransitions().ToList();
      var resultIndices = new HashSet<int>(classifierBuilder.GetResultIndices());
      var alphabetSize = classifierBuilder.AlphabetSize;
      return CompactTransitions(transitions, resultIndices, alphabetSize);
    }

    private static IEnumerable<int> CompactTransitions(IReadOnlyList<int> transitions, ICollection<int> resultIndices,
      int alphabetSize)
    {
      var skippedIndices = new HashSet<int>();
      var offsetMap = new int[transitions.Count];
      var skipTotal = 0;
      for (var i = 0; i < transitions.Count; i += alphabetSize)
      {
        var transitionsToKeep = alphabetSize;
        for (; transitionsToKeep > 0; transitionsToKeep--)
        {
          var transition = i + transitionsToKeep - 1;
          if (transitions[transition] != 0 || resultIndices.Contains(transition))
          {
            break;
          }
          skippedIndices.Add(transition);
        }
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

      var clone = transitions.ToList();
      for (var i = 0; i < clone.Count; ++i)
      {
        if (clone[i] == 0)
        {
          continue;
        }
        if (resultIndices.Contains(i))
        {
          continue;
        }

        clone[i] -= offsetMap[clone[i]];
      }

      var compactedTransitions = new List<int>();
      for (var i = 0; i < clone.Count; ++i)
      {
        if (skippedIndices.Contains(i))
        {
          continue;
        }
        compactedTransitions.Add(clone[i]);
      }

      return compactedTransitions;
    }
  }
}