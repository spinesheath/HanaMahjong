﻿// Spines.Mahjong.Analysis.Classifier.cs
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
  /// Calculates the shanten of a set of four arrangements.
  /// </summary>
  internal class Classifier
  {
    private static readonly int[] ArrangementTransitions = GetTransitions("ArrangementTransitions.txt");
    private static readonly int[] SuitTransitions = GetTransitions("SuitTransitions.txt");
    private static readonly int[] HonorTransitions = GetTransitions("HonorTransitions.txt");

    private static readonly int[] SuitFirstPhase = GetTransitions("SuitFirstPhase.txt");
    private static readonly int[] SuitSecondPhase0 = GetTransitions("SuitSecondPhase0.txt");
    private static readonly int[] SuitSecondPhase1 = GetTransitions("SuitSecondPhase1.txt");
    private static readonly int[] SuitSecondPhase2 = GetTransitions("SuitSecondPhase2.txt");
    private static readonly int[] SuitSecondPhase3 = GetTransitions("SuitSecondPhase3.txt");
    private static readonly int[] SuitSecondPhase4 = GetTransitions("SuitSecondPhase4.txt");
    private static readonly int[][] SuitSecondPhases = {SuitSecondPhase0, SuitSecondPhase1, SuitSecondPhase2, SuitSecondPhase3, SuitSecondPhase4};

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
    public int ClassifyArrangements(int a1, int a2, int a3, int a4)
    {
      return ArrangementTransitions[ArrangementTransitions[ArrangementTransitions[ArrangementTransitions[0 + a1] + a2] + a3] + a4];
    }

    /// <summary>
    /// Returns the Arrangement for a Suit. The word is the sequence
    /// (meld count, melded tile counts, concealed tile counts)
    /// </summary>
    public int ClassifySuits(IReadOnlyList<int> melds, IReadOnlyList<int> concealed)
    {
      var secondPhase = SuitSecondPhases[melds.Count];
      var current = 0;
      for (var i = 0; i < melds.Count; ++i)
      {
        current = SuitFirstPhase[current + melds[i] + 1];
      }
      current = SuitFirstPhase[current];
      for (var i = 0; i < concealed.Count; ++i)
      {
        current = secondPhase[current + concealed[i]];
      }
      return current;
    }

    /// <summary>
    /// Returns the Arrangement for a Suit. The word is the sequence
    /// (meld count, melded tile counts, concealed tile counts)
    /// </summary>
    public int ClassifySuits(IReadOnlyList<int> word)
    {
      var current = 0;
      for (var i = 0; i < word.Count; ++i)
      {
        current = SuitTransitions[current + word[i]];
      }
      return current;
    }

    /// <summary>
    /// Returns the Arrangement for Honors. The word is the sequence
    /// (meld count, melded tile counts, concealed tile counts)
    /// </summary>
    public int ClassifyHonors(IReadOnlyList<int> word)
    {
      var current = 0;
      for (var i = 0; i < word.Count; ++i)
      {
        current = HonorTransitions[current + word[i]];
      }
      return current;
    }

    /// <summary>
    /// Loads the transition table from an embedded resource.
    /// </summary>
    private static int[] GetTransitions(string resourceName)
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