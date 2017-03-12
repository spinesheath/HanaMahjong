// Spines.Mahjong.Analysis.Resource.cs
// 
// Copyright (C) 2017  Johannes Heckl
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
using System.Collections.Generic;
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
    public static IEnumerable<ushort> Transitions(string resourceName)
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
          return ints.Select(i => i < 0 ? 0 : i).Select(i => (ushort)i);
        }
      }
      finally
      {
        stream?.Dispose();
      }
    }
  }
}