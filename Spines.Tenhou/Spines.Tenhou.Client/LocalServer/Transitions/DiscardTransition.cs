// Spines.Tenhou.Client.DiscardTransition.cs
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
  internal class DiscardTransition : IStateTransition<LobbyConnection, Match>
  {
    private readonly IState<LobbyConnection, Match> _currentState;
    private readonly Tile _tile;
    private LobbyConnection _sender;

    public DiscardTransition(IState<LobbyConnection, Match> currentState, XElement message)
    {
      _currentState = currentState;
      _tile = new Tile(InvariantConvert.ToInt32(message.Attribute("p").Value));
    }

    public void Execute(Match host)
    {
      if (!host.IsActive(_sender) || !host.HasTileInClosedHand(_sender, _tile))
      {
        return;
      }
      host.SendDiscard(_tile);
      if (host.CanDraw())
      {
        host.SendDraw();
      }
      else
      {
        host.SendRyuukyoku();
      }
    }

    public IState<LobbyConnection, Match> PrepareNextState(LobbyConnection sender, Match host)
    {
      // TODO clean this up, don't want to hold sender across method calls.
      _sender = sender;
      return PrepareNextStateEmpty(host);
    }

    public IState<LobbyConnection, Match> PrepareNextStateEmpty(Match host)
    {
      return _currentState;
    }
  }
}