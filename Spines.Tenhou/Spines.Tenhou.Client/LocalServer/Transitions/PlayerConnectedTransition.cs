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
        SendGo(host);
        SendTaikyoku(host);
        SendUn(host);
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

    private static void SendGo(Match host)
    {
      var type = new XAttribute("type", "9");
      var lobby = new XAttribute("lobby", "0");
      var gpid = new XAttribute("gpid", "7167A1C7-5FA3ECC6");
      host.SendToAll(new XElement("GO", type, lobby, gpid));
    }

    private static void SendTaikyoku(Match host)
    {
      // TODO how is oya calculated? In replays it's always 0 at the start, in live matches not
      var oya = new XAttribute("oya", 0);
      var log = new XAttribute("log", "2012102722gm-0009-0000-9e067f8e");
      host.SendToAll(new XElement("TAIKYOKU", oya, log));
    }

    private static void SendUn(Match host)
    {
      var dan = new XAttribute("dan", "12,12,12,10");
      var rate = new XAttribute("rate", "1704.57,1675.00,1701.91,1618.53");
      var sx = new XAttribute("sx", "M,M,M,M");
      var n0 = new XAttribute("n0", "player0");
      var n1 = new XAttribute("n1", "player1");
      var n2 = new XAttribute("n2", "player2");
      var n3 = new XAttribute("n3", "player3");
      host.SendToAll(new XElement("UN", n0, n1, n2, n3, dan, rate, sx));
    }
  }
}