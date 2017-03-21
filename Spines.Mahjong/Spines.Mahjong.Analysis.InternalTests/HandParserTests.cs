// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using NUnit.Framework;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Mahjong.Analysis.InternalTests
{
  [TestFixture]
  public class HandParserTests
  {
    [TestCase("1m")]
    [TestCase("123456789m5s555P")]
    [TestCase("123456789m5s'555P")]
    [TestCase("123456789m5s5''55P")]
    [TestCase("123456789m5s5'5'5P")]
    [TestCase("123456789m5s146P")]
    [TestCase("123456789m5s1'46P")]
    [TestCase("123456789m5s0055P")]
    public void HandShouldThrowIfInvalid(string hand)
    {
      Assert.Throws<FormatException>(() => HandParser.Parse(hand));
    }

    [TestCase("123456789m5123s", 13)]
    [TestCase("123456789m5123s5p", 13)]
    [TestCase("123456789m5s5'55P", 10)]
    [TestCase("123456789m5s5'5'55P", 10)]
    public void HandShouldHaveCorrectNumberOfTiles(string hand, int expected)
    {
      var actual = HandParser.Parse(hand).Tiles.Count();
      Assert.That(actual, Is.EqualTo(expected));
    }
  }
}