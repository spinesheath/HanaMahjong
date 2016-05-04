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
using System.Linq;

namespace Spines.Mahjong.Analysis.Combinations
{
  /// <summary>
  /// Base class for combination creators.
  /// </summary>
  public class CombinationCreatorBase
  {
    /// <summary>
    /// There are 4 tiles of each type.
    /// </summary>
    protected const int TilesPerType = 4;

    /// <summary>
    /// A suit has 9 tile types.
    /// </summary>
    protected const int TypesInSuit = 9;

    /// <summary>
    /// Represents the number of tiles in the suit.
    /// </summary>
    protected int[] Accumulator;

    /// <summary>
    /// Resets the accumulator.
    /// </summary>
    protected void Clear()
    {
      Accumulator = new int[TypesInSuit];
    }

    /// <summary>
    /// Creates a new combination from the current state of the accumulator.
    /// </summary>
    protected Combination CreateCurrentCombination()
    {
      return new Combination(Accumulator.ToList());
    }
  }
}