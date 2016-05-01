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
using Spines.Utility;

namespace Spines.Mahjong.Analysis.Combinations
{
  /// <summary>
  /// Creates possible combinations of tiles in one suit.
  /// </summary>
  internal class ConcealedSuitCombinationCreator : CombinationCreatorBase
  {
    /// <summary>
    /// Creates all possible semantically unique concealed combinations for a suit and a given number of tiles.
    /// </summary>
    public IEnumerable<Combination> CreateCombinations(int numberOfTiles)
    {
      Validate.NotNegative(numberOfTiles, nameof(numberOfTiles));
      Clear();
      return CreateCombinations(numberOfTiles, TypesInSuit);
    }

    /// <summary>
    /// Recursively creates possible concealed combinations in one suit.
    /// </summary>
    private IEnumerable<Combination> CreateCombinations(int remainingTiles, int remainingTypes)
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
        // The maximum amount of tiles that can be used for the current type.
        var max = Math.Min(remainingTiles, TilesPerType);
        // Add 0 to max tiles of the current type and accumulate results.
        for (var i = max; i >= 0; --i)
        {
          Accumulator[TypesInSuit - remainingTypes] = i;
          foreach (var gd in CreateCombinations(remainingTiles - i, remainingTypes - 1))
          {
            yield return gd;
          }
        }
      }
    }
  }
}