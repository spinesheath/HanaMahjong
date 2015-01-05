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
using Spines.Utility;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

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

    private void InitializeMessageActions()
    {
      _messageActions.Add("HELO", OnRecievedLoggedOn);
    }

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

    /// <summary>
    /// Raised when the account was successfully logged on.
    /// </summary>
    public event EventHandler<LoggedOnEventArgs> LoggedOn;

    private readonly Dictionary<string, Action<XElement>> _messageActions = new Dictionary<string, Action<XElement>>();

    private void ReceiveMessage(object sender, ReceivedMessageEventArgs e)
    {
      var nodeName = e.Message.Name.LocalName;
      if (_messageActions.ContainsKey(nodeName))
      {
        _messageActions[nodeName](e.Message);
      }
      else if (e.Message.Name == "REJOIN")
      {
        var msg = new XElement(e.Message) {Name = "JOIN"};
        _client.Send(msg);
      }
      else if (e.Message.Name == "GO")
      {
        _client.Send(new XElement("GOK"));
        _client.Send(new XElement("NEXTREADY"));
      }
      else if (e.Message.Name == "UN") // usernames
      {
      }
      else if (e.Message.Name == "TAIKYOKU") // oya and game log id
      {
      }
      else if (e.Message.Name == "INIT") // starting hand
      {
      }
      else if (e.Message.Name == "REACH")
      {
      }
      else if (e.Message.Name == "N") // call
      {
      }
      else if (e.Message.Name == "DORA") // dora revealed after kan
      {
      }
      else if (e.Message.Name == "PROF")
      {
      }
      else if (e.Message.Name == "RANKING")
      {
      }
      else if (e.Message.Name == "CHAT")
      {
      }
      else if (e.Message.Name == "AGARI")
      {
        if (e.Message.Attributes().Any(a => a.Name == "owari"))
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
      else if (e.Message.Name == "RYUUKYOKU")
      {
        if (e.Message.Attributes().Any(a => a.Name == "owari"))
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
      else if (e.Message.Name == "T" || e.Message.Name == "U" || e.Message.Name == "V" || e.Message.Name == "W")
      {
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

    private void OnRecievedLoggedOn(XElement message)
    {
      Authenticate(message.Attribute("auth").Value);
      if (LoggedOn != null)
        LoggedOn(this, new LoggedOnEventArgs(message));
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