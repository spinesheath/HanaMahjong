// Spines.Mahjong.Analysis.InternalTests.SuitAnalyzerTests.cs
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spines.Mahjong.Analysis.Combinations;

namespace Spines.Mahjong.Analysis.InternalTests
{
  [TestClass]
  public class SuitAnalyzerTests
  {
    private readonly int[] _emptyCombination = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    [TestMethod]
    public void TestEmptyHand()
    {
      CheckHand(1, _emptyCombination, _emptyCombination, 0);
    }

    [TestMethod]
    public void TestHandsWithoutMelds()
    {
      //CheckHand(3, new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0 }, _emptyCombination, 0);
      //CheckHand(3, new[] { 2, 0, 0, 0, 0, 0, 0, 0, 0 }, _emptyCombination, 0);
      CheckHand(2, new[] { 4, 4, 4, 0, 0, 0, 0, 0, 0 }, _emptyCombination, 0);
      //CheckHand(2, new[] { 3, 1, 1, 1, 1, 1, 1, 1, 3 }, _emptyCombination, 0);
      //CheckHand(1, new[] { 3, 1, 1, 1, 2, 1, 1, 1, 3 }, _emptyCombination, 0);
      //CheckHand(8, new[] { 4, 0, 0, 1, 0, 0, 1, 0, 1 }, _emptyCombination, 0);
    }

    [TestMethod]
    public void TestHandsWithMelds()
    {
      CheckHand(1, new[] { 2, 0, 0, 0, 0, 0, 0, 0, 0 }, new[] { 0, 0, 0, 0, 4, 4, 4, 4, 0 }, 4);
    }

    private static void CheckHand(int expectedCount, IEnumerable<int> concealedTiles, IEnumerable<int> meldedTiles, int meldCount)
    {
      var concealed = new Combination(concealedTiles);
      var melded = new Combination(meldedTiles);
      var analyzer = new SuitAnalyzer(concealed, melded, meldCount);
      var arrangements = analyzer.Analyze();
      Assert.AreEqual(expectedCount, arrangements.Count(), "hand has wrong arrangement count");
    }
  }
}