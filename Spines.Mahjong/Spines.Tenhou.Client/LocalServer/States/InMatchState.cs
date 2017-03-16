// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Tenhou.Client.LocalServer.States
{
  internal class InMatchState : StateBase
  {
    public InMatchState(LobbyConnection connection, string accountId)
    {
      _connection = connection;
      _accountId = accountId;
    }

    public override IState Process(Message message)
    {
      RestartTimer();
      if (message.Content.Name == "BYE")
      {
        return new IdleState(_connection, _accountId);
      }
      _connection.MatchServer.ProcessMessage(new Message(_accountId, message.Content));
      return this;
    }

    // TODO use LogOnService in LobbyStates too
    private readonly string _accountId;
    private readonly LobbyConnection _connection;
  }
}