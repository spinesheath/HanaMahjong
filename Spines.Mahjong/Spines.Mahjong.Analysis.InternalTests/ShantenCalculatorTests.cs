// Spines.Mahjong.Analysis.InternalTests.ShantenCalculatorTests.cs
// 
// Copyright (C) 2017  Johannes Heckl
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Spines.Mahjong.Analysis.InternalTests
{
  [TestFixture]
  public class ShantenCalculatorTests
  {
    [TestCase("123456789m12344p", -1)]
    [TestCase("123456789m1234p", 0)]
    [TestCase("123456789m1245p", 1)]
    [TestCase("123456789m147p1s", 2)]
    [TestCase("12345679m147p14s", 3)]
    [TestCase("1345679m147p147s", 4)]
    [TestCase("145679m147p147s1z", 5)]
    [TestCase("14679m147p147s12z", 6)]
    [TestCase("1479m147p147s123z", 7)]
    [TestCase("147m147p147s1234z", 8)]
    [TestCase("123456789m44p123S", -1)]
    [TestCase("1245p112z333P6666P", 2)]
    [TestCase("123456789m44p111Z", -1)]
    [TestCase("1245p112z444Z3333Z", 1)]
    public void ShantenShouldBeCorrect(string hand, int expected)
    {
      var c = new Hand(hand);

      var actual = c.Shanten;

      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void HandShouldWork()
    {
      var rand = new Random(5);
      for (var iterations = 0; iterations < 100; iterations++)
      {
        var drawn = new bool[136];

        var hand = new Hand();
        for (var i = 0; i < 13; ++i)
        {
          var tileId = GetRandomTile(rand, drawn);
          drawn[tileId] = true;
          hand.Draw(tileId / 4);
        }

        var before = hand.ToString();
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
            var callResult = hand.OfferCall(tileId / 4,  playerId == 3);
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
        var after = hand.ToString();
        Assert.That(hand, Is.Not.Null);
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