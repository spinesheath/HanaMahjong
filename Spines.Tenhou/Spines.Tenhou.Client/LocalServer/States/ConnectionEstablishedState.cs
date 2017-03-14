// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Tenhou.Client.LocalServer.States
{
  internal class ConnectionEstablishedState : StateBase
  {
    public ConnectionEstablishedState(LobbyConnection connection)
    {
      _connection = connection;
    }

    public override IState Process(Message message)
    {
      RestartTimer();
      if (message.Content.Name != "HELO")
      {
        // TODO Report spam
        return this;
      }
      var accountId = message.Content.Attribute("name").Value;
      if (_connection.TryLogOn(accountId))
      {
        return new AuthenticatingState(_connection, accountId);
      }
      return new FinalState();
    }

    private readonly LobbyConnection _connection;
  }
}