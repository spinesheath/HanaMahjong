// Spines.Mahjong.Analysis.ArrangementAnalyzer.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using Spines.Utility;

namespace Spines.Mahjong.Analysis.Combinations
{
  /// <summary>
  /// Calculates the shanten from 4 sets of Arrangements.
  /// </summary>
  public class ArrangementAnalyzer
  {
    private readonly IList<IEnumerable<Arrangement>> _sets = new List<IEnumerable<Arrangement>>();

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
          continue;
        mentsuCount += mentsuToAdd;
        // Worst case: tiles are spread evenly across the groups, with some groups having one more tile than the rest.
        mentsuValue += a.MentsuValue / a.MentsuCount * mentsuToAdd;
        mentsuValue += Math.Min(a.MentsuValue % a.MentsuCount, mentsuToAdd);
      }
      return mentsuValue + jantouValue;
    }
  }
}