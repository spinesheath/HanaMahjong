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
    // 0: 1
    // 1: 5
    // 2: 25
    // 3: 85
    // 4: 255
    // 5: 649
    // 6: 1481
    // 7: 3042
    // 8: 5739
    // 9: 9987
    // 10: 16196
    // 11: 24551
    // 12: 34988
    // 13: 46976
    // 14: 59618
    // 15: 71606
    // 16: 81564
    // 17: 88138
    // 18: 90453
    // 19: 88138
    // 20: 81564

    [TestMethod]
    public void TestSuitCombinations()
    {
      var combinations = SuitCombinationFactory.CreateCombinations(5);
      Assert.AreEqual(649, combinations.Count(), "Count of combinations was wrong");
    }
  }
}
