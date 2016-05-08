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
using System.Diagnostics;

namespace Spines.Mahjong.Analysis.Combinations
{
  /// <summary>
  /// An (in-)complete Group; either a mentsu or a jantou.
  /// </summary>
  [DebuggerDisplay("ProtoGroup {Value}")]
  internal class ProtoGroup
  {
    /// <summary>
    /// A full jantou.
    /// </summary>
    public static readonly ProtoGroup Jantou2 = new ProtoGroup(2, new MentsuProtoGroupInserter(2, 2));

    /// <summary>
    /// A jantou missing one tile.
    /// </summary>
    public static readonly ProtoGroup Jantou1 = new ProtoGroup(1, new MentsuProtoGroupInserter(1, 2));

    /// <summary>
    /// A full koutsu.
    /// </summary>
    public static readonly ProtoGroup Koutsu3 = new ProtoGroup(3, new MentsuProtoGroupInserter(3, 3));

    /// <summary>
    /// A koutsu missing one tile.
    /// </summary>
    public static readonly ProtoGroup Koutsu2 = new ProtoGroup(2, new MentsuProtoGroupInserter(2, 3));

    /// <summary>
    /// A koutsu missing two tiles.
    /// </summary>
    public static readonly ProtoGroup Koutsu1 = new ProtoGroup(1, new MentsuProtoGroupInserter(1, 3));

    /// <summary>
    /// A full shuntsu.
    /// </summary>
    public static readonly ProtoGroup Shuntsu111 = new ProtoGroup(3, new ShuntsuProtoGroupInserter(1, 1, 1));

    /// <summary>
    /// A shuntsu missing the middle tile.
    /// </summary>
    public static readonly ProtoGroup Shuntsu101 = new ProtoGroup(2, new ShuntsuProtoGroupInserter(1, 0, 1));

    /// <summary>
    /// A shuntsu missing the left tile.
    /// </summary>
    public static readonly ProtoGroup Shuntsu011 = new ProtoGroup(2, new ShuntsuProtoGroupInserter(0, 1, 1));

    /// <summary>
    /// A shuntsu missing the right tile.
    /// </summary>
    public static readonly ProtoGroup Shuntsu110 = new ProtoGroup(2, new ShuntsuProtoGroupInserter(1, 1, 0));

    /// <summary>
    /// A shuntsu missing the left and right tiles.
    /// </summary>
    public static readonly ProtoGroup Shuntsu010 = new ProtoGroup(1, new ShuntsuProtoGroupInserter(0, 1, 0));

    /// <summary>
    /// A shuntsu missing the middle and right tiles.
    /// </summary>
    public static readonly ProtoGroup Shuntsu100 = new ProtoGroup(1, new ShuntsuProtoGroupInserter(1, 0, 0));

    /// <summary>
    /// A shuntsu missing the left and middle tiles.
    /// </summary>
    public static readonly ProtoGroup Shuntsu001 = new ProtoGroup(1, new ShuntsuProtoGroupInserter(0, 0, 1));

    /// <summary>
    /// Can this ProtoGroup be used in an arrangement?
    /// </summary>
    private readonly IProtoGroupInserter _protoGroupInserter;

    /// <summary>
    /// Creates a new instance of ProtoGroup.
    /// </summary>
    private ProtoGroup(int value, IProtoGroupInserter protoGroupInserter)
    {
      Value = value;
      _protoGroupInserter = protoGroupInserter;
    }

    /// <summary>
    /// The Value of the ProtoGroup.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Can this ProtoGroup be used in an arrangement?
    /// </summary>
    public bool CanInsert(IReadOnlyList<int> concealedTiles, IReadOnlyList<int> usedTiles, int offset)
    {
      return _protoGroupInserter.CanInsert(concealedTiles, usedTiles, offset);
    }

    public void Insert(IList<int> concealedTiles, IList<int> usedTiles, int offset)
    {
      _protoGroupInserter.Insert(concealedTiles, usedTiles, offset);
    }

    public void Remove(IList<int> concealedTiles, IList<int> usedTiles, int offset)
    {
      _protoGroupInserter.Remove(concealedTiles, usedTiles, offset);
    }
  }
}