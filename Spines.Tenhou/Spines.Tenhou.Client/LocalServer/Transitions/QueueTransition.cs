// Spines.Tenhou.Client.QueueTransition.cs
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
  internal class QueueTransition : IStateTransition<LocalConnection, LobbyConnection>
  {
    private readonly int _lobby;
    private readonly MatchType _matchType;

    public QueueTransition(int lobby, MatchType matchType)
    {
      _lobby = lobby;
      _matchType = matchType;
    }

    public IState<LocalConnection, LobbyConnection> PrepareNextState(LocalConnection sender, LobbyConnection host)
    {
      return PrepareNextStateEmpty(host);
    }

    public IState<LocalConnection, LobbyConnection> PrepareNextStateEmpty(LobbyConnection host)
    {
      Validate.NotNull(host, "host");
      if (host.MatchServer.CanEnterQueue(host, _lobby, _matchType))
      {
        return new InQueueState();
      }
      return new IdleState();
    }

    public void Execute(LobbyConnection host)
    {
      Validate.NotNull(host, "host");
      if (host.MatchServer.CanEnterQueue(host, _lobby, _matchType))
      {
        host.MatchServer.EnterQueue(host, _lobby, _matchType);
      }
    }
  }
}