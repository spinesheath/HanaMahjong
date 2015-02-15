// Spines.Tenhou.Client.ConnectionEstablishedState.cs
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

namespace Spines.Tenhou.Client.LocalServer.States
{
  internal class ConnectionEstablishedState : StateBase
  {
    private readonly LobbyConnection _connection;

    public ConnectionEstablishedState(LobbyConnection connection)
    {
      _connection = connection;
    }

    public override IState Process(Message message)
    {
      RestartTimer();
      if (message.Content.Name != "HELO")
      {
        // TODO Report spam
        return this;
      }
      var accountId = message.Content.Attribute("name").Value;
      if (!_connection.RegistrationService.IsRegistered(accountId))
      {
        return new FinalState();
      }
      LogOn(accountId);
      return new AuthenticatingState(_connection, accountId);
    }

    // TODO move this elsewhere, for example let authentication service create the message, or a translator class
    private void LogOn(string accountId)
    {
      var accountInformation = _connection.RegistrationService.GetAccountInformation(accountId);
      var authenticationString = _connection.AuthenticationService.GetAuthenticationString(accountId);
      var message = accountInformation.ToMessage();
      message.Add(new XAttribute("auth", authenticationString));
      _connection.SendToClient(message);
      _connection.LogOn(accountId);
    }
  }
}