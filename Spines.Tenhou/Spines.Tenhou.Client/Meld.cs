// Spines.Tenhou.Client.Meld.cs
// 
// Copyright (C) 2015  Johannes Heckl
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
using System.Globalization;
using System.Linq;

namespace Spines.Tenhou.Client
{
  internal class Meld
  {
    private static readonly int[,] KoutsuTileOffsets = {{1, 2, 3}, {0, 2, 3}, {0, 1, 3}, {0, 1, 2}};
    private readonly int _meldCode;

    public Meld(string meldCodeString, int ownerId)
    {
      _meldCode = Convert.ToInt32(meldCodeString, CultureInfo.InvariantCulture);
      Type = GetMeldType();
      Tiles = Decode().ToArray();
      FromPlayerId = GetFromPlayerId(ownerId);
      OwnerId = ownerId;
    }

    public int OwnerId { get; private set; }
    public int FromPlayerId { get; private set; }
    public MeldTile[] Tiles { get; private set; }
    public MeldType Type { get; private set; }

    private static int IntFromBits(int value, int numberOfBits, int leftShift)
    {
      var mask = (1 << numberOfBits) - 1;
      return (value >> leftShift) & mask;
    }

    private IEnumerable<MeldTile> Decode()
    {
      return GetTiles().Select((tile, index) => new MeldTile(tile, GetMeldTileType(tile, index)));
    }

    private int GetBaseIndex()
    {
      if (Type == MeldType.Shuntsu)
      {
        var t = IntFromBits(_meldCode, 6, 10) / 3;
        return ((t / 7) * 9 + t % 7) * 4;
      }
      if (Type == MeldType.AddedKan || Type == MeldType.Koutsu)
      {
        return IntFromBits(_meldCode, 7, 9) / 3 * 4;
      }
      return IntFromBits(_meldCode, 8, 8) & ~3;
    }

    /// <summary>
    /// Extracts the id of the called tile from the meld code.
    /// </summary>
    private Tile GetCalledTile()
    {
      var baseIndex = GetBaseIndex();
      if (Type == MeldType.Shuntsu)
      {
        return new Tile(baseIndex + IntFromBits(_meldCode, 6, 10) % 3);
      }
      if (Type == MeldType.AddedKan || Type == MeldType.Koutsu)
      {
        return new Tile(baseIndex + KoutsuTileOffsets[GetUnusedOrAddedTileIndex(), IntFromBits(_meldCode, 7, 9) % 3]);
      }
      return new Tile(baseIndex + IntFromBits(_meldCode, 8, 8) % 4);
    }

    /// <summary>
    /// Gets the id of the player that discarded the called tile, or id the owner of the meld for a closed kan.
    /// Not sure about added kan.
    /// </summary>
    /// <param name="ownerId">The id of the owner of the meld.</param>
    private int GetFromPlayerId(int ownerId)
    {
      return (ownerId + IntFromBits(_meldCode, 2, 0)) % 4;
    }

    private MeldTileType GetMeldTileType(Tile tile, int index)
    {
      if (Type == MeldType.ClosedKan && (index == 0 || index == 3))
      {
        return MeldTileType.FaceDown;
      }
      if (tile == GetCalledTile())
      {
        return MeldTileType.Flipped;
      }
      if (Type == MeldType.AddedKan && tile == GetAddedTile())
      {
        return MeldTileType.Added;
      }
      return MeldTileType.Normal;
    }

    private MeldType GetMeldType()
    {
      if ((_meldCode & 1 << 2) != 0)
      {
        return MeldType.Shuntsu;
      }
      if ((_meldCode & 1 << 3) != 0)
      {
        return MeldType.Koutsu;
      }
      if ((_meldCode & 1 << 4) != 0)
      {
        return MeldType.AddedKan;
      }
      if ((_meldCode & 1 << 5) != 0)
      {
        throw new ClientException("Nuki not supported");
      }
      if ((_meldCode & 3) != 0)
      {
        return MeldType.CalledKan;
      }
      return MeldType.ClosedKan;
    }

    private IEnumerable<Tile> GetTiles()
    {
      var baseIndex = GetBaseIndex();
      var tileCount = IsKan() ? 4 : 3;
      for (var i = 0; i < tileCount; ++i)
      {
        switch (Type)
        {
          case MeldType.Shuntsu:
            yield return new Tile(baseIndex + 4 * i + IntFromBits(_meldCode, 2, 1 + 2 * i));
            break;
          case MeldType.Koutsu:
            yield return new Tile(baseIndex + KoutsuTileOffsets[GetUnusedOrAddedTileIndex(), i]);
            break;
          default:
            yield return new Tile(baseIndex + i);
            break;
        }
      }
    }

    /// <summary>
    /// Calculates the index of the tile among all the tiles of a tile type that is not used by a koutsu or was added to a koutsu.
    /// </summary>
    private int GetUnusedOrAddedTileIndex()
    {
      return IntFromBits(_meldCode, 2, 5);
    }

    /// <summary>
    /// Gets the tile that was added to a koutsu.
    /// </summary>
    private Tile GetAddedTile()
    {
      return new Tile(GetBaseIndex() + GetUnusedOrAddedTileIndex());
    }

    private bool IsKan()
    {
      return Type != MeldType.Shuntsu && Type != MeldType.Koutsu;
    }

    // Apply a bitmask with all ones in a single block and then treat the selected bits as an integer
  }

  internal enum MeldType
  {
    Shuntsu,
    AddedKan,
    Koutsu,
    CalledKan,
    ClosedKan
  }

  internal enum MeldTileType
  {
    Normal,
    FaceDown,
    Flipped,
    Added
  }

  internal class MeldTile
  {
    public MeldTile(Tile tile, MeldTileType type)
    {
      Tile = tile;
      Type = type;
    }

    public Tile Tile { get; private set; }
    public MeldTileType Type { get; private set; }
  }
}