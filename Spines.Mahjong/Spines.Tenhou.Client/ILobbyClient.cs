// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// A client for the tenhou lobby.
  /// </summary>
  public interface ILobbyClient
  {
    /// <summary>
    /// Called when the client is logged on.
    /// </summary>
    /// <param name="accountInformation">Information about the account.</param>
    /// <param name="authenticationValue">The value to authenticate.</param>
    void LoggedOn(AccountInformation accountInformation, string authenticationValue);

    /// <summary>
    /// Called when a match is started.
    /// </summary>
    /// <param name="matchInformation">Information about the match.</param>
    void MatchStarted(MatchInformation matchInformation);

    /// <summary>
    /// Called when the connection to the server has been established.
    /// </summary>
    void Connected();
  }
}