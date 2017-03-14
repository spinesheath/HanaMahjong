// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Spines.Utility;

namespace Spines.Tools.AnalyzerBuilder.Combinations
{
  /// <summary>
  /// Calculates the shanten from 4 sets of Arrangements.
  /// </summary>
  internal class ArrangementAnalyzer
  {
    /// <summary>
    /// Adds a set of arrangements to the analyzer. Usually there will be at most 4 sets.
    /// </summary>
    public void AddSetOfArrangements(IEnumerable<Arrangement> arrangements)
    {
      var list = Validate.NotNull(arrangements, nameof(arrangements)).ToList();
      _sets.Add(list);
    }

    /// <summary>
    /// Calculates the shanten value from the current sets of arrangements.
    /// </summary>
    public int CalculateShanten()
    {
      var product = _sets.CartesianProduct().SelectMany(p => p.Permute());
      var maxValue = product.Select(SumUsefulTiles).Max();
      return 13 - maxValue;
    }

    private readonly IList<IEnumerable<Arrangement>> _sets = new List<IEnumerable<Arrangement>>();

    private static int SumUsefulTiles(IEnumerable<Arrangement> arrangements)
    {
      var mentsuCount = 0;
      var jantouValue = 0;
      var mentsuValue = 0;
      foreach (var a in arrangements)
      {
        jantouValue = Math.Max(jantouValue, a.JantouValue);
        var mentsuToAdd = Math.Min(4 - mentsuCount, a.MentsuCount);
        if (mentsuToAdd == 0)
        {
          continue;
        }
        mentsuCount += mentsuToAdd;
        // Worst case: tiles are spread evenly across the groups, with some groups having one more tile than the rest.
        mentsuValue += a.MentsuValue / a.MentsuCount * mentsuToAdd;
        mentsuValue += Math.Min(a.MentsuValue % a.MentsuCount, mentsuToAdd);
      }
      return mentsuValue + jantouValue;
    }
  }
}