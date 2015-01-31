// Spines.Tenhou.Client.ConnectingToMatchState.cs
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

namespace Spines.Tenhou.Client.LocalServer
{
  /// <summary>
  /// Waits for all players to connect, then moves on to GettingReadyState.
  /// </summary>
  internal class ConnectingToMatchState : LimitedTimeState<int>
  {
    public ConnectingToMatchState()
      : base(10000)
    {
    }

    public override IStateTransition<int> Process(XElement message)
    {
      return new DoNothingTransition<int>(this);
    }

    protected override IStateTransition<int> CreateTimeOutState()
    {
      return new DoNothingTransition<int>(new FinalState<int>());
    }

    //private void SendGo()
    //{
    //  var type = new XAttribute("type", "9");
    //  var lobby = new XAttribute("type", "0");
    //  var gpid = new XAttribute("type", "7167A1C7-5FA3ECC6");
    //  var message = new XElement("GO", type, lobby, gpid);
    //  SendToAll(message);
    //}

    //private void SendTaikyoku()
    //{
    //  // TODO how is oya calculated? In replays it's always 0 at the start, in live matches not
    //  SendToAll(new XElement("TAIKYOKU", new XAttribute("oya", 0)));
    //}

    //private void SendToAll(XElement message)
    //{
    //  var connectedPlayers = GetConnections();
    //  foreach (var player in connectedPlayers)
    //  {
    //    player.Receive(message);
    //  }
    //}

    //private void SendUn()
    //{
    //  var dan = new XAttribute("dan", "12,12,12,10");
    //  var rate = new XAttribute("rate", "1704.57,1675.00,1701.91,1618.53");
    //  var sx = new XAttribute("sx", "M,M,M,M");
    //  var n0 = new XAttribute("n0", "player0");
    //  var n1 = new XAttribute("n1", "player1");
    //  var n2 = new XAttribute("n2", "player2");
    //  var n3 = new XAttribute("n3", "player3");
    //  SendToAll(new XElement("UN", n0, n1, n2, n3, dan, rate, sx));
    //}
  }
}