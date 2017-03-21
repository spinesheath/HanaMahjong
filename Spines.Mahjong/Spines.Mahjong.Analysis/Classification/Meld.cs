// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// A meld.
  /// </summary>
  public struct Meld
  {
    /// <summary>
    /// The tiles in the meld.
    /// </summary>
    public IEnumerable<Tile> Tiles { get; set; }

    internal Meld(Suit suit, int meldId)
    {
      Tiles = GetTiles(suit, meldId).ToList();
    }

    private static IEnumerable<Tile> GetTiles(Suit suit, int meldId)
    {
      if (meldId < 7)
      {
        return GetShuntsuTiles(suit, meldId);
      }
      if (meldId < 16)
      {
        return GetKoutsuTiles(suit, meldId - 7);
      }
      return GetKantsuTiles(suit, meldId - 16);
    }

    private static IEnumerable<Tile> GetKantsuTiles(Suit suit, int index)
    {
      yield return new Tile { Index = index, Suit = suit, Location = TileLocation.FaceDown };
      yield return new Tile { Index = index, Suit = suit, Location = TileLocation.Melded };
      yield return new Tile { Index = index, Suit = suit, Location = TileLocation.Melded };
      yield return new Tile { Index = index, Suit = suit, Location = TileLocation.FaceDown };
    }

    private static IEnumerable<Tile> GetKoutsuTiles(Suit suit, int index)
    {
      yield return new Tile { Index = index, Suit = suit, Location = TileLocation.Called };
      yield return new Tile { Index = index, Suit = suit, Location = TileLocation.Melded };
      yield return new Tile { Index = index, Suit = suit, Location = TileLocation.Melded };
    }

    private static IEnumerable<Tile> GetShuntsuTiles(Suit suit, int index)
    {
      yield return new Tile {Index = index + 0, Suit = suit, Location = TileLocation.Called};
      yield return new Tile {Index = index + 1, Suit = suit, Location = TileLocation.Melded};
      yield return new Tile {Index = index + 2, Suit = suit, Location = TileLocation.Melded};
    }
  }
}