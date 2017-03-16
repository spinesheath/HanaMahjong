// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Recieves and translates messages from the tenhou.net server.
  /// </summary>
  public interface ITenhouReceiver
  {
    /// <summary>
    /// Adds a listener for lobby messages.
    /// </summary>
    /// <param name="listener">The listener.</param>
    void AddLobbyListener(ILobbyClient listener);

    /// <summary>
    /// Adds a listener for match messages.
    /// </summary>
    /// <param name="listener">The listener.</param>
    void AddMatchListener(IMatchClient listener);
  }
}