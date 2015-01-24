// Spines.Tenhou.Client.TsumokiriAi.cs
// 
// Copyright (C) 2015  Johannes Heckl
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// An AI that always discards the drawn tile and nothing else.
  /// </summary>
  public class TsumokiriAi : IMatchClient
  {
    private readonly TenhouSender _sender;

    /// <summary>
    /// Creates a new instance of TsumokiriAi.
    /// </summary>
    /// <param name="sender">Used to send messages to the server.</param>
    public TsumokiriAi(TenhouSender sender)
    {
      _sender = sender;
    }

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
      throw new System.NotImplementedException();
    }
  }
}