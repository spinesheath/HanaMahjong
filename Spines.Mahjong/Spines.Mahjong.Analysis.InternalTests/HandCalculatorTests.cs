// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Mahjong.Analysis.InternalTests
{
  [TestFixture]
  public class HandCalculatorTests
  {
    [TestCase("123m1234789p3388s")]
    public void DeepShantenShouldBeFast(string hand)
    {
      Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
      Thread.CurrentThread.Priority = ThreadPriority.Highest;

      var parser = new ShorthandParser(hand);
      Console.WriteLine(new HandCalculator(parser).Shanten);

      for (var f = 0; f < 1; ++f)
      {
        var sw = new Stopwatch();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        sw.Restart();

        for (var iterations = 0; iterations < 10000; iterations++)
        {
          var hc = new HandCalculator(parser);
          var u = hc.GetDeepUkeIre(1, 1).ToList();
          Assert.IsNotNull(u);
        }

        sw.Stop();
        Console.WriteLine(sw.ElapsedTicks);

        Assert.That(sw.ElapsedTicks, Is.GreaterThan(0));
      }
    }

    [TestCase("3366p11s11577z444S")]
    [TestCase("1m19p9s1234457z555Z")]
    public void DiscardShouldNotThrow(string hand)
    {
      var parser = new ShorthandParser(hand);
      var c = new HandCalculator(parser);
      Assert.DoesNotThrow(() => c.Discard());
    }

    [TestCase("123456789m12344p", -1)]
    [TestCase("123456789m1234p", 0)]
    [TestCase("123456789m1245p", 1)]
    [TestCase("123456789m147p1s", 2)]
    [TestCase("12345679m147p14s", 3)]
    [TestCase("1345679m147p147s", 4)]
    [TestCase("145679m147p147s1z", 5)]
    [TestCase("14679m147p147s12z", 6)]
    [TestCase("1479m147p147s123z", 6)]
    [TestCase("147m147p147s1234z", 6)]
    [TestCase("123456789m44p123S", -1)]
    [TestCase("1245p112z333P6666P", 2)]
    [TestCase("123456789m44p111Z", -1)]
    [TestCase("1245p112z444Z3333Z", 1)]
    [TestCase("19m19p19s1234567z", 0)]
    [TestCase("114477m114477p11s", -1)]
    [TestCase("1111222445889s", 2)]
    public void ShantenShouldBeCorrect(string hand, int expected)
    {
      var parser = new ShorthandParser(hand);
      var c = new HandCalculator(parser);

      var actual = c.Shanten;

      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void HandShouldWork()
    {
      Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
      Thread.CurrentThread.Priority = ThreadPriority.Highest;

      var parser = new ShorthandParser("123456789m12344p");
      Console.WriteLine(new HandCalculator(parser).Shanten);

      for (var f = 0; f < 1; ++f)
      {
        var rand = new Random(5);

        var sw = new Stopwatch();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        sw.Restart();

        for (var iterations = 0; iterations < 40000; iterations++)
        {
          var drawn = new bool[136];

          var hand = new HandCalculator();
          for (var i = 0; i < 13; ++i)
          {
            var tileId = GetRandomTile(rand, drawn);
            drawn[tileId] = true;
            hand.Draw(tileId / 4);
          }

          var playerId = 0;
          while (hand.Shanten > -1 && drawn.Any(d => !d))
          {
            var tileId = GetRandomTile(rand, drawn);
            drawn[tileId] = true;
            if (playerId == 0)
            {
              var drawResult = hand.Draw(tileId / 4);
              if (drawResult == DrawResult.Tsumo)
              {
                break;
              }
              hand.Discard();
            }
            else
            {
              var callResult = hand.OfferCall(tileId / 4, playerId == 3);
              if (callResult == CallResult.Call)
              {
                hand.Discard();
                playerId = 0;
              }
              else if (callResult == CallResult.Ron)
              {
                break;
              }
            }
            playerId = (playerId + 1) % 4;
          }
        }

        sw.Stop();
        Console.WriteLine(sw.ElapsedTicks);

        Assert.That(sw.ElapsedTicks, Is.GreaterThan(0));
      }
    }

    private static int GetRandomTile(Random rand, IReadOnlyList<bool> drawn)
    {
      while (true)
      {
        var n = rand.Next(136);
        if (!drawn[n])
        {
          return n;
        }
      }
    }
  }
}