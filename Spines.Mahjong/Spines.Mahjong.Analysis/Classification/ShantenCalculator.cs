// Spines.Mahjong.Analysis.ShantenCalculator.cs
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
using System.Linq;
using System.Text.RegularExpressions;
using Spines.Utility;

namespace Spines.Mahjong.Analysis.Classification
{
  internal class ShantenCalculator
  {
    public int Calculate(string hand)
    {
      var manzuRegex = new Regex(@"[\^\w](\d*)m");
      var pinzuRegex = new Regex(@"[\^\w](\d*)p");
      var souzuRegex = new Regex(@"[\^\w](\d*)s");
      var honorRegex = new Regex(@"[\^\w](\d*)z");
      var manzu = GetTileCounts(hand, manzuRegex, 9);
      var pinzu = GetTileCounts(hand, pinzuRegex, 9);
      var souzu = GetTileCounts(hand, souzuRegex, 9);
      var honor = GetTileCounts(hand, honorRegex, 7);

      var suitClassifier = new SuitClassifier();
      var honorClassifier = new HonorClassifier();
      var arrangementClassifier = new ArrangementClassifier();
      var shanten = arrangementClassifier.Classify(
        suitClassifier.Classify(manzu), 
        suitClassifier.Classify(pinzu),
        suitClassifier.Classify(souzu), 
        honorClassifier.Classify(honor));

      return shanten;
    }

    private static IReadOnlyList<int> GetTileCounts(string hand, Regex manzuRegex, int typesInSuit)
    {
      var capture = manzuRegex.Match(hand).Captures.OfType<Capture>().FirstOrDefault();
      if (capture == null)
      {
        return Enumerable.Repeat(0, typesInSuit + typesInSuit + 1).ToList();
      }
      var tiles = capture.Value.Select(c => char.GetNumericValue(c) - 1);
      var idToCount = tiles.GroupBy(t => t).ToDictionary(g => g.Key, g => g.Count());
      var tileCounts = Enumerable.Range(0, 9).Select(i => idToCount.ContainsKey(i) ? idToCount[i] : 0).ToList();

      var weight = GetWeight(typesInSuit, tileCounts);
      if (weight < 0)
        tileCounts.Reverse();

      return 0.Yield().Concat(Enumerable.Repeat(0, typesInSuit).Concat(tileCounts)).ToList();
    }

    private static int GetWeight(int typesInSuit, IReadOnlyList<int> concealed)
    {
      return Enumerable.Range(0, typesInSuit).Sum(i => GetWeight(i, typesInSuit, concealed));
    }

    private static int GetWeight(int tileTypeIndex, int typesInSuit, IReadOnlyList<int> concealed)
    {
      var tileCount = concealed[tileTypeIndex];
      var centerIndex = (typesInSuit - typesInSuit % 1) / 2;
      var shift = Math.Abs(centerIndex - tileTypeIndex) * 2;
      var factor = Math.Sign(centerIndex - tileTypeIndex);
      return (1 << shift) * tileCount * factor;
    }
  }
}