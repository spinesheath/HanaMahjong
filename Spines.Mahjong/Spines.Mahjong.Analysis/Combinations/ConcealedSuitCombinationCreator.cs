// Spines.Mahjong.Analysis.ConcealedSuitCombinationCreator.cs
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
  internal class ConcealedSuitCombinationCreator : CombinationCreatorBase
  {
    private int[] _tilesUsedInMelds;

    /// <summary>
    /// Creates all possible semantically unique concealed combinations for a given number of tiles.
    /// </summary>
    public IEnumerable<Combination> Create(int numberOfTiles)
    {
      var noMeldedTiles = new Combination(new int[TypesInSuit]);
      return Create(numberOfTiles, noMeldedTiles);
    }

    /// <summary>
    /// Creates all possible semantically unique concealed combinations for a given number of tiles and a set of tiles already used in melds.
    /// </summary>
    /// <param name="numberOfTiles">The number of tiles in the concealed part of the hand.</param>
    /// <param name="meldedTiles">The tiles used in the melded part of the hand.</param>
    public IEnumerable<Combination> Create(int numberOfTiles, Combination meldedTiles)
    {
      Validate.NotNegative(numberOfTiles, nameof(numberOfTiles));
      Clear();
      _tilesUsedInMelds = meldedTiles.Counts.ToArray();
      return Create(numberOfTiles, TypesInSuit);
    } 

    /// <summary>
    /// Recursively creates possible concealed combinations in one suit.
    /// </summary>
    private IEnumerable<Combination> Create(int remainingTiles, int remainingTypes)
    {
      // If all types have been tried we are done.
      if (remainingTypes == 0)
      {
        var weight = GetWeight();
        if (remainingTiles == 0 && weight >= 0)
        {
          yield return CreateCurrentCombination();
        }
      }
      else
      {
        var index = TypesInSuit - remainingTypes;
        var freeTiles = TilesPerType - _tilesUsedInMelds[index];
        // The maximum amount of tiles that can be used for the current type.
        var max = Math.Min(remainingTiles, freeTiles);
        // Add 0 to max tiles of the current type and accumulate results.
        for (var i = 0; i <= max; ++i)
        {
          Accumulator[index] = i;
          foreach (var gd in Create(remainingTiles - i, remainingTypes - 1))
          {
            yield return gd;
          }
        }
      }
    }

    /// <summary>
    /// Calculates the conbined weight of all tiles.
    /// The weight of a combination balanced around the middle is 0.
    /// Tiles to the left have positive weight, tiles to the right have negative weight.
    /// </summary>
    private int GetWeight()
    {
      return Enumerable.Range(0, TypesInSuit).Sum(GetWeight);
    }

    /// <summary>
    /// Calculates the weight of a single tile type and count.
    /// TileTypes to the left have positive weight, to the right have negative.
    /// </summary>
    private int GetWeight(int tileTypeIndex)
    {
      var tileCount = Accumulator[tileTypeIndex] + _tilesUsedInMelds[tileTypeIndex];
      const int centerIndex = (TypesInSuit - TypesInSuit % 1) / 2;
      var shift = Math.Abs(centerIndex - tileTypeIndex) * 2;
      var factor = Math.Sign(centerIndex - tileTypeIndex);
      return (1 << shift) * tileCount * factor;
    }
  }
}