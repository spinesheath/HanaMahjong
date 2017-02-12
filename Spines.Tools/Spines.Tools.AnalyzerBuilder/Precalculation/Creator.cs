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
using System.IO;
using System.Linq;
using Spines.Mahjong.Analysis.Classification;
using Spines.Mahjong.Analysis.Combinations;

namespace Spines.Tools.AnalyzerBuilder.Precalculation
{
  /// <summary>
  /// Creates the data for Shanten Calculations from scratch.
  /// If any intermediate data is present already, that part of the creation is skipped.
  /// </summary>
  internal class Creator
  {
    private const int HandLength = 1 + 9 + 9;
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
      var wordsFile = Path.Combine(_workingDirectory, "ArrangementWords.txt");
      if (File.Exists(wordsFile))
      {
        return;
      }
      var arrangements = GetAllArrangements();
      var words = CreateWords(arrangements);
      File.WriteAllLines(wordsFile, words.Select(w => w.ToString()));
    }

    private IEnumerable<List<Arrangement>> GetAllArrangements()
    {
      var files = new CompactAnalyzedDataCreator(_workingDirectory).Create();
      var allLines = files.SelectMany(File.ReadAllLines);
      var distinct = allLines.Select(a => a.Substring(HandLength)).Distinct().OrderBy(x => x);
      return distinct.Select(a => Arrangement.MultipleFromString(a).ToList());
    }

    /// <summary>
    /// Creates all permutations of length 4 of the numbers 0 through alphabetSize - 1.
    /// </summary>
    /// <param name="alphabetSize">The number of characters in the language.</param>
    /// <returns>The language.</returns>
    private static IEnumerable<IList<int>> CreateBaseLanguage(int alphabetSize)
    {
      for (var a = 0; a < alphabetSize; ++a)
      {
        for (var b = a; b < alphabetSize; ++b)
        {
          for (var c = b; c < alphabetSize; ++c)
          {
            for (var d = c; d < alphabetSize; ++d)
            {
              yield return new[] { a, b, c, d };
            }
          }
        }
      }
    }

    private static IEnumerable<WordWithValue> CreateWords(IEnumerable<List<Arrangement>> arrangements)
    {
      var arrangementsList = arrangements.ToList();
      var alphabetSize = arrangementsList.Count;
      var language = CreateBaseLanguage(alphabetSize);
      var tilesInArrangements = arrangementsList.Select(a => a.Max(b => b.TotalValue)).ToList();

      //var max = alphabetSize * ((long)alphabetSize + 1) * ((long)alphabetSize + 2) * ((long)alphabetSize + 3) / 24;
      //long count = 0;
      //long current = 0;
      foreach (var word in language)
      {
        var sumOfTiles = word.Sum(c => tilesInArrangements[c]);
        if (sumOfTiles <= 14 && sumOfTiles >= 5)
        {
          var analyzer = new ArrangementAnalyzer();
          foreach (var character in word)
          {
            analyzer.AddSetOfArrangements(arrangementsList[character]);
          }
          var shanten = analyzer.CalculateShanten();
          if (shanten < 9)
          {
            yield return new WordWithValue(word, shanten);
          }
        }

        //count += 1;
        //if (current * (max / 100) < count)
        //{
        //  IncrementProgressBar();
        //  current += 1;
        //}
      }
    }
  }
}