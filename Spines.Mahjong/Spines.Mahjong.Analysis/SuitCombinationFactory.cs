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
    /// Creates all possible combinations of used tiles for a number of melds in a single suit.
    /// </summary>
    public static IEnumerable<Combination> CreateMeldedCombinations(int numberOfMelds)
    {
      Validate.NotNegative(numberOfMelds, nameof(numberOfMelds));
      return CreateMeldedCombinations(new int[9], numberOfMelds, 0);
    }

    /// <summary>
    /// Creates all possible combinations of used tiles for a number of melds in a single suit.
    /// </summary>
    private static IEnumerable<Combination> CreateMeldedCombinations(IList<int> accumulator, int remainingMelds, int currentIndex)
    {
      // All melds used, return the current used tiles.
      if (remainingMelds == 0)
      {
        yield return new Combination(accumulator.ToList());
      }
      else
      {
        foreach (var combination in CreateMeldedCombinations(accumulator, remainingMelds, currentIndex, 1, 3))
        {
          yield return combination;
        }

        foreach (var combination in CreateMeldedCombinations(accumulator, remainingMelds, currentIndex, 1, 4))
        {
          yield return combination;
        }

        foreach (var combination in CreateMeldedCombinations(accumulator, remainingMelds, currentIndex, 3, 1))
        {
          yield return combination;
        }
      }
    }

    /// <summary>
    /// Can a meld be added to the current accumulator?
    /// </summary>
    /// <param name="accumulator">The accumulator to check against.</param>
    /// <param name="index">The index at which the meld is to be added.</param>
    /// <param name="stride">The stride of the meld; 1 for koutsu/kantsu, 3 for shuntsu.</param>
    /// <param name="amount">The number of tiles per entry, 3 for koutsu, 4 for kantsu, 1 for shuntsu.</param>
    /// <returns>True if a meld can be added, false otherwise.</returns>
    private static bool CanAddMeld(IEnumerable<int> accumulator, int index, int stride, int amount)
    {
      if (index > 9 - stride)
        return false;
      var max = 4 - amount;
      return accumulator.Skip(index).Take(stride).All(i => i <= max);
    }

    /// <summary>
    /// Creates all possible combinations of used tiles for a number of melds in a single suit by adding a specific meld.
    /// </summary>
    private static IEnumerable<Combination> CreateMeldedCombinations(IList<int> accumulator, int remainingMelds,
      int currentIndex, int stride, int amount)
    {
      var indices = Enumerable.Range(currentIndex, 9 - currentIndex);
      var freeIndices = indices.Where(i => CanAddMeld(accumulator, i, stride, amount));
      foreach (var index in freeIndices)
      {
        AdjustAccumulator(accumulator, index, stride, amount);
        foreach (var combination in CreateMeldedCombinations(accumulator, remainingMelds - 1, index))
        {
          yield return combination;
        }
        AdjustAccumulator(accumulator, index, stride, -amount);
      }
    }

    /// <summary>
    /// Adds an amount to multiple entries in the accumulator.
    /// </summary>
    /// <param name="accumulator">The modified accumulator.</param>
    /// <param name="index">The index of the first entry to adjust.</param>
    /// <param name="stride">The number of entries to adjust.</param>
    /// <param name="amount">The amount that is added to each entry.</param>
    private static void AdjustAccumulator(IList<int> accumulator, int index, int stride, int amount)
    {
      for (var i = 0; i < stride; ++i)
      {
        accumulator[index + i] += amount;
      }
    }

    /// <summary>
    /// Creates all possible semantically unique combinations for a suit and a given number of tiles.
    /// </summary>
    public static IEnumerable<Combination> CreateCombinations(int numberOfTiles)
    {
      Validate.NotNegative(numberOfTiles, nameof(numberOfTiles));
      return CreateCombinations(new int[9], numberOfTiles, 9);
    }

    /// <summary>
    /// Calculates the weight of a set of tiles in one suit.
    /// The weight of a combination balanced around the middle is 0.
    /// More tiles to the left has positive weight, more to the right has negative weight.
    /// </summary>
    private static int GetWeight(IList<int> source)
    {
      return Enumerable.Range(0, 9).Sum(i => GetWeight(i, source[i]));
    }

    /// <summary>
    /// Calculates the weight of a single tile type and count.
    /// TileTypes to the left have positive weight, to the right have negative.
    /// </summary>
    private static int GetWeight(int tileTypeIndex, int tileCount)
    {
      var shift = Math.Abs(4 - tileTypeIndex) * 2;
      var factor = Math.Sign(4 - tileTypeIndex);
      return (1 << shift) * tileCount * factor;
    }

    /// <summary>
    /// Recursively creates possible combinations in one suit.
    /// </summary>
    private static IEnumerable<Combination> CreateCombinations(IList<int> accumulator, int remainingTiles,
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
          foreach (var gd in CreateCombinations(accumulator, remainingTiles - i, remainingTypes - 1))
          {
            yield return gd;
          }
        }
      }
    }
  }
}