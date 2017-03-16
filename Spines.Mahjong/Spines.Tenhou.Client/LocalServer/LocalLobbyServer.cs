// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

    private readonly MatchServer _matchServer;

    private readonly IDictionary<LocalConnection, StateMachine> _stateMachines =
      new Dictionary<LocalConnection, StateMachine>();

    private readonly LogOnService _logOnService;

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