// Spines.Tenhou.Client.PlayerReadyTransition.cs
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
  internal class PlayerReadyTransition : IStateTransition<LobbyConnection, Match>
  {
    private readonly IState<LobbyConnection, Match> _currentState;

    public PlayerReadyTransition(IState<LobbyConnection, Match> currentState)
    {
      _currentState = currentState;
    }

    public void Execute(Match host)
    {
      Validate.NotNull(host, "host");
      if (host.AreAllPlayersReadyForNextGame())
      {
        host.StartNextGame();
      }
    }

    public IState<LobbyConnection, Match> PrepareNextState(LobbyConnection sender, Match host)
    {
      Validate.NotNull(host, "host");
      host.ConfirmPlayerIsReady(sender);
      return PrepareNextStateEmpty(host);
    }

    public IState<LobbyConnection, Match> PrepareNextStateEmpty(Match host)
    {
      Validate.NotNull(host, "host");
      if (!host.AreAllPlayersReadyForNextGame())
      {
        return _currentState;
      }
      var timePerTurn = host.TimePerTurn;
      var extraTime = host.GetRemainingExtraTime();
      return new PlayerActiveState(timePerTurn + extraTime);
    }
  }
}