// Spines.Utility.InvariantConvert.cs
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

namespace Spines.Utility
{
  /// <summary>
  /// Conversion methods using invariant culture.
  /// </summary>
  public static class InvariantConvert
  {
    /// <summary>
    /// Converts the hexadecimal value to an integer.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted value.</returns>
    public static int HexToInt32(string value)
    {
      return int.Parse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts the value to a double.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted value.</returns>
    public static double ToDouble(string value)
    {
      return Convert.ToDouble(value, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts the value to an integer.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted value.</returns>
    public static int ToInt32(string value)
    {
      return Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts the value to a string using a format string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="format">The format of the result.</param>
    /// <returns>The converted value.</returns>
    public static string ToString(int value, string format)
    {
      return value.ToString(format, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts the value to a string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted value.</returns>
    public static string ToString(int value)
    {
      return value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts the value to a decimal.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted value.</returns>
    public static decimal ToDecimal(string value)
    {
      return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
    }
  }
}