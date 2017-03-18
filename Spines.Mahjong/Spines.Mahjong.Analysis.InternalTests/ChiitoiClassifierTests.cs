// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;
using Spines.Mahjong.Analysis.Classification;
using Spines.Utility;

namespace Spines.Mahjong.Analysis.InternalTests
{
  [TestFixture]
  public class ChiitoiClassifierTests
  {
    [Test]
    public void HandShouldHaveCorrectShanten()
    {
      var classifier = ChiitoiClassifier.Create();
      Repeat.Action(() => classifier.Draw(0), 13);
      Assert.That(classifier.Shanten, Is.EqualTo(7));
      classifier.Draw(1);
      Assert.That(classifier.Shanten, Is.EqualTo(6));
      classifier.Discard(1);
      Assert.That(classifier.Shanten, Is.EqualTo(6));
      classifier.Draw(1);
      Assert.That(classifier.Shanten, Is.EqualTo(5));
      classifier.Discard(2);
      Assert.That(classifier.Shanten, Is.EqualTo(6));
      classifier.Draw(0);
      classifier.Discard(2);
      Assert.That(classifier.Shanten, Is.EqualTo(7));
      classifier.Draw(1);
      classifier.Discard(1);
      classifier.Draw(1);
      classifier.Discard(1);
      classifier.Draw(1);
      classifier.Discard(1);
      classifier.Draw(1);
      classifier.Discard(1);
      classifier.Draw(1);
      classifier.Discard(1);
      Assert.That(classifier.Shanten, Is.EqualTo(2));
    }
  }
}