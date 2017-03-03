// Spines.Tools.AnalyzerBuilder.SuitFirstPhaseBuilder.cs
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
using Spines.Utility;

namespace Spines.Tools.AnalyzerBuilder.Precalculation
{
  internal class SuitFirstPhaseBuilder : IStateMachineBuilder
  {
    private readonly string _workingDirectory;

    public SuitFirstPhaseBuilder(string workingDirectory)
    {
      _workingDirectory = workingDirectory;
    }

    /// <summary>
    /// The size of the alphabet.
    /// </summary>
    public int AlphabetSize { get; } = 0;

    /// <summary>
    /// The transitions for the specified language.
    /// </summary>
    public IReadOnlyList<int> Transitions { get; } = new List<int>();

    public void SetLanguage()
    {
      var newLanguage = CreateNewLanguage();

      // Transform into transition table, with one extra character for the transition to the final states?

      var columns = new List<int[]>();
      // Value to stateId.
      var finalValues = new Dictionary<int, int>();
      columns.Add(new int[26]);
      foreach (var word in newLanguage)
      {
        var current = 0;
        foreach (var c in word)
        {
          if (columns[current][c] == 0)
          {
            columns.Add(new int[26]);
            columns[current][c] = columns.Count - 1;
          }
          current = columns[current][c];
        }

        if (!finalValues.ContainsKey(word.Value))
        {
          finalValues.Add(word.Value, finalValues.Count);
        }
        columns[current][25] = finalValues[word.Value];
      }

      var incoming = new List<List<List<int>>>();
      for (var i = 0; i < columns.Count; ++i)
      {
        incoming.Add(new List<List<int>>());
        for (var c = 0; c < 26; ++c)
        {
          incoming[i].Add(new List<int>());
        }
      }
      for (var i = 0; i < columns.Count; ++i)
      {
        for (var c = 0; c < 25; ++c)
        {
          var t = columns[i][c];
          if (t != 0)
          {
            incoming[t][c].Add(i);
          }
        }
      }
      for (var i = 0; i < finalValues.Count; ++i)
      {
        incoming.Add(new List<List<int>>());
        for (var c = 0; c < 26; ++c)
        {
          incoming[i + columns.Count].Add(new List<int>());
        }
        for (var j = 0; j < columns.Count; ++j)
        {
          if (columns[j][25] == i)
          {
            incoming[i + columns.Count][25].Add(j);
          }
        }
      }

      // Apply Hopcroft
      Func<HashSet<int>, int, IEnumerable<int>> getIncomingStates = (a, c) => GetIncomingStates(incoming, a, c);
      var normalStates = Enumerable.Range(0, columns.Count);
      var finalStates = Enumerable.Range(columns.Count, finalValues.Count);
      var h = new Hopcroft(normalStates, finalStates, 26, getIncomingStates);
    }

    private static IEnumerable<int> GetIncomingStates(List<List<List<int>>> incoming, IEnumerable<int> a, int c)
    {
      return a.SelectMany(aa => incoming[aa][c]);
    }

    //private static IEnumerable<int> GetIncomingStates(IReadOnlyList<int[]> columns, IEnumerable<int> a, int c)
    //{
    //  foreach (var s in a)
    //  {
    //    if (s >= columns.Count && c == 25) // final
    //    {
    //      var f = s - columns.Count;
    //      for (var i = 0; i < columns.Count; ++i)
    //      {
    //        if (columns[i][25] == f)
    //        {
    //          yield return i;
    //        }
    //      }
    //    }
    //    else if (s < columns.Count && c != 25) // normal
    //    {
    //      for (var i = 0; i < columns.Count; ++i)
    //      {
    //        if (columns[i][c] == s)
    //        {
    //          yield return i;
    //        }
    //      }
    //    }
    //  }
    //}

    private IEnumerable<WordWithValue> CreateNewLanguage()
    {
      var meldWords = GetMeldWords();

      // 3 bit per tile count, 30 bit total
      // create dictionary from 30 bit to result
      var results = new Dictionary<int, int>();
      foreach (var word in meldWords)
      {
        var id = word.Word.Select((c, i) => c << i * 3).Sum();
        results.Add(id, word.Value);
      }

      // create 25x25x25x25 language
      var newBaseLanguage = GetMeldLanguage();

      // count the used tiles
      // get result from dictionary
      foreach (var w in newBaseLanguage)
      {
        var word = w.ToList();
        var oldWord = new int[10];
        foreach (var c in word)
        {
          oldWord[0] += 1;
          if (c < 7)
          {
            oldWord[c + 1] += 1;
            oldWord[c + 2] += 1;
            oldWord[c + 3] += 1;
          }
          else if (c < 16)
          {
            oldWord[c - 6] += 3;
          }
          else if (c < 25)
          {
            oldWord[c - 15] += 4;
          }
        }
        if (oldWord.Any(c => c > 4))
        {
          continue;
        }
        var id = oldWord.Select((c, i) => c << i * 3).Sum();
        if (results.ContainsKey(id))
        {
          yield return new WordWithValue(word, results[id]);
        }
      }
    }

    private IEnumerable<WordWithValue> GetMeldWords()
    {
      var transitions = GetOldTransitions();

      var meldWords = new List<WordWithValue>();
      var baseLanguage = Enumerable.Repeat(Enumerable.Range(0, 5), 10).CartesianProduct();
      foreach (var word in baseLanguage)
      {
        var w = word.ToList();
        var current = 0;
        foreach (var c in w)
        {
          current = transitions[current + c];
          if (current == 0)
          {
            break;
          }
        }
        if (current != 0)
        {
          meldWords.Add(new WordWithValue(w, current));
        }
      }
      return meldWords;
    }

    private IReadOnlyList<int> GetOldTransitions()
    {
      var path = Path.Combine(_workingDirectory, "UncompressedSuitTransitions.txt");
      if (File.Exists(path))
      {
        return File.ReadAllLines(path).Select(line => Convert.ToInt32(line, CultureInfo.InvariantCulture)).ToList();
      }
      var language = new CompactAnalyzedDataCreator(_workingDirectory).CreateSuitWords();
      var builder = new ClassifierBuilder();
      builder.SetLanguage(language);
      var transitions = builder.Transitions;
      var lines = transitions.Select(t => t.ToString(CultureInfo.InvariantCulture));
      File.WriteAllLines(path, lines);
      return transitions;
    }

    /// <summary>
    /// 0 to 4 characters from 0 to 24.
    /// </summary>
    private static IEnumerable<IEnumerable<int>> GetMeldLanguage()
    {
      return Enumerable.Range(0, 5).SelectMany(length => Enumerable.Repeat(Enumerable.Range(0, 25), length).CartesianProduct());
    }

    /// <summary>
    /// Is the transition one that describes can not be reached with a legal word?
    /// </summary>
    /// <param name="transition">The Id of the transtion.</param>
    /// <returns>True, if the transition can not be reached, false otherwise.</returns>
    public bool IsNull(int transition)
    {
      return false;
    }

    /// <summary>
    /// Is the transition one that describes a result?
    /// </summary>
    /// <param name="transition">The Id of the transtion.</param>
    /// <returns>True, if the transition is a result, false otherwise.</returns>
    public bool IsResult(int transition)
    {
      return false;
    }
  }
}