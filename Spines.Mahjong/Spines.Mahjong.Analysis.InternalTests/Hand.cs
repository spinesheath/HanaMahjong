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
using System.Text;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Mahjong.Analysis.InternalTests
{
  /// <summary>
  /// Maintains tile counts and calculates the Shanten of a hand.
  /// </summary>
  internal class Hand
  {
    /// <summary>
    /// Creates a new instance of Hand.
    /// </summary>
    public Hand()
    {
      _suits = new[] {_cManzu, _cPinzu, _cSouzu, _cJihai};
      _classifier = new Classifier();
      _melds = new[] {_mManzu, _mPinzu, _mSouzu};
    }

    /// <summary>
    /// The current Shanten of the hand.
    /// </summary>
    public int Shanten
    {
      get
      {
        return _classifier.ClassifyArrangements(
          _classifier.ClassifySuits(_mManzu, _meldCounts[0], _cManzu),
          _classifier.ClassifySuits(_mPinzu, _meldCounts[1], _cPinzu),
          _classifier.ClassifySuits(_mSouzu, _meldCounts[2], _cSouzu),
          _classifier.ClassifyHonors(Enumerable.Repeat(0, 8).Concat(_cJihai.OrderByDescending(x => x)).ToList()));
      }
    }

    /// <summary>
    /// Discards a tile based on UkeIre.
    /// </summary>
    public void Discard()
    {
      if (_tilesInHand != 14)
      {
        throw new InvalidOperationException("Can't discard from a 13 tile hand.");
      }

      var shanten = Shanten;
      if (shanten == -1)
      {
        throw new InvalidOperationException("Already won.");
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
    }

    /// <summary>
    /// Draws a tile and optionally wins the hand.
    /// </summary>
    /// <param name="tileId">The tile to draw.</param>
    /// <returns>The result of the draw.</returns>
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

    /// <summary>
    /// Offers to call a tile and potentially wins the hand.
    /// </summary>
    /// <param name="tileId">The tile offered to call.</param>
    /// <param name="canChii">Can a chii be called?</param>
    /// <returns>Whether the tile was called, the hand was won or the offer was ignored.</returns>
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
            _melds[suit][_meldCounts[suit]] = meldId;
            _meldCounts[suit] += 1;
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
            _meldCounts[suit] -= 1;
            _suits[suit][c + 0] += 1;
            _suits[suit][c + 1] += 1;
            _suits[suit][c + 2] += 1;
          }
        }

        if (_suits[suit][index] > 2)
        {
          var meldId = 7 + index;
          _melds[suit][_meldCounts[suit]] = meldId;
          _meldCounts[suit] += 1;
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
          _meldCounts[suit] -= 1;
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
      _melds[suit][_meldCounts[suit]] = bestCall;
      _meldCounts[suit] += 1;
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
      return GetConcealedString(0, 'm') + GetConcealedString(1, 'p') + GetConcealedString(2, 's') + GetConcealedString(3, 'z') +
             GetMeldString(0, 'M') + GetMeldString(1, 'P') + GetMeldString(2, 'S');
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
    private readonly int[][] _melds;
    private readonly int[] _meldCounts = new int[3];
    private readonly int[] _mManzu = new int[4];
    private readonly int[] _mPinzu = new int[4];
    private readonly int[] _mSouzu = new int[4];

    /// <summary>
    /// Finds the highest UkeIre after one discard from the hand.
    /// </summary>
    /// <returns>The number of UkeIre.</returns>
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

    /// <summary>
    /// Determines if a call is beneficial.
    /// </summary>
    /// <param name="shantenBeforeCall">The shanten before the call.</param>
    /// <param name="shantenAfterCall">The shanten the hand would have after the call.</param>
    /// <param name="ukeIreAfterCall">The UkeIre the hand would have after the call.</param>
    /// <returns>Whether tile should be called.</returns>
    private bool ShouldCall(int shantenBeforeCall, int shantenAfterCall, int ukeIreAfterCall)
    {
      if (shantenBeforeCall != shantenAfterCall)
      {
        return shantenBeforeCall > shantenAfterCall;
      }
      return ukeIreAfterCall > CountUkeIre(shantenBeforeCall);
    }

    /// <summary>
    /// Counts the UkeIre for the current 13 tile hand.
    /// </summary>
    /// <param name="currentShanten">The shanten of the current hand.</param>
    /// <returns>The number of UkeIre.</returns>
    private int CountUkeIre(int currentShanten)
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

        if (Shanten < currentShanten)
        {
          count += 4 - _visibleByType[k];
        }

        _suits[suit][index] -= 1;
      }
      return count;
    }

    /// <summary>
    /// Creates a string that represents the melds.
    /// </summary>
    /// <param name="suitId">The suit for which to get the string.</param>
    /// <param name="suit">The suit identifier for the melds.</param>
    /// <returns>A string representing the melds.</returns>
    private string GetMeldString(int suitId, char suit)
    {
      var sb = new StringBuilder();
      var meldCount = _meldCounts[suitId];
      for (var i = 0; i < meldCount; ++i)
      {
        var meldId = _melds[suitId][i];
        if (meldId < 7)
        {
          for (var m = meldId; m < meldId + 3; ++m)
          {
            sb.Append((char) ('1' + m));
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

    /// <summary>
    /// Creates a string that represents the closed tiles of a suit.
    /// </summary>
    /// <param name="suitId">The suit for which to get the string.</param>
    /// <param name="suit">The suit identifier.</param>
    /// <returns>A string representing the closed tiles in a suit.</returns>
    private string GetConcealedString(int suitId, char suit)
    {
      var sb = new StringBuilder();
      var tiles = _suits[suitId];
      for (var i = 0; i < tiles.Length; ++i)
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