// Spines.Tools.AnalyzerBuilder.Creator.cs
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
  /// <summary>
  /// Creates the data for Shanten Calculations from scratch.
  /// If any intermediate data is present already, that part of the creation is skipped.
  /// </summary>
  internal class Creator
  {
    private readonly string _workingDirectory;

    /// <summary>
    /// Creates a new Instance of Creator.
    /// </summary>
    /// <param name="workingDirectory">The directory where intermediate results are stored.</param>
    public Creator(string workingDirectory)
    {
      _workingDirectory = workingDirectory;
    }

    /// <summary>
    /// Creates the data for Shanten Calculations from scratch.
    /// If any intermediate data is present already, that part of the creation is skipped.
    /// </summary>
    public void Create()
    {
      new ArragementTransitionsCreator(_workingDirectory).Create();


      CreateHonorTransitions();
      CreateSuitTransitions();
    }

    private void CreateSuitTransitions()
    {
      var targetPath = Path.Combine(_workingDirectory, "SuitTransitions.txt");
      if (File.Exists(targetPath))
      {
        return;
      }

      var words = new CompactAnalyzedDataCreator(_workingDirectory).CreateSuitWords();
      // (MeldedTiles, ConcealedTiles, MeldCount)
      //var mhc = words.Select(w => new WordWithValue(Swap(w.Word, 2, 0, 1, 9), w.Value));

      var compactedTransitions = GetCompactedTransitions(words);

      var lines = compactedTransitions.Select(t => t.ToString(CultureInfo.InvariantCulture));
      File.WriteAllLines(targetPath, lines);
    }

    private void CreateHonorTransitions()
    {
      var targetPath = Path.Combine(_workingDirectory, "HonorTransitions.txt");
      if (File.Exists(targetPath))
      {
        return;
      }

      var words = new CompactAnalyzedDataCreator(_workingDirectory).CreateHonorWords();
      // (MeldedTiles, ConcealedTiles, MeldCount)
      //var mhc = words.Select(w => new WordWithValue(Swap(w.Word, 2, 0, 1, 7), w.Value));

      var compactedTransitions = GetCompactedTransitions(words);

      var lines = compactedTransitions.Select(t => t.ToString(CultureInfo.InvariantCulture));
      File.WriteAllLines(targetPath, lines);
    }

    private static IEnumerable<int> Swap(IReadOnlyList<int> word, int sortOfMeldCount, int sortOfMeldedTiles, int sortOfConcealedTiles, int typesInSuit)
    {
      for (var i = 0; i < 3; ++i)
      {
        if (sortOfMeldCount == i)
        {
          yield return word[0];
        }
        else if (sortOfMeldedTiles == i)
        {
          foreach (var c in word.Skip(1).Take(typesInSuit))
          {
            yield return word[c];
          }
        }
        else if (sortOfConcealedTiles == i)
        {
          foreach (var c in word.Skip(1 + typesInSuit).Take(typesInSuit))
          {
            yield return word[c];
          }
        }
      }
    }

    private static IEnumerable<int> GetCompactedTransitions(IEnumerable<WordWithValue> words)
    {
      var classifierBuilder = new ClassifierFactory().Create(words);
      var transitions = classifierBuilder.CreateTransitions().ToList();
      var resultIndices = new HashSet<int>(classifierBuilder.GetResultIndices());
      var alphabetSize = classifierBuilder.AlphabetSize;
      return CompactTransitions(transitions, resultIndices, alphabetSize);
    }

    private static IEnumerable<int> CompactTransitions(IReadOnlyList<int> transitions, ICollection<int> resultIndices, int alphabetSize)
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