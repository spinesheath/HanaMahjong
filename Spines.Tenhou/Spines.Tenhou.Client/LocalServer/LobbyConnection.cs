// Spines.Tenhou.Client.LobbyConnection.cs
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

namespace Spines.Tenhou.Client.LocalServer
{
  internal class LobbyConnection
  {
    private readonly LocalConnection _connection;

    public LobbyConnection(LocalConnection connection, MatchServer matchServer)
    {
      _connection = connection;
      RegistrationService = new RegistrationService();
      AuthenticationService = new AuthenticationService();
      MatchServer = matchServer;
    }

    public MatchServer MatchServer { get; private set; }
    public IRegistrationService RegistrationService { get; private set; }
    public IAuthenticationService AuthenticationService { get; private set; }

    public void SendToClient(XElement message)
    {
      _connection.Receive(message);
    }
  }
}