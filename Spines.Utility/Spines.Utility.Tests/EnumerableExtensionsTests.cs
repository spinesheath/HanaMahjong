// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Spines.Utility.Tests
{
  [TestFixture]
  public class EnumerableExtensionsTests
  {
    [Test]
    public void CartesianProductShouldHaveCorrectCount()
    {
      var source = new List<IEnumerable<int>>
      {
        new List<int> {0, 1, 2},
        new List<int> {3, 4, 5},
        new List<int> {6, 7, 8}
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