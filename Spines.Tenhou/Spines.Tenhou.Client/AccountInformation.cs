// Spines.Tenhou.Client.AccountInformation.cs
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

using Spines.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Data provided by the server after logging on.
  /// </summary>
  public class AccountInformation
  {
    internal AccountInformation(XElement message)
    {
      RatingScales = GetRatingScales2(message);
      ExpireDays = GetExpireDays(message);
      ExpireDate = GetExpireDate(message);
      UserName = GetUserName(message);
    }

    /// <summary>
    /// The username of the account that was logged on.
    /// </summary>
    public string UserName { get; private set; }

    /// <summary>
    /// The date of expiry for the account.
    /// </summary>
    public DateTime ExpireDate { get; private set; }

    /// <summary>
    /// The number of days until the account expires.
    /// </summary>
    public int ExpireDays { get; private set; }

    /// <summary>
    /// The rating scales for the account.
    /// </summary>
    public IDictionary<string, double> RatingScales { get; private set; }

    private static string DecodeName(string encodedName)
    {
      var encodedCharacters = encodedName.Split(new[] {'%'}, StringSplitOptions.RemoveEmptyEntries);
      var decodedCharacters = encodedCharacters.Select(c => Convert.ToByte(c, 16)).ToArray();
      return new UTF8Encoding().GetString(decodedCharacters);
    }

    private static DateTime GetExpireDate(XElement message)
    {
      var value = message.Attribute("expire").Value;
      var year = InvariantConvert.ToInt32(value.Substring(0, 4));
      var month = InvariantConvert.ToInt32(value.Substring(4, 2));
      var day = InvariantConvert.ToInt32(value.Substring(6, 2));
      return new DateTime(year, month, day);
    }

    private static int GetExpireDays(XElement message)
    {
      return InvariantConvert.ToInt32(message.Attribute("expiredays").Value);
    }

    private static Dictionary<string, double> GetRatingScales2(XElement message)
    {
      var entries = message.Attribute("ratingscale").Value.Split(new[] {"&"}, StringSplitOptions.RemoveEmptyEntries);
      var entryParts = entries.Select(entry => entry.Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries));
      return entryParts.ToDictionary(parts => parts[0], parts => InvariantConvert.ToDouble(parts[1]));
    }

    private static string GetUserName(XElement message)
    {
      return DecodeName(message.Attribute("uname").Value);
    }
  }
}