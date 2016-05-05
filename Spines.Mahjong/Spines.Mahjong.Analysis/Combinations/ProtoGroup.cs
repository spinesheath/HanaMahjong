// Spines.Mahjong.Analysis.ProtoGroup.cs
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
  /// An (in-)complete Group; either a mentsu or a jantou.
  /// </summary>
  internal class ProtoGroup
  {
    /// <summary>
    /// A full jantou.
    /// </summary>
    public static ProtoGroup Jantou = new ProtoGroup(2, new MentsuInsertChecker(2, 2));

    /// <summary>
    /// A jantou missing one tile.
    /// </summary>
    public static ProtoGroup JantouMinusOne = new ProtoGroup(1, new MentsuInsertChecker(1, 2));

    /// <summary>
    /// A full koutsu.
    /// </summary>
    public static ProtoGroup Koutsu = new ProtoGroup(3, new MentsuInsertChecker(3, 3));

    /// <summary>
    /// A koutsu missing one tile.
    /// </summary>
    public static ProtoGroup KoutsuMinusOne = new ProtoGroup(2, new MentsuInsertChecker(2, 3));

    /// <summary>
    /// A koutsu missing two tiles.
    /// </summary>
    public static ProtoGroup KoutsuMinuesTwo = new ProtoGroup(1, new MentsuInsertChecker(1, 3));

    /// <summary>
    /// A full shuntsu.
    /// </summary>
    public static ProtoGroup Shuntsu = new ProtoGroup(3, new ShuntsuInsertChecker(1, 1, 1));

    /// <summary>
    /// A shuntsu missing the middle tile.
    /// </summary>
    public static ProtoGroup ShuntsuMinusMiddle = new ProtoGroup(2, new ShuntsuInsertChecker(1, 0, 1));

    /// <summary>
    /// A shuntsu missing the left tile.
    /// </summary>
    public static ProtoGroup ShuntsuMinusLeft = new ProtoGroup(2, new ShuntsuInsertChecker(0, 1, 1));

    /// <summary>
    /// A shuntsu missing the right tile.
    /// </summary>
    public static ProtoGroup ShuntsuMinusRight = new ProtoGroup(2, new ShuntsuInsertChecker(1, 1, 0));

    /// <summary>
    /// A shuntsu missing the left and right tiles.
    /// </summary>
    public static ProtoGroup ShuntsuOnlyMiddle = new ProtoGroup(1, new ShuntsuInsertChecker(0, 1, 0));

    /// <summary>
    /// A shuntsu missing the middle and right tiles.
    /// </summary>
    public static ProtoGroup ShuntsuOnlyLeft = new ProtoGroup(1, new ShuntsuInsertChecker(1, 0, 0));

    /// <summary>
    /// A shuntsu missing the left and middle tiles.
    /// </summary>
    public static ProtoGroup ShuntsuOnlyRight = new ProtoGroup(1, new ShuntsuInsertChecker(0, 0, 1));

    /// <summary>
    /// Can this ProtoGroup be used in an arrangement?
    /// </summary>
    private readonly IInsertChecker _insertChecker;

    /// <summary>
    /// Creates a new instance of ProtoGroup.
    /// </summary>
    private ProtoGroup(int value, IInsertChecker insertChecker)
    {
      Value = value;
      _insertChecker = insertChecker;
    }

    /// <summary>
    /// The Value of the ProtoGroup.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Can this ProtoGroup be used in an arrangement?
    /// </summary>
    public bool CanInsertProtoGroup(IReadOnlyList<int> concealedTiles, IReadOnlyList<int> usedTiles, int offset)
    {
      return _insertChecker.CanInsert(concealedTiles, usedTiles, offset);
    }
  }
}