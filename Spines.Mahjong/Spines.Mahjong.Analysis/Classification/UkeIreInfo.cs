// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// Information about the tiles that improve the hand after a discard.
  /// </summary>
  public class UkeIreInfo
  {
    /// <summary>
    /// The discarded tile.
    /// </summary>
    public Tile? Discard { get; }

    /// <summary>
    /// The tiles that improve the hand after a discard and the number of remaining tiles.
    /// </summary>
    public Dictionary<Tile, int> Outs { get; }

    internal UkeIreInfo(Tile? discard, Dictionary<Tile, int> outs)
    {
      Discard = discard;
      Outs = outs;
    }
  }
}