// Spines.Mahjong.Analysis.ArrangementClassifier.cs
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// Calculates the shanten of a set of four arrangements.
  /// </summary>
  internal class ArrangementClassifier
  {
    private static readonly int[] Transitions;

    /// <summary>
    /// Loads the transition data from application resources.
    /// </summary>
    static ArrangementClassifier()
    {
      var assembly = Assembly.GetExecutingAssembly();
      const string resourceName = "Spines.Mahjong.Analysis.Resources.ArrangementTransitions.txt";

      using (var stream = assembly.GetManifestResourceStream(resourceName))
      {
        if (stream == null)
        {
          throw new MissingManifestResourceException("Arrangement classifier transition resource is missing.");
        }
        using (var reader = new StreamReader(stream))
        {
          var result = reader.ReadToEnd();
          var lines = Regex.Split(result, "\r\n|\r|\n").Where(line => line.Length > 0);
          Transitions = lines.Select(line => int.Parse(line, CultureInfo.InvariantCulture)).ToArray();
        }
      }
    }

    /// <summary>
    /// Calculates the shanten of 4 arrangements.
    /// Behavior for invalid inputs is undefined.
    /// Input is invalid if there is no legal 14 tile hand that is represented by these arrangements.
    /// </summary>
    /// <param name="a1">Id of the first arrangement as used in the transition data.</param>
    /// <param name="a2">Id of the second arrangement as used in the transition data.</param>
    /// <param name="a3">Id of the third arrangement as used in the transition data.</param>
    /// <param name="a4">Id of the fourth arrangement as used in the transition data.</param>
    /// <returns>The shanten of the hand.</returns>
    public int Classify(int a1, int a2, int a3, int a4)
    {
      return Transitions[Transitions[Transitions[Transitions[0 + a1] + a2] + a3] + a4];
    }
  }
}