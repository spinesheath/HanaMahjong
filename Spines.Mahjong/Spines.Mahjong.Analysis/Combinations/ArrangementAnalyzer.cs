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
      var product = _sets.CartesianProduct();
      var maxValue = 0;
      foreach (var arrangements in product)
      {
        var mentsuCount = 0;
        var jantouValue = 0;
        var mentsuValue = 0;
        foreach (var arrangement in arrangements)
        {
          mentsuCount += arrangement.MentsuCount;
          jantouValue = Math.Max(jantouValue, arrangement.JantouValue);
          mentsuValue += arrangement.MentsuValue;
        }
        if (mentsuCount <= 4)
        {
          maxValue = Math.Max(maxValue, mentsuValue + jantouValue);
        }
      }
      return 13 - maxValue;
    }
  }
}