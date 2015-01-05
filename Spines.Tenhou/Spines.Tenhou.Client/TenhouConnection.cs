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
    /// Raised when the account was successfully logged on.
    /// </summary>
    public event EventHandler<LoggedOnEventArgs> LoggedOn;

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

    private static bool ContainsNumber(ReceivedMessageEventArgs e)
    {
      return e.Message.Name.LocalName.Any(char.IsNumber);
    }

    private static bool StartsWith(ReceivedMessageEventArgs e, char c)
    {
      return e.Message.Name.LocalName[0] == c;
    }

    private void Authenticate(string authenticationString)
    {
      var transformed = Authenticator.Transform(authenticationString);
      var valAttribute = new XAttribute("val", transformed);
      _client.Send(new XElement("AUTH", valAttribute));
    }

    private void ContactServer()
    {
      var idAttribute = new XAttribute("name", _tenhouId.Replace("-", "%2D"));
      var lobbyAttribute = new XAttribute("tid", _lobby.ToString("D4", CultureInfo.InvariantCulture));
      var genderAttribute = new XAttribute("sx", _gender);
      _client.Send(new XElement("HELO", idAttribute, lobbyAttribute, genderAttribute));
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
      _messageActions.Add("T", m => { });
      _messageActions.Add("U", m => { });
      _messageActions.Add("V", m => { });
      _messageActions.Add("W", m => { });
    }

    private void ReceiveMessage(object sender, ReceivedMessageEventArgs e)
    {
      var nodeName = e.Message.Name.LocalName;
      if (_messageActions.ContainsKey(nodeName))
      {
        _messageActions[nodeName](e.Message);
      }
      else if (StartsWith(e, 'T') && ContainsNumber(e))
      {
        SendTsumokiri(e, "D");
      }
      else if (StartsWith(e, 'U') && ContainsNumber(e))
      {
        SendTsumokiri(e, "E");
      }
      else if (StartsWith(e, 'V') && ContainsNumber(e))
      {
        SendTsumokiri(e, "F");
      }
      else if (StartsWith(e, 'W') && ContainsNumber(e))
      {
        SendTsumokiri(e, "G");
      }
      else if (StartsWith(e, 'D') && ContainsNumber(e))
      {
        SendDenyCall(e);
      }
      else if (StartsWith(e, 'E') && ContainsNumber(e))
      {
        SendDenyCall(e);
      }
      else if (StartsWith(e, 'F') && ContainsNumber(e))
      {
        SendDenyCall(e);
      }
      else if (StartsWith(e, 'G') && ContainsNumber(e))
      {
        SendDenyCall(e);
      }
      else if (StartsWith(e, 'd') && ContainsNumber(e))
      {
        SendDenyCall(e);
      }
      else if (StartsWith(e, 'e') && ContainsNumber(e))
      {
        SendDenyCall(e);
      }
      else if (StartsWith(e, 'f') && ContainsNumber(e))
      {
        SendDenyCall(e);
      }
      else if (StartsWith(e, 'g') && ContainsNumber(e))
      {
        SendDenyCall(e);
      }
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
    }

    private void SendDenyCall(ReceivedMessageEventArgs e)
    {
      if (e.Message.Attributes().Any(a => a.Name == "t"))
      {
        _client.Send(new XElement("N"));
      }
    }

    private void SendTsumokiri(ReceivedMessageEventArgs e, string name)
    {
      var p = new XAttribute("p", e.Message.Name.LocalName.Substring(1));
      _client.Send(new XElement(name, p));
    }
  }
}