// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Spines.Utility;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Extracts information from a meld code.
  /// </summary>
  internal class MeldCode
  {
    /// <summary>
    /// Instantiates a new instance of MeldCode.
    /// </summary>
    /// <param name="code">The meld code that specifies the meld.</param>
    /// <param name="ownerId">The owner of the meld.</param>
    public MeldCode(int code, int ownerId)
    {
      _code = new BitField(code);
      OwnerId = ownerId;
      FromPlayerId = GetFromPlayerId();
      Type = GetMeldType();
      Tiles = GetTiles().Select(tile => new MeldTile(tile, GetMeldTileType(tile)));
    }

    /// <summary>
    /// The id of the owner of the meld.
    /// </summary>
    public int OwnerId { get; }

    /// <summary>
    /// The id of the player that discarded the called tile, or the owner in case of a closed kan. Not sure about added kan.
    /// </summary>
    public int FromPlayerId { get; private set; }

    /// <summary>
    /// The tiles in the meld.
    /// </summary>
    public IEnumerable<MeldTile> Tiles { get; private set; }

    /// <summary>
    /// The type of the meld.
    /// </summary>
    public MeldType Type { get; }

    /// <summary>
    /// The code of the meld.
    /// </summary>
    private readonly BitField _code;

    /// <summary>
    /// Gets the tile that was added to a koutsu.
    /// </summary>
    private Tile GetAddedTile()
    {
      return new Tile(GetBaseIndex() + GetUnusedOrAddedTileIndex());
    }

    /// <summary>
    /// Extracts a base index from the meld code depending on the meld type. How the base index is used also depends on the
    /// meld type.
    /// </summary>
    private int GetBaseIndex()
    {
      if (Type == MeldType.Shuntsu)
      {
        var t = _code.ExtractSegment(6, 10) / 3;
        return (t / 7 * 9 + t % 7) * 4;
      }
      if (Type == MeldType.AddedKan || Type == MeldType.Koutsu)
      {
        return _code.ExtractSegment(7, 9) / 3 * 4;
      }
      return _code.ExtractSegment(8, 8) & ~3;
    }

    /// <summary>
    /// Extracts the id of the called tile from the meld code.
    /// </summary>
    private Tile GetCalledTile()
    {
      if (Type == MeldType.Shuntsu)
      {
        return new Tile(GetBaseIndex() + _code.ExtractSegment(6, 10) % 3);
      }
      if (Type == MeldType.AddedKan || Type == MeldType.Koutsu)
      {
        return new Tile(GetBaseIndex() + GetKoutsuShiftedIndex(_code.ExtractSegment(7, 9) % 3));
      }
      return new Tile(GetBaseIndex() + _code.ExtractSegment(8, 8) % 4);
    }

    /// <summary>
    /// Gets the id of the player that discarded the called tile, or id the owner of the meld for a closed kan.
    /// Not sure about added kan.
    /// </summary>
    private int GetFromPlayerId()
    {
      return (OwnerId + _code.ExtractSegment(2, 0)) % 4;
    }

    /// <summary>
    /// Shifts the tile index in a tile type based on the index of the missing tile in a koutsu or the added tile in an added
    /// kan.
    /// </summary>
    /// <param name="index">The index that will be shifted.</param>
    private int GetKoutsuShiftedIndex(int index)
    {
      return index + (GetUnusedOrAddedTileIndex() <= index ? 1 : 0);
    }

    /// <summary>
    /// Determines if the tile has been called, added to a koutsu or selected from the meld owner's hand.
    /// </summary>
    private MeldTileType GetMeldTileType(Tile tile)
    {
      if (Type != MeldType.ClosedKan && tile == GetCalledTile())
      {
        return MeldTileType.Called;
      }
      if (Type == MeldType.AddedKan && tile == GetAddedTile())
      {
        return MeldTileType.Added;
      }
      return MeldTileType.Normal;
    }

    /// <summary>
    /// Extracts the meld type from the meld code.
    /// </summary>
    /// <remarks>The order of checks is important. A meld code could possibly have bits 2 and 3 set which makes it a shuntsu.</remarks>
    private MeldType GetMeldType()
    {
      if (_code.HasBit(2))
      {
        return MeldType.Shuntsu;
      }
      if (_code.HasBit(3))
      {
        return MeldType.Koutsu;
      }
      if (_code.HasBit(4))
      {
        return MeldType.AddedKan;
      }
      if (_code.HasBit(5))
      {
        throw new ClientException("Nuki is not supported");
      }
      if (_code.HasBit(1) || _code.HasBit(0))
      {
        return MeldType.CalledKan;
      }
      return MeldType.ClosedKan;
    }

    /// <summary>
    /// Extracts the tiles of the meld from the meld code.
    /// </summary>
    private IEnumerable<Tile> GetTiles()
    {
      var baseIndex = GetBaseIndex();
      var tileCount = IsKantsu() ? 4 : 3;
      for (var i = 0; i < tileCount; ++i)
      {
        switch (Type)
        {
          case MeldType.Shuntsu:
            yield return new Tile(baseIndex + 4 * i + _code.ExtractSegment(2, 1 + 2 * i));
            break;
          case MeldType.Koutsu:
            yield return new Tile(baseIndex + GetKoutsuShiftedIndex(i));
            break;
          default:
            yield return new Tile(baseIndex + i);
            break;
        }
      }
    }

    /// <summary>
    /// Calculates the index of the tile among all the tiles of a tile type that is not used by a koutsu or was added to a
    /// koutsu.
    /// </summary>
    private int GetUnusedOrAddedTileIndex()
    {
      return _code.ExtractSegment(2, 5);
    }

    /// <summary>
    /// True if the meld is a kantsu.
    /// </summary>
    private bool IsKantsu()
    {
      return Type != MeldType.Shuntsu && Type != MeldType.Koutsu;
    }
  }
}