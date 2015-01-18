// Spines.Tenhou.Client.PlayerInformation.cs
// 
// Copyright (C) 2015  Johannes Heckl
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Information about a player.
  /// </summary>
  public class PlayerInformation
  {
    internal PlayerInformation(XElement message, int playerIndex)
    {
      PlayerIndex = playerIndex;
      UserName = DecodeName(message.Attribute("n" + playerIndex).Value);
      Dan = message.Attribute("dan").Value.Split(new[] {','})[playerIndex];
      Rating = Convert.ToDecimal(message.Attribute("rate").Value.Split(new[] { ',' })[playerIndex], CultureInfo.InvariantCulture);
      Gender = message.Attribute("sx").Value.Split(new[] { ',' })[playerIndex];
    }

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
  }
}