// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// A client for a match.
  /// </summary>
  public interface IMatchClient
  {
    /// <summary>
    /// Asks the client whether to join a match.
    /// </summary>
    /// <param name="proposal">The proposed match.</param>
    void ProposeMatch(MatchProposal proposal);

    /// <summary>
    /// Called when a match starts.
    /// </summary>
    /// <param name="matchInformation">Information about the match.</param>
    void Start(MatchInformation matchInformation);

    /// <summary>
    /// Provides the client with information about the players in a match.
    /// </summary>
    /// <param name="players">Information about the players.</param>
    void UpdatePlayers(IEnumerable<PlayerInformation> players);

    /// <summary>
    /// Tells the client which player is the first dealer.
    /// </summary>
    /// <param name="firstDealerIndex">The playerIndex of the first dealer.</param>
    void SetFirstDealer(int firstDealerIndex);

    /// <summary>
    /// Tells the client the id of the match log.
    /// </summary>
    /// <param name="logId">The id of the match log.</param>
    void SetLogId(string logId);

    /// <summary>
    /// Tells the client that the active player drew a tile.
    /// </summary>
    /// <param name="tile">The tile that was drawn.</param>
    void DrawTile(Tile tile);

    /// <summary>
    /// Tells the cilent that an opponent drew a tile.
    /// </summary>
    /// <param name="playerIndex">The player index of the opponent.</param>
    void OpponentDrawTile(int playerIndex);

    /// <summary>
    /// Tells the client that a tile was discarded.
    /// </summary>
    /// <param name="discardInformation">Information about the discard.</param>
    void Discard(DiscardInformation discardInformation);
  }
}