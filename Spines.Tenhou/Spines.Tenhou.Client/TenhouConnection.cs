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

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Connection to Tenhou.net.
  /// </summary>
  public class TenhouConnection
  {
    private readonly ITenhouTcpClient _client;

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
      _client.Receive += ReceiveMessage;
      ContactServer(tenhouId.Replace("-", "%2D"), gender, lobby);
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
    }

    private void ContactServer(string accountId, string gender, string lobby)
    {
      var idAttribute = new XAttribute("name", accountId);
      var lobbyAttribute = new XAttribute("tid", lobby);
      var genderAttribute = new XAttribute("sx", gender);
      _client.Send(new XElement("HELO", idAttribute, lobbyAttribute, genderAttribute));
    }

    private void Authenticate(string authenticationString)
    {
      var transformed = Authenticator.Transform(authenticationString);
      var valAttribute = new XAttribute("val", transformed);
      _client.Send(new XElement("AUTH", valAttribute));
    }
  }
}