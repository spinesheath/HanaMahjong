// Spines.Mahjong.Analysis.MeldShape.cs
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

namespace Spines.Mahjong.Analysis.Combinations
{
  /// <summary>
  /// Defines the shape of a meld.
  /// </summary>
  internal class MeldShape
  {
    /// <summary>
    /// 3 consecutive tiles.
    /// </summary>
    public static readonly MeldShape Chii = new MeldShape(3, 1);

    /// <summary>
    /// 3 tiles of one type.
    /// </summary>
    public static readonly MeldShape Pon = new MeldShape(1, 3);

    /// <summary>
    /// 4 tiles of one type.
    /// </summary>
    public static readonly MeldShape Kan = new MeldShape(1, 4);

    /// <summary>
    /// Creates an instance of MeldShape.
    /// </summary>
    private MeldShape(int stride, int amount)
    {
      Stride = stride;
      Amount = amount;
    }

    /// <summary>
    /// The number of consecutive tile types in the meld.
    /// </summary>
    public int Stride { get; }

    /// <summary>
    /// The number of tiles per tile type in the meld.
    /// </summary>
    public int Amount { get; }

    public static IEnumerable<MeldShape> MeldShapes
    {
      get
      {
        yield return Chii;
        yield return Pon;
        yield return Kan;
      }
    }
  }
}