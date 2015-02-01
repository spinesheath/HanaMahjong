// Spines.Tenhou.Client.AcceptMatchTransition.cs
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
  internal class AcceptMatchTransition : IStateTransition<LocalConnection, LobbyConnection>
  {
    private readonly XElement _message;

    public AcceptMatchTransition(XElement message)
    {
      _message = message;
    }

    public void Execute(LobbyConnection host)
    {
      host.MatchServer.ProcessMessage(host, _message);
    }

    public IState<LocalConnection, LobbyConnection> PrepareNextState(LobbyConnection host)
    {
      var parts = _message.Attribute("t").Value.Split(new[] { ',' });
      var lobby = InvariantConvert.ToInt32(parts[0]);
      var matchType = new MatchType(InvariantConvert.ToInt32(parts[1]));
      if (host.MatchServer.IsInMatch(host, lobby, matchType))
      {
        return new InMatchState();
      }
      if (host.MatchServer.IsInQueue(host))
      {
        return new InQueueState();
      }
      return new IdleState();
    }
  }
}