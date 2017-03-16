// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// A proposal of a match.
  /// </summary>
  public class MatchProposal
  {
    /// <summary>
    /// The lobby the match is played in.
    /// </summary>
    public int Lobby { get; private set; }

    /// <summary>
    /// The type of the match.
    /// </summary>
    public MatchType MatchType { get; private set; }

    internal MatchProposal(int lobby, MatchType matchType)
    {
      Lobby = lobby;
      MatchType = matchType;
    }
  }
}