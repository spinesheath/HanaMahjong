/*
 *  Copyright (C) 2015  Johannes Heckl
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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spines.Tenhou.Client.InternalTests
{
  [TestClass]
  public class WallGeneratorTests
  {
    private const string Seed = "mt19937ar-sha512-n288-base64,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";

    // hai0="40,100,20,38,8,81,19,3,67,91,128,22,132"
    // hai1="83,28,129,55,107,52,112,14,98,76,63,7,99"
    // hai2="26,124,59,61,39,35,71,32,97,88,115,84,44"
    // hai3="126,105,127,45,80,116,43,82,130,74,101,78,121"
    private readonly int[] _tileIdsGame0 =
    {
      40, 100, 20, 38, 
      83, 28, 129, 55, 
      26, 124, 59, 61, 
      126, 105, 127, 45, 

      8, 81, 19, 3,
      107, 52, 112, 14, 
      39, 35, 71, 32, 
      80, 116, 43, 82, 

      67, 91, 128, 22, 
      98, 76, 63, 7, 
      97, 88, 115, 84, 
      130, 74, 101, 78,

      132,
      99,
      44,
      121
    };

    // rearranged because dealer is playerIndex 1
    // hai0="67,3,96,101,118,92,87,78,80,49,74,18,16"
    // hai1="107,54,64,20,75,43,123,51,44,23,15,89,127"
    // hai2="28,113,114,93,128,40,17,73,76,62,9,85,24"
    // hai3="5,12,58,106,21,102,83,129,132,36,108,47,56"
    private readonly int[] _tileIdsGame1 =
    {
      107, 54, 64, 20,
      28, 113, 114, 93,
      5, 12, 58, 106,
      67, 3, 96, 101,

      75, 43, 123, 51,
      128, 40, 17, 73,
      21, 102, 83, 129,
      118, 92, 87, 78,

      44, 23, 15, 89,
      76, 62, 9, 85,
      132, 36, 108, 47,
      80, 49, 74, 18,

      127,
      24,
      56,
      16
    };

    [TestMethod]
    public void TestWallGenerator()
    {
      var w = new WallGenerator(Seed);
      Assert.AreEqual(5, w.GetDice(0).ToList()[0], "dice 0");
      Assert.AreEqual(5, w.GetDice(0).ToList()[1], "dice 1");
      Assert.AreEqual(11, w.GetWall(0).ToList()[5], "dora 0");
      Assert.IsTrue(_tileIdsGame0.SequenceEqual(w.GetWall(0).Reverse().Take(13 * 4)), "wall");

      Assert.AreEqual(1, w.GetDice(1).ToList()[0], "dice 0");
      Assert.AreEqual(3, w.GetDice(1).ToList()[1], "dice 1");
      Assert.AreEqual(109, w.GetWall(1).ToList()[5], "dora 0");
      Assert.IsTrue(_tileIdsGame1.SequenceEqual(w.GetWall(1).Reverse().Take(13 * 4)), "wall");
    }
  }
}
