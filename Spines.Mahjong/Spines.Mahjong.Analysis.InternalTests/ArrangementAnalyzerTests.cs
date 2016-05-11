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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spines.Mahjong.Analysis.Combinations;
using Spines.Utility;

namespace Spines.Mahjong.Analysis.InternalTests
{
  [TestClass]
  public class ArrangementAnalyzerTests
  {
    [TestMethod]
    public void TestArrangementAnalyzer()
    {
      // Randomly picked 2 shanten hand. 34467p 44446s 3447m
      var pinzu = new[] // 001201100 (0,3,5)(1,2,4)(2,0,0)(2,1,2)(2,2,3)
      {
        new Arrangement(0, 3, 5),
        new Arrangement(1, 2, 4), // 2 incomplete mentsu
        new Arrangement(2, 0, 0),
        new Arrangement(2, 1, 2),
        new Arrangement(2, 2, 3) 
      };
      var souzu = new[] // 000401000 (0,2,5)(1,1,3)(2,0,0)(2,1,2)
      {
        new Arrangement(0, 2, 5), // 1 incomplete and 1 complete mentsu
        new Arrangement(1, 1, 3),
        new Arrangement(2, 0, 0),
        new Arrangement(2, 1, 2)
      };
      var manzu = new[] // 001200100 (0,3,4)(1,1,2)(1,2,3)(2,0,0)(2,1,1)(2,2,2)
      {
        new Arrangement(0, 3, 4),
        new Arrangement(1, 1, 2), 
        new Arrangement(1, 2, 3),
        new Arrangement(2, 0, 0), // 1 complete pair
        new Arrangement(2, 1, 1),
        new Arrangement(2, 2, 2)
      };
      var honor = new Arrangement(0, 0, 0).Yield();

      var analyzer = new ArrangementAnalyzer();
      analyzer.AddSetOfArrangements(pinzu);
      analyzer.AddSetOfArrangements(souzu);
      analyzer.AddSetOfArrangements(manzu);
      analyzer.AddSetOfArrangements(honor);
      var shanten = analyzer.CalculateShanten();
      Assert.AreEqual(2, shanten);
    }
  }
}
