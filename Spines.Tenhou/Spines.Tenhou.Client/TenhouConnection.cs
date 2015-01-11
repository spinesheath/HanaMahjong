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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Spines.Utility;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Connection to Tenhou.net.
  /// </summary>
  public class TenhouConnection
  {
    private readonly ITenhouTcpClient _client;
    private readonly string _gender;
    private readonly int _lobby;
    private readonly Dictionary<string, Action<XElement>> _messageActions = new Dictionary<string, Action<XElement>>();
    private readonly string _tenhouId;

    /// <summary>
    /// Creates a new Instance of TenhouConnection using the specified ITenhouTcpClient.
    /// </summary>
    /// <param name="client">The ITenhouTcpClient used to connect to the server.</param>
    /// <param name="tenhouId">The Id of the Tenhou Account.</param>
    /// <param name="gender">The gender of the Tenhou Account.</param>
    /// <param name="lobby">The lobby to connect to.</param>
    public TenhouConnection(ITenhouTcpClient client, string tenhouId, string gender, int lobby)
    {
      client.ThrowIfNull("client");
      tenhouId.ThrowIfNull("tenhouId");
      gender.ThrowIfNull("gender");
      _tenhouId = tenhouId;
      _gender = gender;
      _lobby = lobby;
      _client = client;
      InitializeMessageActions();
    }

    /// <summary>
    /// Raised when a player discards a tile.
    /// </summary>
    public event EventHandler<DiscardEventArgs> Discard;

    /// <summary>
    /// Raised when the account was successfully logged on.
    /// </summary>
    public event EventHandler<LoggedOnEventArgs> LoggedOn;

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
      var value = _lobby.ToString(CultureInfo.InvariantCulture) + "," + "9";
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
      var idAttribute = new XAttribute("nodeName", _tenhouId.Replace("-", "%2D"));
      var lobbyAttribute = new XAttribute("tid", _lobby.ToString("D4", CultureInfo.InvariantCulture));
      var genderAttribute = new XAttribute("sx", _gender);
      _client.Send(new XElement("HELO", idAttribute, lobbyAttribute, genderAttribute));
    }

    private void InitializeMessageActions()
    {
      _messageActions.Add("HELO", OnRecievedLoggedOn);
      _messageActions.Add("REJOIN", OnRecievedRejoin);
      _messageActions.Add("GO", OnReceivedGo);
      _messageActions.Add("UN", m => { });
      _messageActions.Add("TAIKYOKU", m => { });
      _messageActions.Add("INIT", m => { });
      _messageActions.Add("REACH", m => { });
      _messageActions.Add("N", m => { });
      _messageActions.Add("DORA", m => { });
      _messageActions.Add("PROF", m => { });
      _messageActions.Add("RANKING", m => { });
      _messageActions.Add("CHAT", m => { });
      _messageActions.Add("AGARI", OnReceivedAgariOrRyuukyoku);
      _messageActions.Add("RYUUKYOKU", OnReceivedAgariOrRyuukyoku);
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

    private void OnReceivedDiscard(XElement message)
    {
      if (Discard != null)
      {
        Discard(this, new DiscardEventArgs(message));
      }
    }

    private void OnReceivedGo(XElement message)
    {
      _client.Send(new XElement("GOK"));
      _client.Send(new XElement("NEXTREADY"));
    }

    private void OnReceivedOpponentDraw(XElement message)
    {
      if (OpponentDraw != null)
      {
        OpponentDraw(this, new OpponentDrawEventArgs(message));
      }
    }

    private void OnReceivedPlayerDraw(XElement message)
    {
      if (PlayerDraw != null)
      {
        PlayerDraw(this, new PlayerDrawEventArgs(message));
      }
    }

    private void OnRecievedLoggedOn(XElement message)
    {
      Authenticate(message.Attribute("auth").Value);
      if (LoggedOn != null)
      {
        LoggedOn(this, new LoggedOnEventArgs(message));
      }
    }

    private void OnRecievedRejoin(XElement message)
    {
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
        OnReceivedPlayerDraw(e.Message);
      }
      else if (IsOpponentDraw(nodeName))
      {
        OnReceivedOpponentDraw(e.Message);
      }
      else if (IsDiscard(nodeName))
      {
        OnReceivedDiscard(e.Message);
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
      _client.Send(new XElement("D", new XAttribute("p", tile.Id)));
    }
  }
}