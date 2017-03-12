﻿// Spines.Mahjong.Analysis.InternalTests.TileGroupAnalyzerTests.cs
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
using System.Linq;
using NUnit.Framework;
using Spines.Tools.AnalyzerBuilder.Combinations;

namespace Spines.Tools.AnalyzerBuilder.Tests
{
  [TestFixture]
  public class TileGroupAnalyzerTests
  {
    private readonly int[] _emptySuit = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private readonly int[] _emptyHonor = { 0, 0, 0, 0, 0, 0, 0 };

    [Test]
    public void TestEmptyHand()
    {
      CheckHand(1, _emptySuit, _emptySuit, 0);
    }

    [Test]
    public void TestSuitWithoutMelds()
    {
      CheckHand(2, new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0 }, _emptySuit, 0);
      CheckHand(1, new[] { 4, 4, 4, 0, 0, 0, 0, 0, 0 }, _emptySuit, 0);
      CheckHand(2, new[] { 3, 1, 1, 1, 1, 1, 1, 1, 3 }, _emptySuit, 0);
      CheckHand(1, new[] { 3, 1, 1, 1, 2, 1, 1, 1, 3 }, _emptySuit, 0);
      CheckHand(2, new[] { 4, 0, 0, 1, 0, 0, 1, 0, 1 }, _emptySuit, 0);
      CheckHand(2, new[] { 0, 0, 0, 4, 3, 0, 1, 0, 0 }, _emptySuit, 0);
    }

    [Test]
    public void TestSuitWithMelds()
    {
      CheckHand(1, new[] { 2, 0, 0, 0, 0, 0, 0, 0, 0 }, new[] { 0, 0, 0, 0, 4, 4, 4, 4, 0 }, 4);
      CheckHand(2, new[] { 4, 3, 0, 1, 0, 0, 0, 0, 0 }, new[] { 0, 0, 0, 0, 0, 0, 0, 4, 3 }, 2);
      CheckHand(2, new[] { 4, 2, 0, 1, 0, 0, 0, 0, 0 }, new[] { 0, 0, 0, 0, 0, 0, 0, 4, 3 }, 2);
      CheckHand(3, new[] { 4, 0, 3, 0, 0, 0, 0, 0, 0 }, new[] { 0, 0, 0, 0, 0, 0, 0, 4, 3 }, 2);
    }

    [Test]
    public void TestHonorsWithoutMelds()
    {
      CheckHand(2, new[] { 1, 0, 0, 0, 0, 0, 0 }, _emptyHonor, 0);
      CheckHand(1, new[] { 4, 4, 4, 0, 0, 0, 0 }, _emptyHonor, 0);
      CheckHand(4, new[] { 3, 1, 1, 1, 1, 1, 1 }, _emptyHonor, 0);
      CheckHand(4, new[] { 3, 1, 1, 1, 2, 1, 1 }, _emptyHonor, 0);
      CheckHand(3, new[] { 4, 0, 0, 1, 0, 0, 1 }, _emptyHonor, 0);
      CheckHand(1, new[] { 0, 0, 0, 4, 3, 0, 1 }, _emptyHonor, 0);
    }

    [Test]
    public void TestHonorsWithMelds()
    {
      CheckHand(1, new[] { 2, 0, 0, 0, 0, 0, 0 }, new[] { 0, 0, 4, 4, 4, 4, 0 }, 4);
      CheckHand(2, new[] { 4, 3, 0, 1, 0, 0, 0 }, new[] { 0, 0, 0, 0, 0, 4, 3 }, 2);
      CheckHand(2, new[] { 4, 2, 0, 1, 0, 0, 0 }, new[] { 0, 0, 0, 0, 0, 4, 3 }, 2);
      CheckHand(1, new[] { 4, 0, 3, 0, 0, 0, 0 }, new[] { 0, 0, 0, 0, 0, 4, 3 }, 2);
    }

    private static void CheckHand(int expectedCount, IList<int> concealedTiles, IList<int> meldedTiles, int meldCount)
    {
      var concealed = new Combination(concealedTiles);
      var melded = new Combination(meldedTiles);
      var analyzer = GetTileGroupAnalyzer(meldCount, concealed, melded);
      var arrangements = analyzer.Analyze().ToList();
      var hand = $"({string.Join("", concealedTiles)})({string.Join("", meldedTiles)})({meldCount})";
      var arrangementTexts = arrangements.Select(a => $"({a.JantouValue}, {a.MentsuCount}, {a.MentsuValue})");
      var message = $"hand {hand} has wrong arrangements: {string.Join("", arrangementTexts)}";
      Assert.AreEqual(expectedCount, arrangements.Count, message);
    }

    private static TileGroupAnalyzer GetTileGroupAnalyzer(int meldCount, Combination concealed, Combination melded)
    {
      if (concealed.Counts.Count == 9)
        return TileGroupAnalyzer.ForSuits(concealed, melded, meldCount);
      return TileGroupAnalyzer.ForHonors(concealed, melded, meldCount);
    }
  }
}