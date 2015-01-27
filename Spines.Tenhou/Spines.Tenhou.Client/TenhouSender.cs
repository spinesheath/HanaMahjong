// Spines.Tenhou.Client.TenhouSender.cs
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

using System.Xml.Linq;
using Spines.Utility;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Translates and sends messages to tenhou.net.
  /// </summary>
  internal class TenhouSender
  {
    private readonly LogOnInformation _logOnInformation;
    private readonly ITenhouServer _server;

    /// <summary>
    /// Instantiates a new instance of TenhouSender.
    /// </summary>
    /// <param name="server">The server to send the messages to.</param>
    /// <param name="logOnInformation">Information necessary to log onto the server.</param>
    public TenhouSender(ITenhouServer server, LogOnInformation logOnInformation)
    {
      _server = server;
      _logOnInformation = logOnInformation;
    }

    /// <summary>
    /// Accepts a proposed match.
    /// </summary>
    /// <param name="proposal">The proposed match.</param>
    public void AcceptMatch(MatchProposal proposal)
    {
      Validate.NotNull(proposal, "proposal");
      var t = new XAttribute("t", InvariantConvert.Format("{0},{1},r", proposal.Lobby, proposal.MatchType.TypeId));
      _server.Send(new XElement("JOIN", t));
    }

    /// <summary>
    /// Calls a chii on the last discard with two other tiles.
    /// </summary>
    /// <param name="tile0">A tile from the caller's hand to add to the chii.</param>
    /// <param name="tile1">A tile from the caller's hand to add to the chii.</param>
    public void CallChii(Tile tile0, Tile tile1)
    {
      Validate.NotNull(tile0, "tile0");
      Validate.NotNull(tile1, "tile1");
      var type = new XAttribute("type", "3");
      var hai0 = new XAttribute("hai0", tile0.Id);
      var hai1 = new XAttribute("hai1", tile1.Id);
      _server.Send(new XElement("N", type, hai0, hai1));
    }

    /// <summary>
    /// Denies the last proposed call.
    /// </summary>
    public void DenyCall()
    {
      _server.Send(new XElement("N"));
    }

    /// <summary>
    /// Discards a tile.
    /// </summary>
    /// <param name="tile">The tile to discard.</param>
    public void Discard(Tile tile)
    {
      Validate.NotNull(tile, "tile");
      _server.Send(new XElement("D", new XAttribute("p", tile.Id)));
    }

    /// <summary>
    /// Logs a user in.
    /// </summary>
    public void LogOn()
    {
      var idAttribute = new XAttribute("nodeName", _logOnInformation.TenhouId.Replace("-", "%2D"));
      var lobbyAttribute = new XAttribute("tid", InvariantConvert.ToString(_logOnInformation.Lobby, "D4"));
      var genderAttribute = new XAttribute("sx", _logOnInformation.Gender);
      _server.Send(new XElement("HELO", idAttribute, lobbyAttribute, genderAttribute));
    }

    /// <summary>
    /// Requests a match from the server.
    /// </summary>
    public void RequestMatch()
    {
      var value = InvariantConvert.ToString(_logOnInformation.Lobby) + "," + "9";
      _server.Send(new XElement("JOIN", new XAttribute("t", value)));
    }

    /// <summary>
    /// Authenticates the account.
    /// </summary>
    /// <param name="accountInformation">The account to authenticate.</param>
    public void Authenticate(AccountInformation accountInformation)
    {
      var transformed = Authenticator.Transform(accountInformation.AuthenticationString);
      var valAttribute = new XAttribute("val", transformed);
      _server.Send(new XElement("AUTH", valAttribute));
    }
  }
}