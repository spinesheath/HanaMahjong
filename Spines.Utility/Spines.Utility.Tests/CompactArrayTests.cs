// Spines.Utility.Tests.CompactArrayTests.cs
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
  public class CompactArrayTests
  {
    [Test]
    public void StoredValueShouldBeRecovered()
    {
      var s = new[]
      {
        153088,
        152803,
        153033,
        152148,
        152153,
        153098,
        153293
      };

      var ca = new CompactArray(s);
      var actual = new int[s.Length];
      for (var i = 0; i < s.Length; ++i)
      {
        actual[i] = ca[i];
      }

      CollectionAssert.AreEqual(s, actual);
    }
  }
}