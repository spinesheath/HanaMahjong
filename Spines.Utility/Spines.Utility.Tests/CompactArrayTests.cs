// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

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