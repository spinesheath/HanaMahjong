// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml.Linq;

namespace Spines.Tenhou.Client.LocalServer
{
  internal class LobbyConnection
  {
    public LobbyConnection(LocalConnection connection, MatchServer matchServer, LogOnService logOnService)
    {
      _connection = connection;
      _logOnService = logOnService;
      _registrationService = new RegistrationService();
      AuthenticationService = new AuthenticationService();
      MatchServer = matchServer;
    }

    public MatchServer MatchServer { get; private set; }
    public IAuthenticationService AuthenticationService { get; }

    public bool TryLogOn(string accountId)
    {
      // TODO combine Registration, Authentication and LogOnServices
      if (!_logOnService.TryLogOn(accountId, _connection))
      {
        return false;
      }
      var accountInformation = _registrationService.GetAccountInformation(accountId);
      var authenticationString = AuthenticationService.GetAuthenticationString(accountId);
      var message = accountInformation.ToMessage();
      message.Add(new XAttribute("auth", authenticationString));
      SendToClient(message);
      return true;
    }

    public void LogOff(string accountId)
    {
      _logOnService.LogOff(accountId);
    }

    private readonly LocalConnection _connection;
    private readonly LogOnService _logOnService;
    private readonly RegistrationService _registrationService;

    private void SendToClient(XElement message)
    {
      _connection.Receive(message);
    }
  }
}