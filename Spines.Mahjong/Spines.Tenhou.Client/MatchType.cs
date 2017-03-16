// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// The type of a match.
  /// </summary>
  public class MatchType
  {
    /// <summary>
    /// True if the match is played with 3 red fives.
    /// </summary>
    public bool AkaAri { get; private set; }

    internal MatchType(int typeId)
    {
      AkaAri = typeId == 9;
      TypeId = typeId;
    }

    /// <summary>
    /// The id of the match type as used by tenhou.net.
    /// </summary>
    internal int TypeId { get; set; }
  }
}