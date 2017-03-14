// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using JetBrains.Annotations;

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
    /// Converts the value to a string using a template string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="format">The template of the result.</param>
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

    /// <summary>
    /// Creates a formatted string.
    /// </summary>
    /// <param name="template">The template of the string.</param>
    /// <param name="args">The values to replace the placeholders with.</param>
    /// <returns>The formatted string.</returns>
    [StringFormatMethod("template")]
    public static string Format(string template, params object[] args)
    {
      return string.Format(CultureInfo.InvariantCulture, template, args);
    }
  }
}