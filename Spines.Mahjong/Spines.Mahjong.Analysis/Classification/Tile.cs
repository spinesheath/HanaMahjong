// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// A tile.
  /// </summary>
  public struct Tile
  {
    /// <summary>
    /// The suit of the tile.
    /// </summary>
    public Suit Suit { get; set; }

    /// <summary>
    /// The index of the tile in the suit.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Is it a red tile?
    /// </summary>
    public bool Aka { get; set; }

    /// <summary>
    /// The location of the tile.
    /// </summary>
    public TileLocation Location { get; set; }

    internal int GetTileType()
    {
      return (int) Suit * 9 + Index;
    }

    internal static Tile FromTileType(int tileType)
    {
      return new Tile {Index = tileType % 9, Suit = (Suit) (tileType / 9)};
    }
  }
}