﻿// Spines.Tenhou.Client.AuthenticatingTransition.cs
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

using Spines.Tenhou.Client.LocalServer.States;

namespace Spines.Tenhou.Client.LocalServer.Transitions
{
  internal class AuthenticatingTransition : IStateTransition<LobbyConnection>
  {
    private readonly string _accountId;
    private readonly string _authenticatedString;

    public AuthenticatingTransition(string accountId, string authenticatedString)
    {
      _accountId = accountId;
      _authenticatedString = authenticatedString;
    }

    public IState<LobbyConnection> GetNextState(LobbyConnection host)
    {
      if (!host.AuthenticationService.IsValid(_accountId, _authenticatedString))
      {
        return new FinalState<LobbyConnection>();
      }
      return new IdleState();
    }

    public void Execute(LobbyConnection host)
    {
    }
  }
}