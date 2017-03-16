// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Spines.Utility;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Information about a player.
  /// </summary>
  public class PlayerInformation
  {
    /// <summary>
    /// The index of the player in the match.
    /// </summary>
    public int PlayerIndex { get; private set; }

    /// <summary>
    /// The name of the player.
    /// </summary>
    public string UserName { get; private set; }

    /// <summary>
    /// The rank of the player.
    /// </summary>
    public string Dan { get; private set; }

    /// <summary>
    /// The rating of the player.
    /// </summary>
    public decimal Rating { get; private set; }

    /// <summary>
    /// The gender of the player.
    /// </summary>
    public string Gender { get; private set; }

    private static string DecodeName(string encodedName)
    {
      if (encodedName.Length <= 0 || encodedName[0] != '%')
      {
        return encodedName;
      }
      var encodedCharacters = encodedName.Split(new[] {'%'}, StringSplitOptions.RemoveEmptyEntries);
      var bytes = encodedCharacters.Select(c => Convert.ToByte(c, 16)).ToArray();
      return new UTF8Encoding().GetString(bytes);
    }

    internal PlayerInformation(XElement message, int playerIndex)
    {
      PlayerIndex = playerIndex;
      UserName = DecodeName(message.Attribute("n" + playerIndex).Value);
      Dan = message.Attribute("dan").Value.Split(',')[playerIndex];
      Rating = InvariantConvert.ToDecimal(message.Attribute("rate").Value.Split(',')[playerIndex]);
      Gender = message.Attribute("sx").Value.Split(',')[playerIndex];
    }
  }
}