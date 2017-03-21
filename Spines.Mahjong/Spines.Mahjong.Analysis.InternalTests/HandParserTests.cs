// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using NUnit.Framework;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Mahjong.Analysis.InternalTests
{
  [TestFixture]
  public class HandParserTests
  {
    [TestCase("1m")]
    public void HandShouldThrowIfInvalid(string hand)
    {
      Assert.Throws<FormatException>(() => new HandParser(hand));
    }
  }
}