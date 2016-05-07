// Spines.Mahjong.Analysis.InternalTests.AnalyzerTests.cs
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
  public class AnalyzerTests
  {
    private readonly Combination _emptyCombination = new Combination(new[] {0, 0, 0, 0, 0, 0, 0, 0, 0});

    [TestMethod]
    public void TestEmptyHand()
    {
      CheckHandWithoutMelds(1, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 });
    }

    [TestMethod]
    public void TestHandsWithoutMelds()
    {
      CheckHandWithoutMelds(3, new[] {1, 0, 0, 0, 0, 0, 0, 0, 0});
      CheckHandWithoutMelds(3, new[] {2, 0, 0, 0, 0, 0, 0, 0, 0});
      CheckHandWithoutMelds(2, new[] {4, 4, 4, 0, 0, 0, 0, 0, 0});
    }

    private void CheckHandWithoutMelds(int expectedCount, IEnumerable<int> concealedTiles)
    {
      var concealed = new Combination(concealedTiles);
      var melded = _emptyCombination;
      var analyzer = new Analyzer(concealed, melded, 0);
      var arrangements = analyzer.Analyze();
      Assert.AreEqual(expectedCount, arrangements.Count(), "hand has wrong arrangement count");
    }
  }
}