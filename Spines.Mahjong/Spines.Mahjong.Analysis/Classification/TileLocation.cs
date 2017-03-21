// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// The location of a tile.
  /// </summary>
  public enum TileLocation
  {
    /// <summary>
    /// Location not specified.
    /// </summary>
    None,

    /// <summary>
    /// In a hand.
    /// </summary>
    Concealed,

    /// <summary>
    /// In the pond discarded.
    /// </summary>
    Discarded,

    /// <summary>
    /// Removed from a hand for calling a meld.
    /// </summary>
    Melded,

    /// <summary>
    /// Called for a meld.
    /// </summary>
    Called,

    /// <summary>
    /// The tile added for a chakan.
    /// </summary>
    Added,

    /// <summary>
    /// Face down on the table for an ankan.
    /// </summary>
    FaceDown,

    /// <summary>
    /// In the pond as the tile to indicate a riichi.
    /// </summary>
    Riichi
  }
}