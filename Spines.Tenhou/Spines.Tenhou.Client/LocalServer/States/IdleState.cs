// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Spines.Utility;

namespace Spines.Tenhou.Client.LocalServer.States
{
  internal class IdleState : StateBase
  {
    public IdleState(LobbyConnection connection, string accountId)
    {
      _connection = connection;
      _accountId = accountId;
    }

    public override IState Process(Message message)
    {
      RestartTimer();
      if (message.Content.Name == "BYE")
      {
        _connection.LogOff(_accountId);
        return new ConnectionEstablishedState(_connection);
      }
      if (message.Content.Name != "JOIN")
      {
        return this;
      }
      var parts = message.Content.Attribute("t").Value.Split(',');
      var lobby = InvariantConvert.ToInt32(parts[0]);
      var matchType = new MatchType(InvariantConvert.ToInt32(parts[1]));
      if (!_connection.MatchServer.CanEnterQueue(_accountId))
      {
        return this;
      }
      _connection.MatchServer.EnterQueue(_accountId, lobby, matchType);
      return new InQueueState(_connection, _accountId);
    }

    private readonly string _accountId;
    private readonly LobbyConnection _connection;
  }
}