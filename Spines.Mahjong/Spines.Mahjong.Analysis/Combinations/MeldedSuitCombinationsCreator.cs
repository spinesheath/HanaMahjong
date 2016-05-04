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

using System;
using System.Collections.Generic;
using System.Linq;
using Spines.Utility;

namespace Spines.Mahjong.Analysis.Combinations
{
  /// <summary>
  /// Creates possible combinations of tiles used in melds in one suit.
  /// </summary>
  public class MeldedSuitCombinationsCreator : CombinationCreatorBase
  {
    /// <summary>
    /// Creates all possible combinations of used tiles for a number of melds in a single suit.
    /// </summary>
    public IEnumerable<Combination> Create(int numberOfMelds)
    {
      Validate.NotNegative(numberOfMelds, nameof(numberOfMelds));
      Clear();
      return Create(numberOfMelds, 0);
    }

    /// <summary>
    /// Creates all possible combinations of used tiles for a number of melds in a single suit.
    /// </summary>
    private IEnumerable<Combination> Create(int remainingMelds, int currentIndex)
    {
      // All melds used, return the current used tiles.
      if (remainingMelds == 0)
      {
        return CreateCurrentCombination().Yield();
      }

      return Mentsu.All.SelectMany(m => Create(remainingMelds, currentIndex, m));
    }

    /// <summary>
    /// Can a meld be added to the current accumulator?
    /// </summary>
    private bool CanAddMeld(int index, Mentsu mentsu)
    {
      if (index > TypesInSuit - mentsu.Stride)
      {
        return false;
      }
      var max = TilesPerType - mentsu.Amount;
      return Accumulator.Skip(index).Take(mentsu.Stride).All(i => i <= max);
    }

    /// <summary>
    /// Creates all possible combinations of used tiles for a number of melds in a single suit by adding a specific meld.
    /// </summary>
    private IEnumerable<Combination> Create(int remainingMelds, int currentIndex, Mentsu mentsu)
    {
      var indices = Enumerable.Range(currentIndex, TypesInSuit - currentIndex);
      var freeIndices = indices.Where(i => CanAddMeld(i, mentsu));
      foreach (var index in freeIndices)
      {
        AddToAccumulator(index, mentsu.Stride, mentsu.Amount);
        foreach (var combination in Create(remainingMelds - 1, index))
        {
          yield return combination;
        }
        AddToAccumulator(index, mentsu.Stride, -mentsu.Amount);
      }
    }

    /// <summary>
    /// Adds an amount to multiple entries in the accumulator.
    /// </summary>
    /// <param name="index">The index of the first entry to adjust.</param>
    /// <param name="stride">The number of entries to adjust.</param>
    /// <param name="amount">The amount that is added to each entry.</param>
    private void AddToAccumulator(int index, int stride, int amount)
    {
      for (var i = 0; i < stride; ++i)
      {
        Accumulator[index + i] += amount;
      }
    }

    /// <summary>
    /// Calculates the conbined weight of all tiles.
    /// The weight of a combination balanced around the middle is 0.
    /// Tiles to the left have positive weight, tiles to the right have negative weight.
    /// </summary>
    protected int GetWeight()
    {
      return Enumerable.Range(0, TypesInSuit).Sum(GetWeight);
    }

    /// <summary>
    /// Calculates the weight of a single tile type and count.
    /// TileTypes to the left have positive weight, to the right have negative.
    /// </summary>
    private int GetWeight(int tileTypeIndex)
    {
      var tileCount = Accumulator[tileTypeIndex];
      const int centerIndex = (TypesInSuit - TypesInSuit % 1) / 2;
      var shift = Math.Abs(centerIndex - tileTypeIndex) * 2;
      var factor = Math.Sign(centerIndex - tileTypeIndex);
      return (1 << shift) * tileCount * factor;
    }
  }
}