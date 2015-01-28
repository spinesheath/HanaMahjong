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
    private WallGenerator _wallGenerator;

    public LocalMatchServer(string seed)
    {
      _wallGenerator = new WallGenerator(seed);
    }

    public event EventHandler Finished;

    public void AcceptMatch(LocalMahjongConnection connection)
    {
      lock (_connectedPlayers)
      {
        if(_connectedPlayers.Count < 4)
        {
          _connectedPlayers.Add(connection);
        }
      }
    }

    public void Start()
    {
      WaitForPlayersToJoinAsync();
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
    }

    private void SendGo()
    {
      var type = new XAttribute("type", 9);
      var lobby = new XAttribute("type", 0);
      var gpid = new XAttribute("type", "7167A1C7-5FA3ECC6");
      var message = new XElement("GO", type, lobby, gpid);
      SendToAll(message);
    }

    private void SendToAll(XElement message)
    {
      var connectedPlayers = GetConnectedPlayers();
      foreach (var localMahjongConnection in connectedPlayers)
      {
        localMahjongConnection.Receive(message);
      }
    }

    private IEnumerable<LocalMahjongConnection> GetConnectedPlayers()
    {
      lock (_connectedPlayers)
      {
        return _connectedPlayers.ToList();
      }
    }
  }
}