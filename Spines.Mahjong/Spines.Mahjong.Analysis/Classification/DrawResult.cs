// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// The result of a draw.
  /// </summary>
  internal enum DrawResult
  {
    /// <summary>
    /// The tile was drawn normally.
    /// </summary>
    Draw,

    /// <summary>
    /// The hand was won off the tile.
    /// </summary>
    Tsumo
  }
}