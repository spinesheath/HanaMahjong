// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Information necessary to log onto tenhou.net.
  /// </summary>
  public class LogOnInformation
  {
    /// <summary>
    /// Initializes a new instance of LogOnInformation.
    /// </summary>
    /// <param name="tenhouId">The Id of the Tenhou Account.</param>
    /// <param name="gender">The gender of the Tenhou Account.</param>
    /// <param name="lobby">The lobby to connect to.</param>
    public LogOnInformation(string tenhouId, string gender, int lobby)
    {
      TenhouId = tenhouId;
      Gender = gender;
      Lobby = lobby;
    }

    /// <summary>
    /// The Id of the Tenhou Account.
    /// </summary>
    public string TenhouId { get; private set; }

    /// <summary>
    /// The gender of the Tenhou Account.
    /// </summary>
    public string Gender { get; private set; }

    /// <summary>
    /// The lobby to connect to.
    /// </summary>
    public int Lobby { get; private set; }
  }
}