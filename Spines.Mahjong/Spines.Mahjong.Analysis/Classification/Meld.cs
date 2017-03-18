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
    public IEnumerable<Tile> Tiles { get; }

    internal Meld(int meldId, Suit suit)
    {
      Tiles = GetTiles(meldId, suit).ToList();
    }

    private static IEnumerable<Tile> GetTiles(int meldId, Suit suit)
    {
      if (meldId < 7)
      {
        return Enumerable.Range(meldId, 3).Select(i => new Tile {Index = i, Suit = suit});
      }
      if (meldId < 16)
      {
        return Enumerable.Repeat(meldId - 7, 3).Select(i => new Tile {Index = i, Suit = suit});
      }
      return Enumerable.Repeat(meldId - 16, 4).Select(i => new Tile {Index = i, Suit = suit});
    }
  }
}