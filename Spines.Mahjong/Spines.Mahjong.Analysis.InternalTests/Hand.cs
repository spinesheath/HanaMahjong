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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Mahjong.Analysis.InternalTests
{
  public enum CallResult
  {
    Ignore,
    Call,
    Ron
  }

  public enum DrawResult
  {
    Draw,
    Tsumo
  }

  internal class Hand
  {
    public Hand()
    {
      _suits = new[] {_cManzu, _cPinzu, _cSouzu, _cJihai};
      _classifier = new Classifier();
      _melds = new[] {_mManzu, _mPinzu, _mSouzu};
    }

    public int Shanten
    {
      get
      {
        return _classifier.ClassifyArrangements(
          _classifier.ClassifySuits(_mManzu, _cManzu),
          _classifier.ClassifySuits(_mPinzu, _cPinzu),
          _classifier.ClassifySuits(_mSouzu, _cSouzu),
          _classifier.ClassifyHonors(Enumerable.Repeat(0, 8).Concat(_cJihai.OrderByDescending(x => x)).ToList()));
      }
    }

    public bool Discard()
    {
      if (_tilesInHand != 14)
      {
        return false;
      }

      var shanten = Shanten;
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

        var shantenAfterDiscard = Shanten;
        if (shantenAfterDiscard > bestShanten || shantenAfterDiscard > shanten)
        {
          _suits[suit][index] += 1;
          continue;
        }

        var count = CountUkeIre(shantenAfterDiscard);

        if (count > ukeIreCount || shantenAfterDiscard < bestShanten)
        {
          ukeIreCount = count;
          bestDiscard = j;
          bestShanten = shantenAfterDiscard;
        }

        _suits[suit][index] += 1;
      }

      _suits[bestDiscard / 9][bestDiscard % 9] -= 1;
      _tilesInHand -= 1;

      return true;
    }

    public DrawResult Draw(int tileId)
    {
      if (_tilesInHand == 14)
      {
        throw new InvalidOperationException("Can't draw with a 14 tile hand.");
      }
      if (_visibleById[tileId])
      {
        throw new InvalidOperationException("Can't draw a tile that is already visible.");
      }
      _visibleById[tileId] = true;
      var t = tileId / 4;

      var suit = t / 9;
      var index = t % 9;

      _visibleByType[t] += 1;
      _suits[suit][index] += 1;
      _tilesInHand += 1;

      return Shanten == -1 ? DrawResult.Tsumo : DrawResult.Draw;
    }

    private int CountUkeIre14()
    {
      var ukeIreCount = -1;
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

        var shantenAfterDiscard = Shanten;
        if (shantenAfterDiscard > bestShanten)
        {
          _suits[suit][index] += 1;
          continue;
        }

        var count = CountUkeIre(shantenAfterDiscard);

        if (count > ukeIreCount || shantenAfterDiscard < bestShanten)
        {
          ukeIreCount = count;
          bestShanten = shantenAfterDiscard;
        }

        _suits[suit][index] += 1;
      }
      return ukeIreCount;
    }

    public CallResult Call(int tileId, bool canChii)
    {
      if (_tilesInHand == 14)
      {
        throw new InvalidOperationException("Can't call with a 14 tile hand.");
      }
      if (_visibleById[tileId])
      {
        throw new InvalidOperationException("Can't call a tile that is already visible.");
      }
      _visibleById[tileId] = true;
      var t = tileId / 4;
      _visibleByType[t] += 1;

      var previousShanten = Shanten;
      if (previousShanten == -1)
      {
        throw new InvalidOperationException("Already won.");
      }

      var suit = t / 9;
      var index = t % 9;

      var bestCall = -1;
      var shantenOfBestCall = 10;
      var ukeIreOfBestCall = -1;

      if (suit < 3)
      {
        _suits[suit][index] += 1;
        _tilesInHand += 1;
        if (Shanten == -1)
        {
          return CallResult.Ron;
        }

        if (canChii)
        {
          for (var c = Math.Max(0, index - 2); c < Math.Min(7, index + 2); ++c)
          {
            if (_suits[suit][c + 0] <= 0 || _suits[suit][c + 1] <= 0 || _suits[suit][c + 2] <= 0)
            {
              continue;
            }
            var meldId = c;
            _melds[suit].Add(meldId);
            _suits[suit][c + 0] -= 1;
            _suits[suit][c + 1] -= 1;
            _suits[suit][c + 2] -= 1;
            var shanten = Shanten;
            if (shanten <= shantenOfBestCall)
            {
              var ukeIre = CountUkeIre14();
              if (shanten < shantenOfBestCall || ukeIre > ukeIreOfBestCall)
              {
                bestCall = meldId;
                shantenOfBestCall = shanten;
                ukeIreOfBestCall = ukeIre;
              }
            }
            _melds[suit].RemoveAt(_melds[suit].Count - 1);
            _suits[suit][c + 0] += 1;
            _suits[suit][c + 1] += 1;
            _suits[suit][c + 2] += 1;
          }
        }

        if (_suits[suit][index] > 2)
        {
          var meldId = 7 + index;
          _melds[suit].Add(meldId);
          _suits[suit][index] -= 3;
          var shanten = Shanten;
          if (shanten <= shantenOfBestCall)
          {
            var ukeIre = CountUkeIre14();
            if (shanten < shantenOfBestCall || ukeIre > ukeIreOfBestCall)
            {
              bestCall = meldId;
              shantenOfBestCall = shanten;
              ukeIreOfBestCall = ukeIre;
            }
          }
          _melds[suit].RemoveAt(_melds[suit].Count - 1);
          _suits[suit][index] += 3;
        }

        _suits[suit][index] -= 1;
        _tilesInHand -= 1;
      }

      if (!ShouldCall(previousShanten, shantenOfBestCall, ukeIreOfBestCall))
      {
        return CallResult.Ignore;
      }

      _suits[suit][index] += 1;
      _melds[suit].Add(bestCall);
      if (bestCall < 7)
      {
        _suits[suit][bestCall + 0] -= 1;
        _suits[suit][bestCall + 1] -= 1;
        _suits[suit][bestCall + 2] -= 1;
      }
      else if (bestCall < 16)
      {
        _suits[suit][bestCall - 7] -= 3;
      }

      _tilesInHand += 1;
      return CallResult.Call;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// A string that represents the current object.
    /// </returns>
    public override string ToString()
    {
      return ToConcealedString(_cManzu, 'm') + ToConcealedString(_cPinzu, 'p') + ToConcealedString(_cSouzu, 's') +
             ToConcealedString(_cJihai, 'z') +
             ToMeldString(_mManzu, 'M') + ToMeldString(_mPinzu, 'P') + ToMeldString(_mSouzu, 'S');
    }

    private readonly int[] _cManzu = new int[9];
    private readonly int[] _cSouzu = new int[9];
    private readonly int[] _cPinzu = new int[9];
    private readonly int[] _cJihai = new int[7];
    private readonly int[] _visibleByType = new int[34];
    private readonly bool[] _visibleById = new bool[136];
    private readonly int[][] _suits;
    private int _tilesInHand;
    private readonly Classifier _classifier;
    private readonly List<int>[] _melds;
    private readonly List<int> _mManzu = new List<int>();
    private readonly List<int> _mPinzu = new List<int>();
    private readonly List<int> _mSouzu = new List<int>();

    private bool ShouldCall(int previousShanten, int shantenOfBestCall, int ukeIreOfBestCall)
    {
      if (previousShanten == shantenOfBestCall)
      {
        var previousUkeIre = CountUkeIre(previousShanten);
        if (ukeIreOfBestCall > previousUkeIre)
        {
          return true;
        }
      }
      else if (previousShanten > shantenOfBestCall)
      {
        return true;
      }
      return false;
    }

    private int CountUkeIre(int previousShanten)
    {
      var count = 0;
      for (var k = 0; k < 34; ++k)
      {
        if (_visibleByType[k] == 4)
        {
          continue;
        }

        var suit = k / 9;
        var index = k % 9;

        _suits[suit][index] += 1;

        if (Shanten < previousShanten)
        {
          count += 4 - _visibleByType[k];
        }

        _suits[suit][index] -= 1;
      }
      return count;
    }

    private static string ToMeldString(IEnumerable<int> meldIds, char suit)
    {
      var sb = new StringBuilder();
      foreach (var meldId in meldIds)
      {
        if (meldId < 7)
        {
          for (var i = meldId; i < meldId + 3; ++i)
          {
            sb.Append((char) ('1' + i));
          }
        }
        else if (meldId < 16)
        {
          sb.Append((char) ('1' + meldId - 7), 3);
        }
        sb.Append(suit);
      }
      return sb.ToString();
    }

    private static string ToConcealedString(IReadOnlyList<int> tiles, char suit)
    {
      var sb = new StringBuilder();
      for (var i = 0; i < tiles.Count; ++i)
      {
        for (var j = 0; j < tiles[i]; ++j)
        {
          sb.Append((char) ('1' + i));
        }
      }
      if (sb.Length == 0)
      {
        return string.Empty;
      }
      sb.Append(suit);
      return sb.ToString();
    }
  }
}