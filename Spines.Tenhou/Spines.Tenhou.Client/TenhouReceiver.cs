// Spines.Tenhou.Client.TenhouReceiver.cs
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
  /// Recieves and translates messages from Tenhou.net.
  /// </summary>
  public class TenhouReceiver
  {
    private readonly ILobbyClient _lobbyClient;
    private readonly ITenhouTcpClient _server;
    private readonly Dictionary<string, Action<XElement>> _messageActions = new Dictionary<string, Action<XElement>>();
    private readonly TenhouSender _sender;
    private readonly IMatchClient _matchClient;

    /// <summary>
    /// Creates a new Instance of TenhouReceiver using the specified ITenhouTcpClient.
    /// </summary>
    /// <param name="client">The ITenhouTcpClient used to connect to the server.</param>
    /// <param name="sender">Used to send messages to the server.</param>
    /// <param name="lobbyClient">A client for the tenhou lobby.</param>
    /// <param name="matchClient">A match client.</param>
    public TenhouReceiver(ITenhouTcpClient client, TenhouSender sender, ILobbyClient lobbyClient, IMatchClient matchClient)
    {
      _server = Validate.NotNull(client, "client");
      _lobbyClient = Validate.NotNull(lobbyClient, "lobbyClient");
      _sender = Validate.NotNull(sender, "sender");
      _matchClient = Validate.NotNull(matchClient, "matchClient");
      InitializeMessageActions();
      _server.Receive += ReceiveMessage;
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
      _server.Send(new XElement("AUTH", valAttribute));
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
      var playerCount = message.Attributes().Count(a => a.Name.LocalName[0] == 'n');
      return Enumerable.Range(0, playerCount).Select(i => new PlayerInformation(message, i));
    }

    private void OnReceivedCall(XElement message)
    {
      
    }

    private void OnReceivedAgariOrRyuukyoku(XElement message)
    {
      if (message.Attributes().Any(a => a.Name == "owari"))
      {
        _server.Send(new XElement("BYE"));
        _server.Send(new XElement("BYE"));
        _sender.LogOn();
      }
      else
      {
        _server.Send(new XElement("NEXTREADY"));
      }
    }

    private void OnReceivedGo(XElement message)
    {
      _server.Send(new XElement("GOK"));
      _server.Send(new XElement("NEXTREADY"));
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
      _matchClient.ProposeMatch(new MatchProposal(lobby, matchType));
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
        _matchClient.DrawTile(CreateTileFromDrawOrDiscard(e.Message));
      }
      else if (IsOpponentDraw(nodeName))
      {
        _matchClient.OpponentDrawTile(GetPlayerIndexFromDrawOrDiscard(e.Message));
      }
      else if (IsDiscard(nodeName))
      {
        _matchClient.Discard(new DiscardInformation(e.Message));
      }
    }

    /// <summary>
    /// Extracts the player index from a draw or discard message.
    /// </summary>
    /// <param name="message">The draw or discard message.</param>
    /// <returns>The player index.</returns>
    private static int GetPlayerIndexFromDrawOrDiscard(XElement message)
    {
      return message.Name.LocalName[0] - 'T';
    }

    /// <summary>
    /// Creates a tile from the name of a draw or discard message.
    /// </summary>
    /// <param name="message">The draw or discard message.</param>
    /// <returns>The drawn or discarded tile.</returns>
    private static Tile CreateTileFromDrawOrDiscard(XElement message)
    {
      return new Tile(InvariantConvert.ToInt32(message.Name.LocalName.Substring(1)));
    }
  }
}