// Spines.Mahjong.Analysis.SuitCombinationCreator.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using Spines.Utility;

namespace Spines.Mahjong.Analysis.Combinations
{
  /// <summary>
  /// Creates possible combinations of tiles in one suit.
  /// </summary>
  internal class SuitCombinationCreator
  {
    /// <summary>
    /// Creates all possible semantically unique concealed combinations for a suit and a given number of tiles.
    /// </summary>
    public IEnumerable<Combination> CreateConcealedCombinations(int numberOfTiles)
    {
      Validate.NotNegative(numberOfTiles, nameof(numberOfTiles));
      return CreateConcealedCombinations(new int[9], numberOfTiles, 9);
    }

    /// <summary>
    /// Calculates the weight of a set of tiles in one suit.
    /// The weight of a combination balanced around the middle is 0.
    /// More tiles to the left has positive weight, more to the right has negative weight.
    /// </summary>
    private int GetWeight(IList<int> source)
    {
      return Enumerable.Range(0, 9).Sum(i => GetWeight(i, source[i]));
    }

    /// <summary>
    /// Calculates the weight of a single tile type and count.
    /// TileTypes to the left have positive weight, to the right have negative.
    /// </summary>
    private int GetWeight(int tileTypeIndex, int tileCount)
    {
      var shift = Math.Abs(4 - tileTypeIndex) * 2;
      var factor = Math.Sign(4 - tileTypeIndex);
      return (1 << shift) * tileCount * factor;
    }

    /// <summary>
    /// Recursively creates possible concealed combinations in one suit.
    /// </summary>
    private IEnumerable<Combination> CreateConcealedCombinations(IList<int> accumulator, int remainingTiles,
      int remainingTypes)
    {
      // If all types have been tried we are done.
      if (remainingTypes == 0)
      {
        var weight = GetWeight(accumulator);
        if (remainingTiles == 0 && weight >= 0)
        {
          yield return new Combination(accumulator.ToList());
        }
      }
      else
      {
        // The maximum amount of tiles that can be used for the current type.
        var max = Math.Min(remainingTiles, 4);
        // Add 0 to max tiles of the current type and accumulate results.
        for (var i = max; i >= 0; --i)
        {
          accumulator[9 - remainingTypes] = i;
          foreach (var gd in CreateConcealedCombinations(accumulator, remainingTiles - i, remainingTypes - 1))
          {
            yield return gd;
          }
        }
      }
    }
  }
}