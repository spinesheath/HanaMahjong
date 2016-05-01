// Spines.Mahjong.Analysis.MeldedSuitCombinationsCreator.cs
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
using System.Linq;
using Spines.Utility;

namespace Spines.Mahjong.Analysis.Combinations
{
  /// <summary>
  /// Creates possible combinations of tiles used in melds in one suit.
  /// </summary>
  internal class MeldedSuitCombinationsCreator
  {
    private const int TilesPerType = 4;
    private const int TypesInSuit = 9;
    private int[] _accumulator;

    /// <summary>
    /// Creates all possible combinations of used tiles for a number of melds in a single suit.
    /// </summary>
    public IEnumerable<Combination> CreateMeldedCombinations(int numberOfMelds)
    {
      Validate.NotNegative(numberOfMelds, nameof(numberOfMelds));
      _accumulator = new int[TypesInSuit];
      return CreateMeldedCombinations(numberOfMelds, 0);
    }

    /// <summary>
    /// Creates all possible combinations of used tiles for a number of melds in a single suit.
    /// </summary>
    private IEnumerable<Combination> CreateMeldedCombinations(int remainingMelds, int currentIndex)
    {
      // All melds used, return the current used tiles.
      if (remainingMelds == 0)
      {
        return new Combination(_accumulator.ToList()).Yield();
      }

      return
        MeldShape.MeldShapes.SelectMany(m => CreateMeldedCombinations(remainingMelds, currentIndex, m.Stride, m.Amount));
    }

    /// <summary>
    /// Can a meld be added to the current accumulator?
    /// </summary>
    /// <param name="index">The index at which the meld is to be added.</param>
    /// <param name="stride">The stride of the meld; 1 for koutsu/kantsu, 3 for shuntsu.</param>
    /// <param name="amount">The number of tiles per entry, 3 for koutsu, 4 for kantsu, 1 for shuntsu.</param>
    /// <returns>True if a meld can be added, false otherwise.</returns>
    private bool CanAddMeld(int index, int stride, int amount)
    {
      if (index > TypesInSuit - stride)
      {
        return false;
      }
      var max = TilesPerType - amount;
      return _accumulator.Skip(index).Take(stride).All(i => i <= max);
    }

    /// <summary>
    /// Creates all possible combinations of used tiles for a number of melds in a single suit by adding a specific meld.
    /// </summary>
    private IEnumerable<Combination> CreateMeldedCombinations(int remainingMelds, int currentIndex, int stride,
      int amount)
    {
      var indices = Enumerable.Range(currentIndex, TypesInSuit - currentIndex);
      var freeIndices = indices.Where(i => CanAddMeld(i, stride, amount));
      foreach (var index in freeIndices)
      {
        AdjustAccumulator(index, stride, amount);
        foreach (var combination in CreateMeldedCombinations(remainingMelds - 1, index))
        {
          yield return combination;
        }
        AdjustAccumulator(index, stride, -amount);
      }
    }

    /// <summary>
    /// Adds an amount to multiple entries in the accumulator.
    /// </summary>
    /// <param name="index">The index of the first entry to adjust.</param>
    /// <param name="stride">The number of entries to adjust.</param>
    /// <param name="amount">The amount that is added to each entry.</param>
    private void AdjustAccumulator(int index, int stride, int amount)
    {
      for (var i = 0; i < stride; ++i)
      {
        _accumulator[index + i] += amount;
      }
    }
  }
}