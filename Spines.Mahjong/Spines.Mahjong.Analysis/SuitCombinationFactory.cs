// Spines.Mahjong.Analysis.SuitCombinationFactory.cs
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

namespace Spines.Mahjong.Analysis
{
  /// <summary>
  /// Creates possible combinations of tiles in one suit.
  /// </summary>
  internal static class SuitCombinationFactory
  {
    /// <summary>
    /// Creates all possible semantically unique combinations for a suit and a given number of tiles.
    /// </summary>
    public static IEnumerable<Combination> CreateCombinations(int numberOfTiles)
    {
      Validate.NotNegative(numberOfTiles, nameof(numberOfTiles));

      const int tileTypes = 9;
      var accumulator = new int[tileTypes];
      return CreateCombinations(accumulator, numberOfTiles, tileTypes);
    }

    /// <summary>
    /// Calculates the weight of a set of tiles in one suit.
    /// </summary>
    private static int GetWeight(IList<int> source)
    {
      var sum = 0;
      for (var i = 0; i < 9; ++i)
      {
        var shift = Math.Abs(4 - i) * 2;
        var factor = Math.Sign(4 - i);
        sum += (1 << shift) * source[i] * factor;
      }
      return sum;
    }

    /// <summary>
    /// Recursively creates possible combinations in one suit.
    /// </summary>
    private static IEnumerable<Combination> CreateCombinations(IList<int> accumulator, int remainingTiles, int remainingTypes)
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
          foreach (var gd in CreateCombinations(accumulator, remainingTiles - i, remainingTypes - 1))
          {
            yield return gd;
          }
        }
      }
    }
  }
}