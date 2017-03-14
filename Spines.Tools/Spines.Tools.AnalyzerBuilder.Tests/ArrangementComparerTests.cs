// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;
using Spines.Tools.AnalyzerBuilder.Combinations;

namespace Spines.Tools.AnalyzerBuilder.Tests
{
  [TestFixture]
  public class ArrangementComparerTests
  {
    [Test]
    public void Test14Tiles()
    {
      var comparer = new ArrangementComparer();
      AssertIsWorse(comparer, new Arrangement(0, 2, 2), new Arrangement(0, 1, 2));
    }

    [Test]
    public void Test12Tiles()
    {
      var comparer = new ArrangementComparer();
      AssertIsWorse(comparer, new Arrangement(0, 4, 11), new Arrangement(0, 4, 12));
    }

    [Test]
    public void Test8Tiles()
    {
      var comparer = new ArrangementComparer();
      AssertIsWorse(comparer, new Arrangement(2, 0, 0), new Arrangement(2, 1, 3));
    }

    private static void AssertIsWorse(ArrangementComparer comparer, Arrangement a, Arrangement b)
    {
      Assert.IsTrue(comparer.IsWorseThan(a, b), $"{a} should be worse than {b}");
      Assert.IsFalse(comparer.IsWorseThan(b, a), $"{b} should not be worse than {a}");
    }
  }
}