// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// A hand.
  /// </summary>
  public class Hand
  {
    /// <summary>
    /// The current draw.
    /// </summary>
    public Tile? Draw { get; }

    /// <summary>
    /// The concealed tiles in the hand, excluding the draw.
    /// </summary>
    public IEnumerable<Tile> Tiles { get; }

    /// <summary>
    /// The melds in the hand.
    /// </summary>
    public IEnumerable<Meld> Melds { get; }

    internal Hand(IEnumerable<Tile> tiles, IEnumerable<Meld> melds, Tile? draw)
    {
      Tiles = tiles.ToList();
      Melds = melds.ToList();
      Draw = draw;
    }
  }
}