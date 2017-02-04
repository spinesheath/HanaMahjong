// Spines.Mahjong.Analysis.ShantenCounter.cs
// 
// Copyright (C) 2016  Johannes Heckl
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

namespace Spines.Mahjong.Analysis
{
  /// <summary>
  /// Calculates the shanten count of a hand.
  /// </summary>
  public class ShantenCounter
  {
    /// <summary>
    /// Calculates the shanten count of a hand.
    /// </summary>
    public int Count(IEnumerable<int> concealedTiles, IEnumerable<int> meldedTiles, int meldCount)
    {
      var concealedCounts = new int[34];
      foreach (var tile in concealedTiles)
      {
        concealedCounts[tile / 4] += 1;
      }
      var meldedCounts = new int[34];
      foreach (var tile in meldedTiles)
      {
        meldedCounts[tile / 4] += 1;
      }
      var c = new ShantenClassifiers();


      return 0;
    }
  }
}