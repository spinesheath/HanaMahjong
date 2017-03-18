// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;
using Spines.Mahjong.Analysis.Classification;
using Spines.Utility;

namespace Spines.Mahjong.Analysis.InternalTests
{
  [TestFixture]
  public class KokushiClassifierTests
  {
    [Test]
    public void HandShouldHaveCorrectShanten()
    {
      var classifier = KokushiClassifier.Create();
      Repeat.Action(() => classifier.MoveNext(0), 13);
      Assert.That(classifier.Shanten, Is.EqualTo(1));
      classifier.MoveNext(1);
      Assert.That(classifier.Shanten, Is.EqualTo(0));
      classifier.MoveNext(2);
      Assert.That(classifier.Shanten, Is.EqualTo(1));
      classifier.MoveNext(1);
      Assert.That(classifier.Shanten, Is.EqualTo(1));
      classifier.MoveNext(3);
      Assert.That(classifier.Shanten, Is.EqualTo(1));
      classifier.MoveNext(3);
      Assert.That(classifier.Shanten, Is.EqualTo(2));
    }
  }
}