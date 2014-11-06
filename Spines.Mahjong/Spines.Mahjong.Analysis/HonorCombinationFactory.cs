// Spines.Mahjong.Analysis.HonorCombinationFactory.cs
// 
// Copyright (C) 2014  Johannes Heckl
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

namespace Spines.Mahjong.Analysis
{
  /// <summary>
  /// Counts the number of distinct tile combinations for honor tiles.
  /// </summary>
  public class HonorCombinationFactory
  {
    /// <summary>
    /// Initializes the instance for a given number of tiles.
    /// </summary>
    public HonorCombinationFactory(int numberOfTiles)
    {
      if (numberOfTiles < 0)
      {
        throw new AnalysisException("NumberOfTiles can not be negative.");
      }
      Combinations = Enumerable.Repeat(new Combination(), numberOfTiles);
    }

    /// <summary>
    /// All distinct combinations for the given configurations.
    /// </summary>
    public IEnumerable<Combination> Combinations { get; private set; }
  }
}