// Spines.Tenhou.Client.LogOnTransition.cs
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
using Spines.Tenhou.Client.LocalServer.States;

namespace Spines.Tenhou.Client.LocalServer.Transitions
{
  internal class LogOnTransition : IStateTransition<LobbyConnection>
  {
    private readonly string _accountId;

    public LogOnTransition(string accountId)
    {
      _accountId = accountId;
    }

    public IState<LobbyConnection> GetNextState(LobbyConnection host)
    {
      if (!host.RegistrationService.IsRegistered(_accountId))
      {
        return new FinalState<LobbyConnection>();
      }
      var authenticationString = host.AuthenticationService.GetAuthenticationString(_accountId);
      return new AuthenticatingState(_accountId, authenticationString);
    }

    public void Execute(LobbyConnection host)
    {
      if (host.RegistrationService.IsRegistered(_accountId))
      {
        LogOn(host);
      }
    }

    private void LogOn(LobbyConnection host)
    {
      var accountInformation = host.RegistrationService.GetAccountInformation(_accountId);
      var authenticationString = host.AuthenticationService.GetAuthenticationString(_accountId);
      var message = accountInformation.ToMessage();
      message.Add(new XAttribute("auth", authenticationString));
      host.SendToClient(message);
    }
  }
}