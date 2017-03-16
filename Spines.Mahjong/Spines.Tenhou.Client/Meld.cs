// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Spines.Utility;

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
      var meldCode = new MeldCode(InvariantConvert.ToInt32(meldCodeString), ownerId);
      Type = meldCode.Type;
      Tiles = meldCode.Tiles;
      FromPlayerId = meldCode.FromPlayerId;
      OwnerId = meldCode.OwnerId;
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
    public IEnumerable<MeldTile> Tiles { get; private set; }

    /// <summary>
    /// The type of the meld.
    /// </summary>
    public MeldType Type { get; private set; }
  }
}