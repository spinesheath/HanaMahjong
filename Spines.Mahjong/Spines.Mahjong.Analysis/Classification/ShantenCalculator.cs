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
      var manzu = GetTileCounts(hand, 'm', 9);
      var pinzu = GetTileCounts(hand, 'p', 9);
      var souzu = GetTileCounts(hand, 's', 9);
      var honor = GetTileCounts(hand, 'z', 7);

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

    private static IReadOnlyList<int> GetTileCounts(string hand, char tileGroupName, int typesInSuit)
    {
      var concealed = GetTiles(hand, tileGroupName, typesInSuit).ToList();
      var meldedName = char.ToUpper(tileGroupName);
      var meldCount = hand.Count(c => c == meldedName);
      var melded = GetTiles(hand, meldedName, typesInSuit).ToList();

      var weight = GetWeight(typesInSuit, concealed, melded);
      if (weight < 0)
      {
        concealed.Reverse();
        melded.Reverse();
      }

      return meldCount.Yield().Concat(melded).Concat(concealed).ToList();
    }

    private static IEnumerable<int> GetTiles(string hand, char tileGroupName, int typesInSuit)
    {
      var regex = new Regex(@"[\^\w](\d*)" + tileGroupName);
      var captures = regex.Matches(hand).OfType<Match>().SelectMany(m => m.Captures.OfType<Capture>());
      var tiles = captures.SelectMany(c => c.Value).Select(c => (int) char.GetNumericValue(c) - 1);
      var idToCount = tiles.GroupBy(t => t).ToDictionary(g => g.Key, g => g.Count());
      return Enumerable.Range(0, typesInSuit).Select(i => idToCount.ContainsKey(i) ? idToCount[i] : 0);
    }

    private static int GetWeight(int typesInSuit, IReadOnlyList<int> concealed, IReadOnlyList<int> melded)
    {
      return Enumerable.Range(0, typesInSuit).Sum(i => GetWeight(i, typesInSuit, concealed, melded));
    }

    private static int GetWeight(int tileTypeIndex, int typesInSuit, IReadOnlyList<int> concealed,
      IReadOnlyList<int> melded)
    {
      var tileCount = concealed[tileTypeIndex] + melded[tileTypeIndex];
      var centerIndex = (typesInSuit - typesInSuit % 1) / 2;
      var shift = Math.Abs(centerIndex - tileTypeIndex) * 2;
      var factor = Math.Sign(centerIndex - tileTypeIndex);
      return (1 << shift) * tileCount * factor;
    }
  }
}