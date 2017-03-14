// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;
using Spines.Tools.AnalyzerBuilder.Combinations;
using Spines.Utility;

namespace Spines.Tools.AnalyzerBuilder.Tests
{
  [TestFixture]
  public class ArrangementAnalyzerTests
  {
    [Test]
    public void CalculateShantenShouldReturnShanten()
    {
      // Randomly picked 2 shanten hand. 34467p 44446s 3447m
      var pinzu = new[] // 001201100 (0,3,5)(1,2,4)(2,0,0)(2,1,2)(2,2,3)
      {
        new Arrangement(0, 3, 5),
        new Arrangement(1, 2, 4), // 2 incomplete mentsu
        new Arrangement(2, 0, 0),
        new Arrangement(2, 1, 2),
        new Arrangement(2, 2, 3)
      };
      var souzu = new[] // 000401000 (0,2,5)(1,1,3)(2,0,0)(2,1,2)
      {
        new Arrangement(0, 2, 5), // 1 incomplete and 1 complete mentsu
        new Arrangement(1, 1, 3),
        new Arrangement(2, 0, 0),
        new Arrangement(2, 1, 2)
      };
      var manzu = new[] // 001200100 (0,3,4)(1,1,2)(1,2,3)(2,0,0)(2,1,1)(2,2,2)
      {
        new Arrangement(0, 3, 4),
        new Arrangement(1, 1, 2),
        new Arrangement(1, 2, 3),
        new Arrangement(2, 0, 0), // 1 complete pair
        new Arrangement(2, 1, 1),
        new Arrangement(2, 2, 2)
      };
      var honor = new Arrangement(0, 0, 0).Yield();

      var analyzer = new ArrangementAnalyzer();
      analyzer.AddSetOfArrangements(pinzu);
      analyzer.AddSetOfArrangements(souzu);
      analyzer.AddSetOfArrangements(manzu);
      analyzer.AddSetOfArrangements(honor);
      var shanten = analyzer.CalculateShanten();
      Assert.AreEqual(2, shanten);
    }
  }
}