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
    private string _tenhouId;
    private string _gender;
    private string _lobby;

    /// <summary>
    /// Creates a new Instance of TenhouConnection using the specified ITenhouTcpClient.
    /// </summary>
    /// <param name="client">The ITenhouTcpClient used to connect to the server.</param>
    public TenhouConnection(ITenhouTcpClient client)
    {
      _client = client;
    }

    /// <summary>
    /// Logs a user in.
    /// </summary>
    /// <param name="tenhouId">The Id of the Tenhou Account.</param>
    /// <param name="gender">The gender of the Tenhou Account.</param>
    /// <param name="lobby">The lobby to connect to.</param>
    public void LogOn(string tenhouId, string gender, string lobby)
    {
      tenhouId.ThrowIfNull("tenhouId");
      gender.ThrowIfNull("gender");
      lobby.ThrowIfNull("lobby");
      _tenhouId = tenhouId;
      _gender = gender;
      _lobby = lobby;
      _client.Receive += ReceiveMessage;
      ContactServer();
    }

    /// <summary>
    /// Joins a match.
    /// </summary>
    public void Join()
    {
      var t = new XAttribute("t", "0,9");
      _client.Send(new XElement("JOIN", t));
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
      var lobbyAttribute = new XAttribute("tid", _lobby);
      var genderAttribute = new XAttribute("sx", _gender);
      _client.Send(new XElement("HELO", idAttribute, lobbyAttribute, genderAttribute));
    }

    private void ReceiveMessage(object sender, ReceivedMessageEventArgs e)
    {
      if (e.Message.Name == "HELO")
      {
        var auth = e.Message.Attributes().FirstOrDefault(a => a.Name == "auth");
        if (auth != null)
        {
          Authenticate(auth.Value);
        }
      }
      else if(e.Message.Name == "REJOIN")
      {
        var msg = new XElement(e.Message) {Name = "JOIN"};
        _client.Send(msg);
      }
      else if (e.Message.Name == "GO")
      {
        _client.Send(new XElement("GOK"));
        _client.Send(new XElement("NEXTREADY"));
      }
      else if(e.Message.Name == "UN") // usernames
      {
        
      }
      else if(e.Message.Name == "TAIKYOKU") // oya and game log id
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

    private static bool ContainsNumber(ReceivedMessageEventArgs e)
    {
      return e.Message.Name.LocalName.Any(char.IsNumber);
    }

    private static bool StartsWith(ReceivedMessageEventArgs e, char c)
    {
      return e.Message.Name.LocalName[0] == c;
    }
  }
}