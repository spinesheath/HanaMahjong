// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Tenhou.Client.LocalServer.States
{
  internal class AuthenticatingState : StateBase
  {
    public AuthenticatingState(LobbyConnection connection, string accountId)
    {
      _connection = connection;
      _accountId = accountId;
    }

    public override IState Process(Message message)
    {
      RestartTimer();
      if (message.Content.Name != "AUTH")
      {
        return this;
      }
      var v = message.Content.Attribute("val").Value;
      if (_connection.AuthenticationService.IsValid(_accountId, v))
      {
        return new IdleState(_connection, _accountId);
      }
      return new FinalState();
    }

    private readonly string _accountId;
    private readonly LobbyConnection _connection;
  }
}