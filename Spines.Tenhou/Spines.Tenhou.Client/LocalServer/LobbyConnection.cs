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
    private readonly LogOnService _logOnService;
    private readonly RegistrationService _registrationService;

    public LobbyConnection(LocalConnection connection, MatchServer matchServer, LogOnService logOnService)
    {
      _connection = connection;
      _logOnService = logOnService;
      _registrationService = new RegistrationService();
      AuthenticationService = new AuthenticationService();
      MatchServer = matchServer;
    }

    public MatchServer MatchServer { get; private set; }
    public IAuthenticationService AuthenticationService { get; private set; }

    public bool TryLogOn(string accountId)
    {
      // TODO combine Registration, Authentication and LogOnServices
      if (!_logOnService.TryLogOn(accountId, _connection))
      {
        return false;
      }
      var accountInformation = _registrationService.GetAccountInformation(accountId);
      var authenticationString = AuthenticationService.GetAuthenticationString(accountId);
      var message = accountInformation.ToMessage();
      message.Add(new XAttribute("auth", authenticationString));
      SendToClient(message);
      return true;
    }

    private void SendToClient(XElement message)
    {
      _connection.Receive(message);
    }

    public void LogOff(string accountId)
    {
      _logOnService.LogOff(accountId);
    }
  }
}