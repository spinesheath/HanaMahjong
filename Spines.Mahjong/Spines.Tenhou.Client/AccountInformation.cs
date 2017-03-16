// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Spines.Utility;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Data provided by the server after logging on.
  /// </summary>
  public class AccountInformation
  {
    /// <summary>
    /// The username of the account that was logged on.
    /// </summary>
    public UserName UserName { get; }

    /// <summary>
    /// The date of expiry for the account.
    /// </summary>
    public DateTime ExpireDate { get; }

    /// <summary>
    /// The number of days until the account expires.
    /// </summary>
    public int ExpireDays { get; }

    /// <summary>
    /// The rating scales for the account.
    /// </summary>
    public IDictionary<string, double> RatingScales { get; }

    private static DateTime GetExpireDate(XElement message)
    {
      var value = message.Attribute("expire").Value;
      var year = InvariantConvert.ToInt32(value.Substring(0, 4));
      var month = InvariantConvert.ToInt32(value.Substring(4, 2));
      var day = InvariantConvert.ToInt32(value.Substring(6, 2));
      return new DateTime(year, month, day);
    }

    private static Dictionary<string, double> GetRatingScales(XElement message)
    {
      var entries = message.Attribute("ratingscale").Value.Split(new[] {"&"}, StringSplitOptions.RemoveEmptyEntries);
      var entryParts = entries.Select(entry => entry.Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries));
      return entryParts.ToDictionary(parts => parts[0], parts => InvariantConvert.ToDouble(parts[1]));
    }

    internal AccountInformation(XElement message)
    {
      RatingScales = GetRatingScales(message);
      ExpireDays = InvariantConvert.ToInt32(message.Attribute("expiredays").Value);
      ExpireDate = GetExpireDate(message);
      UserName = new UserName(message.Attribute("uname").Value);
    }

    internal XElement ToMessage()
    {
      var uname = new XAttribute("uname", UserName.EncodedName);
      var expire = new XAttribute("expire",
        InvariantConvert.Format("{0}{1}{2}", ExpireDate.Year, ExpireDate.Month, ExpireDate.Day));
      var days = new XAttribute("expiredays", ExpireDays);
      var scale = new XAttribute("ratingscale",
        string.Join("&", RatingScales.Select(p => InvariantConvert.Format("{0}={1}", p.Key, p.Value))));
      return new XElement("HELO", uname, expire, days, scale);
    }
  }
}