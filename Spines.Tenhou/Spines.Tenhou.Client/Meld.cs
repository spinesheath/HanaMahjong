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
    /// <summary>
    /// Creates a meld from a meld code.
    /// </summary>
    /// <param name="meldCodeString">The meld code that specifies the meld.</param>
    /// <param name="ownerId">The owner of the meld.</param>
    public Meld(string meldCodeString, int ownerId)
    {
      var meldCode = Convert.ToInt32(meldCodeString, CultureInfo.InvariantCulture);
      Type = GetMeldType(meldCode);
      Tiles = GetTiles(meldCode).Select(tile => new MeldTile(tile, GetMeldTileType(meldCode, tile))).ToArray();
      FromPlayerId = GetFromPlayerId(meldCode, ownerId);
      OwnerId = ownerId;
    }

    /// <summary>
    /// The id of the owner of the meld.
    /// </summary>
    public int OwnerId { get; private set; }

    /// <summary>
    /// The id of the player that discarded the called tile, or the owner in case of a closed kan. Not sure about added kan.
    /// </summary>
    public int FromPlayerId { get; private set; }

    /// <summary>
    /// The tiles in the meld.
    /// </summary>
    public MeldTile[] Tiles { get; private set; }

    /// <summary>
    /// The type of the meld.
    /// </summary>
    public MeldType Type { get; private set; }

    /// <summary>
    /// Calculates the index of the tile among all the tiles of a tile type that is not used by a koutsu or was added to a
    /// koutsu.
    /// </summary>
    private static int GetUnusedOrAddedTileIndex(int meldCode)
    {
      return IntFromBits(meldCode, 2, 5);
    }

    /// <summary>
    /// Checks if the meld code has the specified bit, starting from the least significant bit at index 0.
    /// </summary>
    private static bool HasBit(int meldCode, int bitIndex)
    {
      return (meldCode & 1 << bitIndex) != 0;
    }

    /// <summary>
    /// Apply a bitmask with all ones in a single block and then treat the selected bits as an integer.
    /// For example, with letters as arbitrary bits: IntFromBits(abcdefgh, 3, 2) == 00000def.
    /// </summary>
    /// <param name="value">The value to extract the result from.</param>
    /// <param name="numberOfBits">The number of bits that will be selected.</param>
    /// <param name="leftShift">How many of the least significant bits to ignore.</param>
    /// <returns>The selected bits from the original value.</returns>
    private static int IntFromBits(int value, int numberOfBits, int leftShift)
    {
      var mask = (1 << numberOfBits) - 1;
      return (value >> leftShift) & mask;
    }

    /// <summary>
    /// Gets the tile that was added to a koutsu.
    /// </summary>
    private Tile GetAddedTile(int meldCode)
    {
      return new Tile(GetBaseIndex(meldCode) + GetUnusedOrAddedTileIndex(meldCode));
    }

    /// <summary>
    /// Extracts a base index from the meld code depending on the meld type. How the base index is used also depends on the
    /// meld type.
    /// </summary>
    private int GetBaseIndex(int meldCode)
    {
      if (Type == MeldType.Shuntsu)
      {
        var t = IntFromBits(meldCode, 6, 10) / 3;
        return ((t / 7) * 9 + t % 7) * 4;
      }
      if (Type == MeldType.AddedKan || Type == MeldType.Koutsu)
      {
        return IntFromBits(meldCode, 7, 9) / 3 * 4;
      }
      return IntFromBits(meldCode, 8, 8) & ~3;
    }

    /// <summary>
    /// Extracts the id of the called tile from the meld code.
    /// </summary>
    private Tile GetCalledTile(int meldCode)
    {
      if (Type == MeldType.Shuntsu)
      {
        return new Tile(GetBaseIndex(meldCode) + IntFromBits(meldCode, 6, 10) % 3);
      }
      if (Type == MeldType.AddedKan || Type == MeldType.Koutsu)
      {
        return new Tile(GetBaseIndex(meldCode) + GetKoutsuShiftedIndex(meldCode, IntFromBits(meldCode, 7, 9) % 3));
      }
      return new Tile(GetBaseIndex(meldCode) + IntFromBits(meldCode, 8, 8) % 4);
    }

    /// <summary>
    /// Gets the id of the player that discarded the called tile, or id the owner of the meld for a closed kan.
    /// Not sure about added kan.
    /// </summary>
    /// <param name="meldCode">The meld code.</param>
    /// <param name="ownerId">The id of the owner of the meld.</param>
    private int GetFromPlayerId(int meldCode, int ownerId)
    {
      return (ownerId + IntFromBits(meldCode, 2, 0)) % 4;
    }

    /// <summary>
    /// Shifts the tile index in a tile type based on the index of the missing tile in a koutsu or the added tile in an added
    /// kan.
    /// </summary>
    /// <param name="meldCode">The meld code.</param>
    /// <param name="index">The index that will be shifted.</param>
    private int GetKoutsuShiftedIndex(int meldCode, int index)
    {
      return index + ((GetUnusedOrAddedTileIndex(meldCode) <= index) ? 1 : 0);
    }

    /// <summary>
    /// Determines if the tile has been called, added to a koutsu or selected from the meld owner's hand.
    /// </summary>
    private MeldTileType GetMeldTileType(int meldCode, Tile tile)
    {
      if (Type != MeldType.ClosedKan && tile == GetCalledTile(meldCode))
      {
        return MeldTileType.Called;
      }
      if (Type == MeldType.AddedKan && tile == GetAddedTile(meldCode))
      {
        return MeldTileType.Added;
      }
      return MeldTileType.Normal;
    }

    /// <summary>
    /// Extracts the meld type from the meld code.
    /// </summary>
    /// <param name="meldCode"></param>
    /// <remarks>The order of checks is important. A meld code could possibly have bits 2 and 3 set which makes it a shuntsu.</remarks>
    private MeldType GetMeldType(int meldCode)
    {
      if (HasBit(meldCode, 2))
      {
        return MeldType.Shuntsu;
      }
      if (HasBit(meldCode, 3))
      {
        return MeldType.Koutsu;
      }
      if (HasBit(meldCode, 4))
      {
        return MeldType.AddedKan;
      }
      if (HasBit(meldCode, 5))
      {
        throw new ClientException("Nuki not supported");
      }
      if (HasBit(meldCode, 1) || HasBit(meldCode, 0))
      {
        return MeldType.CalledKan;
      }
      return MeldType.ClosedKan;
    }

    private IEnumerable<Tile> GetTiles(int meldCode)
    {
      var baseIndex = GetBaseIndex(meldCode);
      var tileCount = IsKan() ? 4 : 3;
      for (var i = 0; i < tileCount; ++i)
      {
        switch (Type)
        {
          case MeldType.Shuntsu:
            yield return new Tile(baseIndex + 4 * i + IntFromBits(meldCode, 2, 1 + 2 * i));
            break;
          case MeldType.Koutsu:
            yield return new Tile(baseIndex + GetKoutsuShiftedIndex(meldCode, i));
            break;
          default:
            yield return new Tile(baseIndex + i);
            break;
        }
      }
    }

    private bool IsKan()
    {
      return Type != MeldType.Shuntsu && Type != MeldType.Koutsu;
    }
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
    Called,
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