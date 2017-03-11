// Spines.Mahjong.Analysis.CombinationCreatorBase.cs
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

namespace Spines.Tools.AnalyzerBuilder.Combinations
{
  /// <summary>
  /// Base class for combination creators.
  /// </summary>
  public abstract class CombinationCreatorBase
  {
    /// <summary>
    /// Creates a new instance of CombinationCreatorBase.
    /// </summary>
    protected CombinationCreatorBase(int typesInSuit)
    {
      TypesInSuit = typesInSuit;
    }

    /// <summary>
    /// There are 4 tiles of each type.
    /// </summary>
    protected const int TilesPerType = 4;

    /// <summary>
    /// 9 for a suit or 7 for honors.
    /// </summary>
    protected int TypesInSuit { get; }

    /// <summary>
    /// Represents the number of tiles in the suit.
    /// </summary>
    protected IList<int> Accumulator { get; private set; }

    /// <summary>
    /// Tiles that are used in melds. 
    /// </summary>
    protected IList<int> TilesInExternalMelds { get; private set; }

    /// <summary>
    /// Resets the accumulator.
    /// </summary>
    protected void Clear()
    {
      Accumulator = new int[TypesInSuit];
      TilesInExternalMelds = new int[TypesInSuit];
    }

    /// <summary>
    /// Creates a new combination from the current state of the accumulator.
    /// </summary>
    protected Combination CreateCurrentCombination()
    {
      return new Combination(Accumulator.ToList());
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
      var tileCount = Accumulator[tileTypeIndex] + TilesInExternalMelds[tileTypeIndex];
      var centerIndex = (TypesInSuit - TypesInSuit % 1) / 2;
      var shift = Math.Abs(centerIndex - tileTypeIndex) * 2;
      var factor = Math.Sign(centerIndex - tileTypeIndex);
      return (1 << shift) * tileCount * factor;
    }
  }
}