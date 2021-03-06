﻿// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace Spines.Hana.Clay.Converters
{
  /// <summary>
  /// Negates a boolean value.
  /// </summary>
  /// <remarks>Used in XAML.</remarks>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
  internal class InvertConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool)
      {
        return !(bool) value;
      }
      throw new ArgumentException("Can only invert bools.", nameof(value));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool)
      {
        return !(bool) value;
      }
      throw new ArgumentException("Can only invert bools.", nameof(value));
    }
  }
}