// Spines.Utility.Tests.ReadonlyListExtensionTests.cs
// 
// Copyright (C) 2017  Johannes Heckl
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

using NUnit.Framework;

namespace Spines.Utility.Tests
{
  [TestFixture]
  public class ReadOnlyListExtensionTests
  {
    [Test]
    public void SliceShouldContainCorrectElements()
    {
      var array = new[] {1, 2, 3, 4, 5};
      var expected = new[] {2, 3, 4};

      var actual = array.Slice(1, 3);

      CollectionAssert.AreEqual(expected, actual);
    }
  }
}