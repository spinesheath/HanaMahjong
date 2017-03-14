// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Spines.Tenhou.Client.LocalServer
{
  internal class PlayerStatus
  {
    public PlayerStatus(int playerIndex)
    {
      PlayerIndex = playerIndex;
    }

    /// <summary>
    /// The index of the player in the match.
    /// </summary>
    public int PlayerIndex { get; private set; }

    /// <summary>
    /// Whether the player is participating in the match.
    /// </summary>
    public bool IsParticipating { get; set; }

    /// <summary>
    /// Whether the player confirmed the GO message.
    /// </summary>
    public bool AcceptedGo { get; set; }

    /// <summary>
    /// Whether the player sent the NEXTREADY message after the GO message or the most recent game.
    /// </summary>
    public bool IsReadyForNextGame { get; set; }

    public void AddTile(Tile tile)
    {
      _closedHand.Add(tile);
    }

    public bool HasTileInClosedHand(Tile tile)
    {
      return _closedHand.Contains(tile);
    }

    public void RemoveTile(Tile tile)
    {
      _closedHand.Remove(tile);
    }

    private readonly ISet<Tile> _closedHand = new HashSet<Tile>();
  }
}