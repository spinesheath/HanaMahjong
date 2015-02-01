// Spines.Tenhou.Client.PassMessageToMatchTransition.cs
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
  internal class PassMessageToMatchTransition : IStateTransition<LocalConnection, LobbyConnection>
  {
    private readonly XElement _message;
    private readonly IState<LocalConnection, LobbyConnection> _nextState;

    public PassMessageToMatchTransition(XElement message, IState<LocalConnection, LobbyConnection> nextState)
    {
      _message = message;
      _nextState = nextState;
    }

    public void Execute(LobbyConnection host)
    {
      host.MatchServer.ProcessMessage(host, _message);
    }

    public IState<LocalConnection, LobbyConnection> PrepareNextState(LocalConnection sender, LobbyConnection host)
    {
      return PrepareNextStateEmpty(host);
    }

    public IState<LocalConnection, LobbyConnection> PrepareNextStateEmpty(LobbyConnection host)
    {
      return _nextState;
    }
  }
}