// Spines.Tenhou.Client.PlayersGettingReadyState.cs
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

namespace Spines.Tenhou.Client.LocalServer.States
{
  internal class PlayersGettingReadyState : StateBase
  {
    private readonly Match _match;

    public PlayersGettingReadyState(Match match)
    {
      _match = match;
    }

    public override IState Process(Message message)
    {
      // TODO if timer runs out, start match anyways
      RestartTimer();
      if (message.Content.Name == "GOK")
      {
        _match.ConfirmGo(message.SenderId);
        return this;
      }
      if (message.Content.Name == "NEXTREADY")
      {
        _match.ConfirmPlayerIsReady(message.SenderId);
        if (!_match.AreAllPlayersReadyForNextGame())
        {
          return this;
        }
        _match.StartNextGame();
        var timePerTurn = _match.TimePerTurn;
        var extraTime = _match.GetRemainingExtraTime();
        return new PlayerActiveState(_match, timePerTurn + extraTime);
      }
      return this;
    }
  }
}