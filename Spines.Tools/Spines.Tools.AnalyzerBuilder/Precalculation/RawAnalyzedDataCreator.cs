// Spines.Tools.AnalyzerBuilder.RawAnalyzedDataCreator.cs
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
using Spines.Tools.AnalyzerBuilder.Combinations;

namespace Spines.Tools.AnalyzerBuilder.Precalculation
{
  /// <summary>
  /// Creates the files with all the possible weighted suit and honor hands, with raw arrangement data.
  /// </summary>
  internal abstract class RawAnalyzedDataCreator
  {
    /// <summary>
    /// Creates an instance for analyzing honors.
    /// </summary>
    /// <returns>An instance of RawAnalyzedDataCreator for analyzing honors.</returns>
    public static RawAnalyzedDataCreator ForHonors()
    {
      return new RawAnalyzedHonorDataCreator();
    }

    /// <summary>
    /// Creates an instance for analyzing suits.
    /// </summary>
    /// <returns>An instance of RawAnalyzedDataCreator for analyzing suits.</returns>
    public static RawAnalyzedDataCreator ForSuits()
    {
      return new RawAnalyzedSuitDataCreator();
    }

    /// <summary>
    /// Creates the files with all the possible weighted suit and honor hands, with raw arrangement data.
    /// </summary>
    /// <param name="workingDirectory">The directory where the files are placed.</param>
    public IEnumerable<string> Create(string workingDirectory)
    {
      foreach (var count in Enumerable.Range(0, 15))
      {
        var fileName = Path.Combine(workingDirectory, GetFileName(count));
        if (!File.Exists(fileName))
        {
          var lines = Analyze(count);
          File.WriteAllLines(fileName, lines);
        }
        yield return fileName;
      }
    }

    /// <summary>
    /// Creates the file name for the given tile count.
    /// </summary>
    /// <param name="count">The tile count.</param>
    /// <returns>A file name.</returns>
    protected abstract string GetFileName(int count);

    /// <summary>
    /// Creates the analyzed data for a given number of concealed tiles.
    /// </summary>
    /// <param name="tileCount">The number of concealed tiles to use for the analysis.</param>
    /// <returns>A string containing the hand and the arrangements for each hand.</returns>
    private IEnumerable<string> Analyze(int tileCount)
    {
      var maxMelds = (14 - tileCount) / 3;
      foreach (var meldCount in Enumerable.Range(0, maxMelds + 1))
      {
        var meldeds = GetMeldedCombinationsCreator().Create(meldCount);
        foreach (var melded in meldeds)
        {
          var concealeds = GetConcealedCombinationCreator().Create(tileCount, melded);
          foreach (var concealed in concealeds)
          {
            var arrangements = GetTileGroupAnalyzer(concealed, melded, meldCount).Analyze();
            var meldedCounts = string.Join("", melded.Counts).PadRight(9, '.');
            var concealedCounts = string.Join("", concealed.Counts).PadRight(9, '.');
            var arrangementsString = string.Join("", arrangements);
            yield return $"{meldCount}{meldedCounts}{concealedCounts}{arrangementsString}";
          }
        }
      }
    }

    /// <summary>
    /// Returns a TileGroupAnalyzer for the the given data.
    /// </summary>
    /// <param name="concealed">The concealed tiles to analyze.</param>
    /// <param name="melded">The melded tiles to analyze.</param>
    /// <param name="meldCount">The number of melds.</param>
    /// <returns>An instance of TileGroupAnalyzer.</returns>
    protected abstract TileGroupAnalyzer GetTileGroupAnalyzer(Combination concealed, Combination melded, int meldCount);

    /// <summary>
    /// Returns a ConcealedCombinationCreator.
    /// </summary>
    /// <returns>An instance of ConcealedCombinationCreator.</returns>
    protected abstract ConcealedCombinationCreator GetConcealedCombinationCreator();

    /// <summary>
    /// Returns a MeldedCombinationsCreator.
    /// </summary>
    /// <returns>An instance of MeldedCombinationsCreator.</returns>
    protected abstract MeldedCombinationsCreator GetMeldedCombinationsCreator();
  }
}