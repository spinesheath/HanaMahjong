// Spines.Utility.Tests.EnumerableExtensionsTests.cs
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

namespace Spines.Utility.Tests
{
  [TestFixture]
  public class EnumerableExtensionsTests
  {
    [Test]
    public void CartesianPoductShouldHaveCorrectCount()
    {
      var source = new List<IEnumerable<int>>
      {
        new List<int> { 0, 1, 2 },
        new List<int> { 3, 4, 5 },
        new List<int> { 6, 7, 8 }
      };
      const int expected = 3 * 3 * 3;

      var actual = source.CartesianProduct().Count();

      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void PermuteShouldHaveCorrectCount()
    {
      var source = new[] {0, 1, 2, 3};
      const int expected = 4 * 3 * 2 * 1;

      var actual = source.Permute().Count();

      Assert.That(actual, Is.EqualTo(expected));
    }
  }
}