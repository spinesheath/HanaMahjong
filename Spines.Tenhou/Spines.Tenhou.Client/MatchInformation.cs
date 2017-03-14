// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml.Linq;
using Spines.Utility;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Information about a match.
  /// </summary>
  public class MatchInformation
  {
    /// <summary>
    /// The type of the match.
    /// </summary>
    public MatchType MatchType { get; private set; }

    /// <summary>
    /// The lobby for the match.
    /// </summary>
    public int Lobby { get; private set; }

    /// <summary>
    /// The match id, required for rejoining a match after losing connection.
    /// </summary>
    public string MatchId { get; private set; }

    /// <summary>
    /// Instantiates a new instance of MatchInformation.
    /// </summary>
    internal MatchInformation(XElement message)
    {
      MatchType = new MatchType(InvariantConvert.ToInt32(message.Attribute("type").Value));
      Lobby = InvariantConvert.ToInt32(message.Attribute("lobby").Value);
      MatchId = message.Attribute("gpid").Value;
    }
  }
}