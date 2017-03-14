// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Spines.Utility;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// An AI that always discards the drawn tile and nothing else.
  /// </summary>
  internal class TsumokiriAI : IMatchClient
  {
    /// <summary>
    /// Asks the client whether to join a match.
    /// </summary>
    /// <param name="proposal">The proposed match.</param>
    public void ProposeMatch(MatchProposal proposal)
    {
      _sender.AcceptMatch(proposal);
    }

    /// <summary>
    /// Called when a match starts.
    /// </summary>
    /// <param name="matchInformation">Information about the match.</param>
    public void Start(MatchInformation matchInformation)
    {
    }

    /// <summary>
    /// Provides the client with information about the players in a match.
    /// </summary>
    /// <param name="players">Information about the players.</param>
    public void UpdatePlayers(IEnumerable<PlayerInformation> players)
    {
    }

    /// <summary>
    /// Tells the client which player is the first dealer.
    /// </summary>
    /// <param name="firstDealerIndex">The playerIndex of the first dealer.</param>
    public void SetFirstDealer(int firstDealerIndex)
    {
    }

    /// <summary>
    /// Tells the client the id of the match log.
    /// </summary>
    /// <param name="logId">The id of the match log.</param>
    public void SetLogId(string logId)
    {
    }

    /// <summary>
    /// Tells the client that the active player drew a tile.
    /// </summary>
    /// <param name="tile">The tile that was drawn.</param>
    public void DrawTile(Tile tile)
    {
      _sender.Discard(tile);
    }

    /// <summary>
    /// Tells the cilent that an opponent drew a tile.
    /// </summary>
    /// <param name="playerIndex">The player index of the opponent.</param>
    public void OpponentDrawTile(int playerIndex)
    {
    }

    /// <summary>
    /// Tells the client that a tile was discarded.
    /// </summary>
    /// <param name="discardInformation">Information about the discard.</param>
    public void Discard(DiscardInformation discardInformation)
    {
      Validate.NotNull(discardInformation, "discardInformation");
      if (discardInformation.Callable)
      {
        _sender.DenyCall();
      }
    }

    private readonly TenhouSender _sender;

    /// <summary>
    /// Creates a new instance of TsumokiriAI.
    /// </summary>
    /// <param name="sender">Used to send messages to the server.</param>
    internal TsumokiriAI(TenhouSender sender)
    {
      _sender = sender;
    }
  }
}