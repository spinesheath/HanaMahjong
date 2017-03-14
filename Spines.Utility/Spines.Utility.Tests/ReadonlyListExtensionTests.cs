// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

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