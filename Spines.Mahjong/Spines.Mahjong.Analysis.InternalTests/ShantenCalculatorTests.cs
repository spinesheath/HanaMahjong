// Spines.Mahjong.Analysis.InternalTests.ShantenCalculatorTests.cs
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
using System.Text;
using NUnit.Framework;
using Spines.Mahjong.Analysis.Classification;
using Spines.Utility;

namespace Spines.Mahjong.Analysis.InternalTests
{
  [TestFixture]
  public class ShantenCalculatorTests
  {
    [TestCase("123456789m12344p", -1)]
    [TestCase("123456789m1234p", 0)]
    [TestCase("123456789m1245p", 1)]
    [TestCase("123456789m147p1s", 2)]
    [TestCase("12345679m147p14s", 3)]
    [TestCase("1345679m147p147s", 4)]
    [TestCase("145679m147p147s1z", 5)]
    [TestCase("14679m147p147s12z", 6)]
    [TestCase("1479m147p147s123z", 7)]
    [TestCase("147m147p147s1234z", 8)]
    [TestCase("123456789m44p123S", -1)]
    [TestCase("1245p112z333P6666P", 2)]
    public void Calculate2ShouldBeCorrect(string hand, int expected)
    {
      var c = new ShantenCalculator();

      var actual = c.Calculate(hand);

      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void HandShouldWork()
    {
      var hand = new Hand(1);
      while (hand.DrawRandom())
      {
        if (!hand.DiscardBest())
        {
          break;
        }
      }
      Assert.That(hand, Is.Not.Null);
    }

    [Test]
    public void ProgressiveCalculationShouldWork()
    {
      var rand = new Random(1);
      var classifier = new Classifier();

      var concealed = new int[34];
      var melded = new int[34];
      var discarded = new int[34];
      var allDrawn = new bool[136];
      const int meldCount = 0;
      var shanten = 10;
      var emptyMelds = new int[0];
      var actualDraws = 0;
      for (var i = 0; i < 10000; ++i)
      {
        if (shanten == -1 || allDrawn.All(x => x))
        {
          concealed.Populate(0);
          melded.Populate(0);
          discarded.Populate(0);
          shanten = 10;
          allDrawn.Populate(false);
        }

        var s = rand.Next(136);
        if (allDrawn[s])
        {
          continue;
        }
        allDrawn[s] = true;
        var t = s / 4;
        var draw = ToTile(t);
        if (concealed[t] + melded[t] + discarded[t] == 4)
        {
          continue;
        }
        concealed[t] += 1;
        
        actualDraws += 1;
        if (concealed.Sum() + meldCount * 3 < 14)
        {
          continue;
        }

        var curHand = ToHand(concealed);

        shanten = classifier.ClassifyArrangements(
            classifier.ClassifySuits(emptyMelds, concealed.Slice(0, 9)),
            classifier.ClassifySuits(emptyMelds, concealed.Slice(9, 9)),
            classifier.ClassifySuits(emptyMelds, concealed.Slice(18, 9)),
            classifier.ClassifyHonors(Enumerable.Repeat(0, 8).Concat(concealed.Slice(27, 7).OrderByDescending(x => x)).ToList()));

        if (shanten == -1)
        {
          continue;
        }

        var ukeIreCount = -1;
        var bestDiscard = -1;
        var bestShanten = 10;
        var bestUkeIre = new int[34];
        for (var j = 0; j < 34; ++j)
        {
          if (concealed[j] == 0)
          {
            continue;
          }
          concealed[j] -= 1;
          discarded[j] += 1;

          var handAfterDiscard = ToHand(concealed);

          var s1 = classifier.ClassifyArrangements(
            classifier.ClassifySuits(emptyMelds, concealed.Slice(0, 9)),
            classifier.ClassifySuits(emptyMelds, concealed.Slice(9, 9)),
            classifier.ClassifySuits(emptyMelds, concealed.Slice(18, 9)),
            classifier.ClassifyHonors(Enumerable.Repeat(0, 8).Concat(concealed.Slice(27, 7).OrderByDescending(x => x)).ToList()));

          if (bestShanten < 10)
          {
            Assert.That(s1, Is.AtLeast(shanten));
            Assert.That(s1, Is.AtMost(shanten + 1));
          }

          if (s1 > bestShanten)
          {
            concealed[j] += 1;
            discarded[j] -= 1;
            continue;
          }

          var ukeIre = new int[34];
          var u = 0;
          for (var k = 0; k < 34; ++k)
          {
            var used = concealed[k] + melded[k] + discarded[k];
            if (used == 4)
            {
              continue;
            }
            concealed[k] += 1;

            var handAfterDraw = ToHand(concealed);

            var s2 = classifier.ClassifyArrangements(
              classifier.ClassifySuits(emptyMelds, concealed.Slice(0, 9)),
              classifier.ClassifySuits(emptyMelds, concealed.Slice(9, 9)),
              classifier.ClassifySuits(emptyMelds, concealed.Slice(18, 9)),
              classifier.ClassifyHonors(Enumerable.Repeat(0, 8).Concat(concealed.Slice(27, 7).OrderByDescending(x => x)).ToList()));

            if (bestShanten < 10)
            {
              Assert.That(s2, Is.AtLeast(s1 - 1));
              Assert.That(s2, Is.AtMost(s1));
            }

            concealed[k] -= 1;

            if (s2 < shanten)
            {
              ukeIre[k] = 1;
              u += 4 - used;
            }
          }

          if (u > ukeIreCount || s1 < bestShanten)
          {
            Assert.That(s1, Is.AtMost(bestShanten));

            bestUkeIre = ukeIre;
            ukeIreCount = u;
            bestDiscard = j;
            bestShanten = s1;
          }

          concealed[j] += 1;
          discarded[j] -= 1;
        }

        var discard = ToTile(bestDiscard);
        var ukeIreTiles = ToHand(bestUkeIre);

        concealed[bestDiscard] -= 1;
        discarded[bestDiscard] += 1;
        shanten = bestShanten;
      }
      Assert.That(actualDraws, Is.Not.Zero);
    }

    private static string ToTile(int tile)
    {
      var i = tile / 9;
      var c = tile % 9;
      return (char)('1' + c) + "mpsz".Substring(i, 1);
    }

    private static string ToHand(IReadOnlyList<int> concealed)
    {
      return ToHand(concealed.Slice(0, 9), 'm') +
             ToHand(concealed.Slice(9, 9), 'p') +
             ToHand(concealed.Slice(18, 9), 's') +
             ToHand(concealed.Slice(27, 7), 'z');
    }

    private static string ToHand(IReadOnlyList<int> slice, char suit)
    {
      var sb = new StringBuilder();
      for (var i = 0; i < slice.Count; ++i)
      {
        for (var j = 0; j < slice[i]; ++j)
        {
          sb.Append((char)('1' + i));
        }
      }
      sb.Append(suit);
      return sb.ToString();
    }
  }
}