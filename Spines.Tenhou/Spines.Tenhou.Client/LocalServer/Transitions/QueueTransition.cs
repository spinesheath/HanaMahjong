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

namespace Spines.Tenhou.Client.LocalServer.Transitions
{
  internal class QueueTransition : IStateTransition<LobbyConnection>
  {
    private readonly int _lobby;
    private readonly MatchType _matchType;

    public QueueTransition(int lobby, MatchType matchType)
    {
      _lobby = lobby;
      _matchType = matchType;
    }

    public IState<LobbyConnection> GetNextState(LobbyConnection host)
    {
      return new InQueueState();
    }

    public void Execute(LobbyConnection host)
    {
      host.EnterQueue(_lobby, _matchType);
    }
  }
}