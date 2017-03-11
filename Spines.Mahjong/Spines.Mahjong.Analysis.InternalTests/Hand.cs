// Spines.Mahjong.Analysis.InternalTests.Hand.cs
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
using System.Linq;
using Spines.Mahjong.Analysis.Classification;
using Spines.Utility;

namespace Spines.Mahjong.Analysis.InternalTests
{
  internal class Hand
  {
    public Hand(int seed)
    {
      _suits = new[] {_cManzu, _cPinzu, _cSouzu, _cJihai};
      _rand = new Random(seed);
      _classifier = new Classifier();
      Repeat.Action(() => DrawRandom(), 13);
    }

    public bool DiscardBest()
    {
      var shanten = _classifier.ClassifyArrangements(
        _classifier.ClassifySuits(_emptyMelds, _cManzu),
        _classifier.ClassifySuits(_emptyMelds, _cPinzu),
        _classifier.ClassifySuits(_emptyMelds, _cSouzu),
        _classifier.ClassifyHonors(Enumerable.Repeat(0, 8).Concat(_cJihai.OrderByDescending(x => x)).ToList()));

      if (shanten == -1)
      {
        return false;
      }

      var ukeIreCount = -1;
      var bestDiscard = -1;
      var bestShanten = 10;
      for (var j = 0; j < 34; ++j)
      {
        var suit = j / 9;
        var index = j % 9;

        if (_suits[suit][index] == 0)
        {
          continue;
        }
        _suits[suit][index] -= 1;

        var shantenAfterDiscard = _classifier.ClassifyArrangements(
          _classifier.ClassifySuits(_emptyMelds, _cManzu),
          _classifier.ClassifySuits(_emptyMelds, _cPinzu),
          _classifier.ClassifySuits(_emptyMelds, _cSouzu),
          _classifier.ClassifyHonors(Enumerable.Repeat(0, 8).Concat(_cJihai.OrderByDescending(x => x)).ToList()));

        if (shantenAfterDiscard > bestShanten)
        {
          _suits[suit][index] += 1;
          continue;
        }

        var u = 0;
        for (var k = 0; k < 34; ++k)
        {
          if (_used[k] == 4)
          {
            continue;
          }
          _suits[k / 9][k % 9] += 1;

          var shantenFromUkeIre = _classifier.ClassifyArrangements(
            _classifier.ClassifySuits(_emptyMelds, _cManzu),
            _classifier.ClassifySuits(_emptyMelds, _cPinzu),
            _classifier.ClassifySuits(_emptyMelds, _cSouzu),
            _classifier.ClassifyHonors(Enumerable.Repeat(0, 8).Concat(_cJihai.OrderByDescending(x => x)).ToList()));

          _suits[k / 9][k % 9] -= 1;

          if (shantenFromUkeIre < shanten)
          {
            u += 4 - _used[k];
          }
        }

        if (u > ukeIreCount || shantenAfterDiscard < bestShanten)
        {
          ukeIreCount = u;
          bestDiscard = j;
          bestShanten = shantenAfterDiscard;
        }

        _suits[suit][index] += 1;
      }

      _suits[bestDiscard / 9][bestDiscard % 9] -= 1;
      _tilesInHand -= 1;

      return true;
    }

    public bool Draw(int tileId)
    {
      if (_tilesInHand == 14)
      {
        return false;
      }
      if (_drawn[tileId])
      {
        return false;
      }
      _drawn[tileId] = true;
      var t = tileId / 4;
      if (_used[t] == 4)
      {
        return false;
      }

      var suit = t / 9;
      var index = t % 9;

      _used[t] += 1;
      _suits[suit][index] += 1;
      _drawCount += 1;
      _tilesInHand += 1;

      return true;
    }

    public bool DrawRandom()
    {
      if (_drawCount == 136 || _tilesInHand == 14)
      {
        return false;
      }
      while (true)
      {
        var s = _rand.Next(136);
        if (_drawn[s])
        {
          continue;
        }
        _drawn[s] = true;
        var t = s / 4;
        if (_used[t] == 4)
        {
          continue;
        }

        var suit = t / 9;
        var index = t % 9;

        _used[t] += 1;
        _suits[suit][index] += 1;
        _drawCount += 1;
        _tilesInHand += 1;
        return true;
      }
    }

    private readonly Random _rand;
    private readonly int[] _cManzu = new int[9];
    private readonly int[] _cSouzu = new int[9];
    private readonly int[] _cPinzu = new int[9];
    private readonly int[] _cJihai = new int[7];
    private readonly int[] _used = new int[34];
    private readonly bool[] _drawn = new bool[136];
    private readonly int[][] _suits;
    private int _drawCount;
    private int _tilesInHand;
    private readonly Classifier _classifier;
    private readonly int[] _emptyMelds = new int[0];
  }
}