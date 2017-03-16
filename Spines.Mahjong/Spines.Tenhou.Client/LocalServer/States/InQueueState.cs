// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Tenhou.Client.LocalServer.States
{
  internal class InQueueState : StateBase
  {
    public InQueueState(LobbyConnection connection, string accountId)
    {
      _connection = connection;
      _accountId = accountId;
    }

    public override IState Process(Message message)
    {
      RestartTimer();
      if (message.Content.Name != "JOIN")
      {
        return this;
      }
      if (_connection.MatchServer.IsInMatch(_accountId))
      {
        _connection.MatchServer.ProcessMessage(new Message(_accountId, message.Content));
        return new InMatchState(_connection, _accountId);
      }
      if (_connection.MatchServer.IsInQueue(_accountId))
      {
        return this;
      }
      return new IdleState(_connection, _accountId);
    }

    private readonly string _accountId;
    private readonly LobbyConnection _connection;
  }
}