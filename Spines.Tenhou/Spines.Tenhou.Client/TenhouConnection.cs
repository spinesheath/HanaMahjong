// Spines.Tenhou.Client.TenhouConnection.cs
// 
// Copyright (C) 2014  Johannes Heckl
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

using Spines.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Connection to Tenhou.net.
  /// </summary>
  public class TenhouConnection
  {
    private readonly ILobbyClient _lobbyClient;
    private readonly ITenhouTcpClient _client;
    private readonly Dictionary<string, Action<XElement>> _messageActions = new Dictionary<string, Action<XElement>>();
    private readonly LogOnInformation _logOnInformation;
    private IMatchClient _matchClient;

    /// <summary>
    /// Creates a new Instance of TenhouConnection using the specified ITenhouTcpClient.
    /// </summary>
    /// <param name="client">The ITenhouTcpClient used to connect to the server.</param>
    /// <param name="logOnInformation">The information necessary to log onto tenhou.net.</param>
    /// <param name="lobbyClient">A client for the tenhou lobby.</param>
    public TenhouConnection(ITenhouTcpClient client, LogOnInformation logOnInformation, ILobbyClient lobbyClient)
    {
      _client = Validate.NotNull(client, "client");
      _logOnInformation = Validate.NotNull(logOnInformation, "logOnInformation");
      _lobbyClient = Validate.NotNull(lobbyClient, "lobbyClient");
      InitializeMessageActions();
    }

    /// <summary>
    /// Raised when a player discards a tile.
    /// </summary>
    public event EventHandler<DiscardEventArgs> Discard;

    /// <summary>
    /// Raised when an opponent draws a tile.
    /// </summary>
    public event EventHandler<OpponentDrawEventArgs> OpponentDraw;

    /// <summary>
    /// Raised when the active player draws a tile.
    /// </summary>
    public event EventHandler<PlayerDrawEventArgs> PlayerDraw;

    /// <summary>
    /// Joins a match.
    /// </summary>
    public void Join()
    {
      var value = InvariantConvert.ToString(_logOnInformation.Lobby) + "," + "9";
      _client.Send(new XElement("JOIN", new XAttribute("t", value)));
    }

    /// <summary>
    /// Logs a user in.
    /// </summary>
    public void LogOn()
    {
      _client.Receive += ReceiveMessage;
      ContactServer();
    }

    private static bool IsDiscard(string nodeName)
    {
      return "DEFGdefg".Contains(nodeName[0]) && nodeName.Any(char.IsNumber);
    }

    private static bool IsOpponentDraw(string nodeName)
    {
      return nodeName == "U" || nodeName == "V" || nodeName == "W";
    }

    private static bool IsPlayerDraw(string nodeName)
    {
      return nodeName[0] == 'T' && nodeName.Any(char.IsNumber);
    }

    private void Authenticate(string authenticationString)
    {
      var transformed = Authenticator.Transform(authenticationString);
      var valAttribute = new XAttribute("val", transformed);
      _client.Send(new XElement("AUTH", valAttribute));
    }

    private void ContactServer()
    {
      var idAttribute = new XAttribute("nodeName", _logOnInformation.TenhouId.Replace("-", "%2D"));
      var lobbyAttribute = new XAttribute("tid", InvariantConvert.ToString(_logOnInformation.Lobby, "D4"));
      var genderAttribute = new XAttribute("sx", _logOnInformation.Gender);
      _client.Send(new XElement("HELO", idAttribute, lobbyAttribute, genderAttribute));
    }

    private void InitializeMessageActions()
    {
      _messageActions.Add("HELO", OnReceivedLoggedOn);
      _messageActions.Add("REJOIN", OnReceivedRejoin);
      _messageActions.Add("GO", OnReceivedGo);
      _messageActions.Add("LN", m => { });
      _messageActions.Add("UN", OnReceivedPlayers);
      _messageActions.Add("TAIKYOKU", OnReceivedTaikyoku);
      _messageActions.Add("INIT", m => { });
      _messageActions.Add("REACH", m => { });
      _messageActions.Add("N", OnReceivedCall);
      _messageActions.Add("DORA", m => { });
      _messageActions.Add("PROF", m => { });
      _messageActions.Add("RANKING", m => { });
      _messageActions.Add("CHAT", m => { });
      _messageActions.Add("AGARI", OnReceivedAgariOrRyuukyoku);
      _messageActions.Add("RYUUKYOKU", OnReceivedAgariOrRyuukyoku);
    }

    private void OnReceivedTaikyoku(XElement message)
    {
      var firstDealerIndex = InvariantConvert.ToInt32(message.Attribute("oya").Value);
      var logId = message.Attribute("log").Value;
      _matchClient.SetFirstDealer(firstDealerIndex);
      _matchClient.SetLogId(logId);
    }

    private void OnReceivedPlayers(XElement message)
    {
      _matchClient.UpdatePlayers(GetPlayers(message));
    }

    private static IEnumerable<PlayerInformation> GetPlayers(XElement message)
    {
      var playerCount = message.Attributes().Count(a => a.Name.LocalName.StartsWith("n", StringComparison.InvariantCulture));
      return Enumerable.Range(0, playerCount).Select(i => new PlayerInformation(message, i));
    }

    private void OnReceivedCall(XElement message)
    {
      
    }

    private void OnReceivedAgariOrRyuukyoku(XElement message)
    {
      if (message.Attributes().Any(a => a.Name == "owari"))
      {
        _client.Send(new XElement("BYE"));
        _client.Send(new XElement("BYE"));
        ContactServer();
      }
      else
      {
        _client.Send(new XElement("NEXTREADY"));
      }
    }

    private void OnReceivedGo(XElement message)
    {
      _client.Send(new XElement("GOK"));
      _client.Send(new XElement("NEXTREADY"));
      _matchClient.Start(new MatchInformation(message));
    }

    private void OnReceivedLoggedOn(XElement message)
    {
      Authenticate(message.Attribute("auth").Value);
      _lobbyClient.LoggedOn(new AccountInformation(message));
    }

    private void OnReceivedRejoin(XElement message)
    {
      var parts = message.Attribute("t").Value.Split(new []{','});
      var lobby = InvariantConvert.ToInt32(parts[0]);
      var matchType = new MatchType(parts[1]);
      if (!_lobbyClient.ProposeMatch(lobby, matchType))
      {
        return;
      }
      _matchClient = _lobbyClient.GetMatchClient();
      _client.Send(new XElement(message) {Name = "JOIN"});
    }

    private void ReceiveMessage(object sender, ReceivedMessageEventArgs e)
    {
      var nodeName = e.Message.Name.LocalName;
      if (_messageActions.ContainsKey(nodeName))
      {
        _messageActions[nodeName](e.Message);
      }
      else if (IsPlayerDraw(nodeName))
      {
        EventUtility.CheckAndRaise(PlayerDraw, this, new PlayerDrawEventArgs(e.Message));
      }
      else if (IsOpponentDraw(nodeName))
      {
        EventUtility.CheckAndRaise(OpponentDraw, this, new OpponentDrawEventArgs(e.Message));
      }
      else if (IsDiscard(nodeName))
      {
        EventUtility.CheckAndRaise(Discard, this, new DiscardEventArgs(e.Message));
      }
    }

    /// <summary>
    /// Tells the server that a callable tile will not be called.
    /// </summary>
    public void SendDenyCall()
    {
      _client.Send(new XElement("N"));
    }

    /// <summary>
    /// Tells the server that the most recent discard was called for a chii together with two other tiles.
    /// </summary>
    public void SendChii(Tile tile0, Tile tile1)
    {
      Validate.NotNull(tile0, "tile0");
      Validate.NotNull(tile1, "tile1");
      var type = new XAttribute("type", "3");
      var hai0 = new XAttribute("hai0", tile0.Id);
      var hai1 = new XAttribute("hai1", tile1.Id);
      _client.Send(new XElement("N", type, hai0, hai1));
    }

    /// <summary>
    /// Tells the server that a tile is discarded.
    /// </summary>
    public void SendDiscard(Tile tile)
    {
      Validate.NotNull(tile, "tile");
      _client.Send(new XElement("D", new XAttribute("p", tile.Id)));
    }
  }
}