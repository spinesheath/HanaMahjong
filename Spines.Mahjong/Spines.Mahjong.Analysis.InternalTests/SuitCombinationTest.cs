/*
 *  Copyright (C) 2016  Johannes Heckl
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spines.Mahjong.Analysis.InternalTests
{
  [TestClass]
  public class SuitCombinationTest
  {
    /// <summary>
    /// Number of combinations of melds in a single suit.
    /// </summary>
    [TestMethod]
    public void TestMeldCombinations()
    {
      VerifyMeldCombinationCount(0, 1);
      VerifyMeldCombinationCount(1, 25);
      VerifyMeldCombinationCount(2, 284);
      VerifyMeldCombinationCount(3, 1914);
      VerifyMeldCombinationCount(4, 8439);
    }

    private static void VerifyMeldCombinationCount(int numberOfMelds, int numberOfCombinations)
    {
      var combinations = SuitCombinationFactory.CreateMeldedCombinations(numberOfMelds);
      Assert.AreEqual(numberOfCombinations, combinations.Count(), "Count of meld combinations was wrong");
    }

    /// <summary>
    /// Number of combinations of tiles in a single suit, no melds.
    /// </summary>
    [TestMethod]
    public void TestSuitCombinations()
    {
      VerifyCombinationCount(0, 1);
      VerifyCombinationCount(1, 5);
      VerifyCombinationCount(2, 25);
      VerifyCombinationCount(3, 85);
      VerifyCombinationCount(4, 255);
      //VerifyCombinationCount(5, 649);
      //VerifyCombinationCount(6, 1481);
      //VerifyCombinationCount(7, 3042);
      //VerifyCombinationCount(8, 5739);
      //VerifyCombinationCount(9, 9987);
      //VerifyCombinationCount(10, 16196);
      //VerifyCombinationCount(11, 24551);
      //VerifyCombinationCount(12, 34988);
      //VerifyCombinationCount(13, 46976);
      //VerifyCombinationCount(14, 59618);
      //VerifyCombinationCount(15, 71606);
      //VerifyCombinationCount(16, 81564);
      //VerifyCombinationCount(17, 88138);
      //VerifyCombinationCount(18, 90453);
      //VerifyCombinationCount(19, 88138);
      //VerifyCombinationCount(20, 81564);
    }

    private static void VerifyCombinationCount(int numberOfTiles, int numberOfCombinations)
    {
      var combinations = SuitCombinationFactory.CreateCombinations(numberOfTiles);
      Assert.AreEqual(numberOfCombinations, combinations.Count(), "Count of combinations was wrong");
    }
  }
}
