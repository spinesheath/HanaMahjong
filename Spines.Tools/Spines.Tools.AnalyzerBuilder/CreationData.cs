// Spines.Tools.AnalyzerBuilder.CreationData.cs
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

using System.Collections.Generic;

namespace Spines.Tools.AnalyzerBuilder
{
  internal static class CreationData
  {
    public static readonly IDictionary<CreationType, int> CreationCounts = new Dictionary<CreationType, int>
    {
      {CreationType.ConcealedSuit, 15},
      {CreationType.MeldedSuit, 5},
      {CreationType.MixedSuit, 15},
      {CreationType.AnalyzedSuit, 15},
      {CreationType.ArrangementCsv, 15},
      {CreationType.AnalyzedHonors, 15}
    };

    public static readonly IDictionary<CreationType, string> Prefixes = new Dictionary<CreationType, string>
    {
      {CreationType.ConcealedSuit, "ConcealedSuitCombinations"},
      {CreationType.MeldedSuit, "MeldedSuitCombinations"},
      {CreationType.MixedSuit, "MixedSuitCombinations"},
      {CreationType.AnalyzedSuit, "AnayzedSuitCombinations"},
      {CreationType.ArrangementCsv, "ArrangementCsv"},
      {CreationType.AnalyzedHonors, "AnayzedHonorCombinations"}
    };

    public static readonly IDictionary<CreationType, string> FileTypes = new Dictionary<CreationType, string>
    {
      {CreationType.ConcealedSuit, "txt"},
      {CreationType.MeldedSuit, "txt"},
      {CreationType.MixedSuit, "txt"},
      {CreationType.AnalyzedSuit, "txt"},
      {CreationType.ArrangementCsv, "csv"},
      {CreationType.AnalyzedHonors, "txt"}
    };
  }
}