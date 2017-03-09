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
using NUnit.Framework;
using Spines.Mahjong.Analysis.Classification;
using Spines.Mahjong.Analysis.Combinations;
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
    public void CalculateShouldBeCorrect(string hand, int expected)
    {
      var c = new ShantenCalculator();

      var actual = c.Calculate(hand);

      Assert.That(actual, Is.EqualTo(expected));
    }

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

      var actual = c.Calculate2(hand);

      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void CalculateShouldBeFast()
    {
      var emptyMelds = new int[0];
      var emptySuit = new int[9];
      var emptyHonor = new int[15];
      var classifier = new Classifier();
      foreach (var hand in GetChinitsuHands().Take(1000))
      {
        var shanten = classifier.ClassifyArrangements(
          classifier.ClassifySuits(hand.Item1, hand.Item2),
          classifier.ClassifySuits(emptyMelds, emptySuit),
          classifier.ClassifySuits(emptyMelds, emptySuit),
          classifier.ClassifyHonors(emptyHonor));
        Assert.That(shanten, Is.GreaterThan(-2));
      }
    }

    private static IEnumerable<Tuple<int[], int[]>> GetChinitsuHands()
    {
      foreach (var meldCount in Enumerable.Range(0, 5))
      {
        var baseLanguage = Enumerable.Repeat(Enumerable.Range(0, 25), meldCount).CartesianProduct();
        foreach (var w in baseLanguage)
        {
          var meldWord = w.ToArray();
          var oldWord = new int[9];
          foreach (var c in meldWord)
          {
            if (c < 7)
            {
              oldWord[c + 0] += 1;
              oldWord[c + 1] += 1;
              oldWord[c + 2] += 1;
            }
            else if (c < 16)
            {
              oldWord[c - 7] += 3;
            }
            else if (c < 25)
            {
              oldWord[c - 16] += 4;
            }
          }
          if (oldWord.Any(c => c > 4))
          {
            continue;
          }

          var concealeds = ConcealedCombinationCreator.ForSuits().Create(13 - meldCount * 3, new Combination(oldWord));
          foreach (var concealed in concealeds)
          {
            yield return Tuple.Create(meldWord, concealed.Counts.ToArray());
          }
        }
      }
    }
  }
}