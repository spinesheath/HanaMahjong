// Spines.Tenhou.Client.LocalLobbyServer.cs
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
using System.Xml.Linq;
using Spines.Tenhou.Client.LocalServer.States;

namespace Spines.Tenhou.Client.LocalServer
{
  internal class LocalLobbyServer
  {
    private readonly IDictionary<LocalConnection, StateMachine<LocalConnection, LobbyConnection>> _stateMachines =
      new Dictionary<LocalConnection, StateMachine<LocalConnection, LobbyConnection>>();

    private readonly MatchServer _matchServer;

    public LocalLobbyServer(ISeedGenerator seedGenerator)
    {
      _matchServer = new MatchServer(seedGenerator);
    }

    public void Send(LocalConnection connection, XElement message)
    {
      StateMachine<LocalConnection, LobbyConnection> stateMachine;
      lock (_stateMachines)
      {
        if (!_stateMachines.ContainsKey(connection))
        {
          var lobbyConnection = new LobbyConnection(connection, _matchServer);
          stateMachine = new StateMachine<LocalConnection, LobbyConnection>(lobbyConnection, new ConnectionEstablishedState());
          stateMachine.Finished += OnConnectionEnded;
          _stateMachines.Add(connection, stateMachine);
        }
        else
        {
          stateMachine = _stateMachines[connection];
        }
      }
      stateMachine.ProcessMessage(connection, message);
    }

    private void OnConnectionEnded(object sender, EventArgs e)
    {
      var stateMachine = (StateMachine<LocalConnection, LobbyConnection>)sender;
      stateMachine.Finished -= OnConnectionEnded;
      lock (_stateMachines)
      {
        var keyValuePair = _stateMachines.FirstOrDefault(p => p.Value == stateMachine);
        _stateMachines.Remove(keyValuePair);
      }
    }

    //private IEnumerable<LocalConnection> AddPlayerAndTake4(LocalConnection connection)
    //{
    //  lock (_playersInMatches)
    //  {
    //    if (_playersInMatches.ContainsKey(connection))
    //    {
    //      return Enumerable.Empty<LocalConnection>();
    //    }
    //  }
    //  lock (_matchQueue)
    //  {
    //    _matchQueue.Add(connection);
    //    if (_matchQueue.Count <= 3)
    //    {
    //      return Enumerable.Empty<LocalConnection>();
    //    }
    //    var players = _matchQueue.Take(4).ToList();
    //    foreach (var localMahjongConnection in players)
    //    {
    //      _matchQueue.Remove(localMahjongConnection);
    //    }
    //    return players;
    //  }
    //}

    //private void OnMatchFinished(object sender, EventArgs e)
    //{
    //  var localMatchServer = (LocalMatchServer) sender;
    //  localMatchServer.Finished -= OnMatchFinished;
    //  var players = Enumerable.Empty<LocalConnection>();
    //  lock (_matchServers)
    //  {
    //    if (_matchServers.ContainsKey(localMatchServer))
    //    {
    //      players = _matchServers[localMatchServer];
    //      _matchServers.Remove(localMatchServer);
    //    }
    //  }
    //  lock (_playersInMatches)
    //  {
    //    foreach (var localMahjongConnection in players)
    //    {
    //      _playersInMatches.Remove(localMahjongConnection);
    //    }
    //  }
    //}

    //private void AcceptMatch(LocalConnection connection, AccountInformation account)
    //{
    //  lock (_playersInMatches)
    //  {
    //    if (_playersInMatches.ContainsKey(connection))
    //    {
    //      _playersInMatches[connection].AcceptMatch(connection, account);
    //    }
    //  }
    //}

    //private void ProposeMatch(LocalConnection connection)
    //{
    //  var players = AddPlayerAndTake4(connection).ToList();
    //  if (players.Count < 4)
    //  {
    //    return;
    //  }
    //  var seed = _seedGenerator.CreateSeed();
    //  var localMatchServer = new LocalMatchServer(seed);
    //  lock (_playersInMatches)
    //  {
    //    foreach (var localMahjongConnection in players)
    //    {
    //      _playersInMatches.Add(localMahjongConnection, localMatchServer);
    //    }
    //  }
    //  localMatchServer.Finished += OnMatchFinished;
    //  localMatchServer.Start();
    //  lock (_matchServers)
    //  {
    //    _matchServers.Add(localMatchServer, players);
    //  }
    //  foreach (var localMahjongConnection in players)
    //  {
    //    localMahjongConnection.Receive(new XElement("REJOIN", new XAttribute("t", "0,9,r")));
    //  }
    //}
  }
}