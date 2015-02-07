// Spines.Tenhou.Client.ConfirmGoTransition.cs
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
using Spines.Utility;

namespace Spines.Tenhou.Client.LocalServer.Transitions
{
  internal class ConfirmGoTransition : IStateTransition<LobbyConnection, Match>
  {
    private readonly IState<LobbyConnection, Match> _currentState;

    public ConfirmGoTransition(IState<LobbyConnection, Match> currentState)
    {
      _currentState = currentState;
    }

    public void Execute(Match host)
    {
    }

    public IState<LobbyConnection, Match> PrepareNextState(LobbyConnection sender, Match host)
    {
      Validate.NotNull(host, "host");
      host.ConfirmGo(sender);
      return PrepareNextStateEmpty(host);
    }

    public IState<LobbyConnection, Match> PrepareNextStateEmpty(Match host)
    {
      return _currentState;
    }
  }
}