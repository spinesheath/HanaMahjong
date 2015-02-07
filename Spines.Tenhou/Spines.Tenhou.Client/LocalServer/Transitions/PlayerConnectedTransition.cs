// Spines.Tenhou.Client.PlayerConnectedTransition.cs
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
using Spines.Utility;

namespace Spines.Tenhou.Client.LocalServer.Transitions
{
  internal class PlayerConnectedTransition : IStateTransition<LobbyConnection, Match>
  {
    private readonly IState<LobbyConnection, Match> _currentState;
    private readonly int _lobby;
    private readonly MatchType _matchType;

    public PlayerConnectedTransition(XElement message, IState<LobbyConnection, Match> currentState)
    {
      var parts = message.Attribute("t").Value.Split(new[] {','});
      _lobby = InvariantConvert.ToInt32(parts[0]);
      _matchType = new MatchType(InvariantConvert.ToInt32(parts[1]));
      _currentState = currentState;
    }

    public void Execute(Match host)
    {
      Validate.NotNull(host, "host");
      if (host.AreAllPlayersParticipating())
      {
        host.SendGo();
        host.SendTaikyoku();
        host.SendUn();
      }
    }

    public IState<LobbyConnection, Match> PrepareNextState(LobbyConnection sender, Match host)
    {
      Validate.NotNull(host, "host");
      host.ConfirmPlayerAsParticipant(sender, _lobby, _matchType);
      return PrepareNextStateEmpty(host);
    }

    public IState<LobbyConnection, Match> PrepareNextStateEmpty(Match host)
    {
      Validate.NotNull(host, "host");
      return host.AreAllPlayersParticipating() ? new PlayersGettingReadyState() : _currentState;
    }
  }
}