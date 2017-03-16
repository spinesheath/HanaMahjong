// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml.Linq;
using Spines.Utility;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Translates and sends messages to tenhou.net.
  /// </summary>
  internal class TenhouSender
  {
    /// <summary>
    /// Instantiates a new instance of TenhouSender.
    /// </summary>
    /// <param name="connection">The connection to send the messages to.</param>
    /// <param name="logOnInformation">Information necessary to log onto the connection.</param>
    public TenhouSender(ITenhouConnection connection, LogOnInformation logOnInformation)
    {
      _connection = connection;
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
      _connection.Send(new XElement("JOIN", t));
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
      _connection.Send(new XElement("N", type, hai0, hai1));
    }

    /// <summary>
    /// Denies the last proposed call.
    /// </summary>
    public void DenyCall()
    {
      _connection.Send(new XElement("N"));
    }

    /// <summary>
    /// Discards a tile.
    /// </summary>
    /// <param name="tile">The tile to discard.</param>
    public void Discard(Tile tile)
    {
      Validate.NotNull(tile, "tile");
      _connection.Send(new XElement("D", new XAttribute("p", tile.Id)));
    }

    /// <summary>
    /// Logs a user in.
    /// </summary>
    public void LogOn()
    {
      var idAttribute = new XAttribute("name", _logOnInformation.TenhouId.Replace("-", "%2D"));
      var lobbyAttribute = new XAttribute("tid", InvariantConvert.ToString(_logOnInformation.Lobby, "D4"));
      var genderAttribute = new XAttribute("sx", _logOnInformation.Gender);
      _connection.Send(new XElement("HELO", idAttribute, lobbyAttribute, genderAttribute));
    }

    /// <summary>
    /// Requests a match from the connection.
    /// </summary>
    public void RequestMatch()
    {
      var value = InvariantConvert.ToString(_logOnInformation.Lobby) + "," + "9";
      _connection.Send(new XElement("JOIN", new XAttribute("t", value)));
    }

    /// <summary>
    /// Authenticates the account.
    /// </summary>
    /// <param name="authenticationString">The authentication string.</param>
    public void Authenticate(string authenticationString)
    {
      var transformed = Authenticator.Transform(authenticationString);
      var valAttribute = new XAttribute("val", transformed);
      _connection.Send(new XElement("AUTH", valAttribute));
    }

    private readonly LogOnInformation _logOnInformation;
    private readonly ITenhouConnection _connection;
  }
}