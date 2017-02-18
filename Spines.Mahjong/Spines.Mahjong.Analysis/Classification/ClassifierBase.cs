// Spines.Mahjong.Analysis.Classifier.cs
// 
// Copyright (C) 2016  Johannes Heckl
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

using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// Classifies words of a language with equal-length words.
  /// </summary>
  internal abstract class ClassifierBase
  {
    protected static int[] GetTransitions(string resourceName)
    {
      var assembly = Assembly.GetExecutingAssembly();
      Stream stream = null;
      try
      {
        stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
          throw new MissingManifestResourceException("Arrangement classifier transition resource is missing.");
        }
        using (var reader = new StreamReader(stream))
        {
          stream = null;
          var result = reader.ReadToEnd();
          var lines = Regex.Split(result, "\r\n|\r|\n").Where(line => line.Length > 0);
          return lines.Select(line => int.Parse(line, CultureInfo.InvariantCulture)).ToArray();
        }
      }
      finally
      {
        stream?.Dispose();
      }
    }
  }
}