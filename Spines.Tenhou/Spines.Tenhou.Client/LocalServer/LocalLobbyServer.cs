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
using Spines.Utility;

namespace Spines.Tenhou.Client.LocalServer
{
  internal class LocalLobbyServer
  {
    private readonly MatchServer _matchServer;

    private readonly IDictionary<LocalConnection, StateMachine> _stateMachines =
      new Dictionary<LocalConnection, StateMachine>();

    private readonly LogOnService _logOnService;

    public LocalLobbyServer(ISeedGenerator seedGenerator)
    {
      _logOnService = new LogOnService();
      _matchServer = new MatchServer(seedGenerator, _logOnService);
    }

    public void Process(LocalConnection connection, XElement message)
    {
      StateMachine stateMachine;
      lock (_stateMachines)
      {
        if (!_stateMachines.ContainsKey(connection))
        {
          var lobbyConnection = new LobbyConnection(connection, _matchServer, _logOnService);
          stateMachine = new StateMachine(new ConnectionEstablishedState(lobbyConnection));
          stateMachine.Finished += OnConnectionEnded;
          _stateMachines.Add(connection, stateMachine);
        }
        else
        {
          stateMachine = _stateMachines[connection];
        }
      }
      stateMachine.Process(new Message(InvariantConvert.ToString(connection.GetHashCode()), message));
    }

    private void OnConnectionEnded(object sender, EventArgs e)
    {
      var stateMachine = (StateMachine) sender;
      stateMachine.Finished -= OnConnectionEnded;
      lock (_stateMachines)
      {
        var keyValuePair = _stateMachines.FirstOrDefault(p => p.Value == stateMachine);
        _stateMachines.Remove(keyValuePair);
      }
    }
  }
}