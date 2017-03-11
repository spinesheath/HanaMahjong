// Spines.Tools.AnalyzerBuilder.RawAnalyzedHonorDataCreator.cs
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

using Spines.Tools.AnalyzerBuilder.Combinations;

namespace Spines.Tools.AnalyzerBuilder.Precalculation
{
  /// <summary>
  /// Creates honor data with raw arrangement data.
  /// </summary>
  internal class RawAnalyzedHonorDataCreator : RawAnalyzedDataCreator
  {
    /// <summary>
    /// Creates the file name for the given tile count.
    /// </summary>
    /// <param name="count">The tile count.</param>
    /// <returns>A file name.</returns>
    protected override string GetFileName(int count)
    {
      return $"honors_{count}.txt";
    }

    /// <summary>
    /// Returns a TileGroupAnalyzer for the the given data.
    /// </summary>
    /// <param name="concealed">The concealed tiles to analyze.</param>
    /// <param name="melded">The melded tiles to analyze.</param>
    /// <param name="meldCount">The number of melds.</param>
    /// <returns>An instance of TileGroupAnalyzer.</returns>
    protected override TileGroupAnalyzer GetTileGroupAnalyzer(Combination concealed, Combination melded, int meldCount)
    {
      return TileGroupAnalyzer.ForHonors(concealed, melded, meldCount);
    }

    /// <summary>
    /// Returns a ConcealedCombinationCreator.
    /// </summary>
    /// <returns>An instance of ConcealedCombinationCreator.</returns>
    protected override ConcealedCombinationCreator GetConcealedCombinationCreator()
    {
      return ConcealedCombinationCreator.ForHonors();
    }

    /// <summary>
    /// Returns a MeldedCombinationsCreator.
    /// </summary>
    /// <returns>An instance of MeldedCombinationsCreator.</returns>
    protected override MeldedCombinationsCreator GetMeldedCombinationsCreator()
    {
      return MeldedCombinationsCreator.ForHonors();
    }
  }
}