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
    public Suit Suit { get; internal set; }

    /// <summary>
    /// The index of the tile in the suit.
    /// </summary>
    public int Index { get; internal set; }

    /// <summary>
    /// Is it a red tile?
    /// </summary>
    public bool Aka { get; internal set; }
  }
}