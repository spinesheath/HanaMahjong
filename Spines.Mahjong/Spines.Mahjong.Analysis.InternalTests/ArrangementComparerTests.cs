﻿// Spines.Mahjong.Analysis.InternalTests.ArrangementComparerTests.cs
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spines.Mahjong.Analysis.Combinations;

namespace Spines.Mahjong.Analysis.InternalTests
{
  [TestClass]
  public class ArrangementComparerTests
  {
    [TestMethod]
    public void Test14Tiles()
    {
      var comparer = new ArrangementComparer(14);
      AssertIsWorse(comparer, new Arrangement(0, 1, 2), new Arrangement(0, 3, 3));
      AssertIsWorse(comparer, new Arrangement(0, 2, 2), new Arrangement(0, 1, 2));
      AssertIsWorse(comparer, new Arrangement(2, 0, 0), new Arrangement(1, 1, 1));
    }

    [TestMethod]
    public void Test12Tiles()
    {
      var comparer = new ArrangementComparer(12);
      AssertIsWorse(comparer, new Arrangement(0, 4, 11), new Arrangement(0, 4, 12));
    }

    [TestMethod]
    public void Test8Tiles()
    {
      var comparer = new ArrangementComparer(8);
      AssertIsWorse(comparer, new Arrangement(2, 0, 0), new Arrangement(2, 1, 3));
    }

    private static void AssertIsWorse(ArrangementComparer comparer, Arrangement a, Arrangement b)
    {
      Assert.IsTrue(comparer.IsWorseThan(a, b), $"{a} should be worse than {b}");
      Assert.IsFalse(comparer.IsWorseThan(b, a), $"{b} should not be worse than {a}");
    }
  }
}