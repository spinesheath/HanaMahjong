﻿// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// Parses embedded resources.
  /// </summary>
  internal static class Resource
  {
    /// <summary>
    /// Loads the transition table from an embedded resource.
    /// </summary>
    public static ushort[] Transitions(string resourceName)
    {
      var fullResourceName = "Spines.Mahjong.Analysis.Resources." + resourceName;
      var assembly = Assembly.GetExecutingAssembly();
      Stream stream = null;
      try
      {
        stream = assembly.GetManifestResourceStream(fullResourceName);
        if (stream == null)
        {
          throw new MissingManifestResourceException("Arrangement classifier transition resource is missing.");
        }
        using (var reader = new StreamReader(stream))
        {
          stream = null;
          var result = reader.ReadToEnd();
          var lines = Regex.Split(result, "\r\n|\r|\n").Where(line => line.Length > 0);
          var ints = lines.Select(line => Convert.ToInt32(line, CultureInfo.InvariantCulture));
          return ints.Select(i => i < 0 ? 0 : i).Select(i => (ushort) i).ToArray();
        }
      }
      finally
      {
        stream?.Dispose();
      }
    }
  }
}