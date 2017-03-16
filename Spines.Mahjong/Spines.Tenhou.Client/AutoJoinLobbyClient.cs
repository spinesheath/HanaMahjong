// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Lobby client that automatically tries to join a match.
  /// </summary>
  internal class AutoJoinLobbyClient : ILobbyClient
  {
    /// <summary>
    /// Called when the client is logged on.
    /// </summary>
    /// <param name="accountInformation">Information about the account.</param>
    /// <param name="authenticationValue"></param>
    public void LoggedOn(AccountInformation accountInformation, string authenticationValue)
    {
      _sender.Authenticate(authenticationValue);
      if (!_joinedOnce)
      {
        _sender.RequestMatch();
        _joinedOnce = true;
      }
    }

    /// <summary>
    /// Called when a match is started.
    /// </summary>
    /// <param name="matchInformation">Information about the match.</param>
    public void MatchStarted(MatchInformation matchInformation)
    {
    }

    /// <summary>
    /// Called when the connection to the server has been established.
    /// </summary>
    public void Connected()
    {
      _sender.LogOn();
    }

    private readonly TenhouSender _sender;
    private bool _joinedOnce;

    /// <summary>
    /// Creates a new instance of AutoJoinLobbyClient.
    /// </summary>
    /// <param name="sender">Used to send messages to the server.</param>
    internal AutoJoinLobbyClient(TenhouSender sender)
    {
      _sender = sender;
    }
  }
}