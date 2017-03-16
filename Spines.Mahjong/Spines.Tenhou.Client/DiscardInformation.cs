// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Xml.Linq;
using Spines.Utility;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Data provided by the server when a player discards a tile.
  /// </summary>
  public class DiscardInformation
  {
    /// <summary>
    /// 0, 1, 2, 3 for T, U, V, W.
    /// </summary>
    public int PlayerIndex { get; private set; }

    /// <summary>
    /// Id of the discarded tile.
    /// </summary>
    public Tile Tile { get; private set; }

    /// <summary>
    /// True if the discarded tile is the last tile drawn.
    /// </summary>
    public bool Tsumokiri { get; }

    /// <summary>
    /// True if the discarded tile can be called.
    /// </summary>
    public bool Callable { get; private set; }

    internal DiscardInformation(XElement message)
    {
      Callable = message.Attributes("t").Any();
      var name = message.Name.LocalName;
      Tsumokiri = "defg".Contains(name[0]);
      PlayerIndex = name[0] - (Tsumokiri ? 'd' : 'D');
      Tile = new Tile(InvariantConvert.ToInt32(name.Substring(1)));
    }
  }
}