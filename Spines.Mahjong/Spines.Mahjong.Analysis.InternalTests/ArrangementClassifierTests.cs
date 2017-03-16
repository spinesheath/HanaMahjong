// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Mahjong.Analysis.InternalTests
{
  [TestFixture]
  public class ArrangementClassifierTests
  {
    [Test]
    public void ClassifyShouldReturnValueOfWord()
    {
      var a = ArrangementClassifier.Classify(new[] { 0, 0, 0, 19 });
      const int e = 9;

      Assert.That(a, Is.EqualTo(e));
    }
  }
}