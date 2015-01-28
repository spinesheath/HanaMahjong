// Spines.Tenhou.Client.LocalMatchServer.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Spines.Utility;

namespace Spines.Tenhou.Client
{
  internal class LocalMatchServer
  {
    private readonly IList<LocalMahjongConnection> _connectedPlayers = new List<LocalMahjongConnection>();
    private readonly WallGenerator _wallGenerator;

    public LocalMatchServer(string seed)
    {
      _wallGenerator = new WallGenerator(seed);
    }

    public event EventHandler Finished;

    public void AcceptMatch(LocalMahjongConnection connection)
    {
      lock (_connectedPlayers)
      {
        if (_connectedPlayers.Count < 4)
        {
          _connectedPlayers.Add(connection);
        }
      }
    }

    public void Start()
    {
      WaitForPlayersToJoinAsync();
    }

    private static IEnumerable<int> GetTiles(IReadOnlyList<int> wall, int oyaIndex, int playerIndex)
    {
      var drawOrderIndex = (4 + playerIndex - oyaIndex) % 4;
      yield return wall[135 - drawOrderIndex * 4];
      yield return wall[134 - drawOrderIndex * 4];
      yield return wall[133 - drawOrderIndex * 4];
      yield return wall[132 - drawOrderIndex * 4];
      yield return wall[135 - 16 - drawOrderIndex * 4];
      yield return wall[134 - 16 - drawOrderIndex * 4];
      yield return wall[133 - 16 - drawOrderIndex * 4];
      yield return wall[132 - 16 - drawOrderIndex * 4];
      yield return wall[135 - 32 - drawOrderIndex * 4];
      yield return wall[134 - 32 - drawOrderIndex * 4];
      yield return wall[133 - 32 - drawOrderIndex * 4];
      yield return wall[132 - 32 - drawOrderIndex * 4];
      yield return wall[135 - 48 - drawOrderIndex * 4];
    }

    private IEnumerable<LocalMahjongConnection> GetConnectedPlayers()
    {
      lock (_connectedPlayers)
      {
        return _connectedPlayers.ToList();
      }
    }

    private void SendGo()
    {
      var type = new XAttribute("type", 9);
      var lobby = new XAttribute("type", 0);
      var gpid = new XAttribute("type", "7167A1C7-5FA3ECC6");
      var message = new XElement("GO", type, lobby, gpid);
      SendToAll(message);
    }

    private void SendInit(int gameIndex)
    {
      var wall = _wallGenerator.GetWall(gameIndex).ToList();
      var dice = _wallGenerator.GetDice(gameIndex).ToList();
      // TODO riichi and honba?
      var seedValues = new[] {gameIndex, 0, 0, dice[0], dice[1], wall[5]};
      var seed = new XAttribute("seed", string.Join(",", seedValues));
      var points = Enumerable.Repeat(250, 4);
      var ten = new XAttribute("ten", string.Join(",", points));
      // TODO how is oya initialized
      const int oyaIndex = 1;
      var oya = new XAttribute("oya", oyaIndex);
      var connectedPlayers = GetConnectedPlayers().ToList();
      for (var i = 0; i < connectedPlayers.Count; ++i)
      {
        var tiles = GetTiles(wall, oyaIndex, i);
        var hai = new XAttribute("ten", string.Join(",", tiles));
        var message = new XElement("INIT", seed, ten, oya, hai);
        connectedPlayers[i].Receive(message);
      }
    }

    private void SendTaikyoku()
    {
      throw new NotImplementedException();
    }

    private void SendToAll(XElement message)
    {
      var connectedPlayers = GetConnectedPlayers();
      foreach (var localMahjongConnection in connectedPlayers)
      {
        localMahjongConnection.Receive(message);
      }
    }

    private void SendUn()
    {
      throw new NotImplementedException();
    }

    private void WaitForPlayersToJoin()
    {
      for (var i = 0; i < 100; ++i)
      {
        var connectedPlayers = GetConnectedPlayers();
        if (connectedPlayers.Count() < 4)
        {
          Thread.Sleep(100);
        }
      }
    }

    private async void WaitForPlayersToJoinAsync()
    {
      var task = Task.Run(() => WaitForPlayersToJoin());
      await task;
      var connectedPlayers = GetConnectedPlayers();
      if (connectedPlayers.Count() < 4)
      {
        EventUtility.CheckAndRaise(Finished, this, new EventArgs());
        return;
      }
      SendGo();
      SendUn();
      SendTaikyoku();
      SendInit(0);
    }
  }
}