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
      _melds = new[] {_mManzu, _mPinzu, _mSouzu};
    }

    public Hand(string initial)
      : this()
    {
      var parser = new ShorthandParser(initial);
      var concealed = parser.Concealed.ToList();
      for (var t = 0; t < concealed.Count; ++t)
      {
        for (var i = 0; i < concealed[t]; ++i)
        {
          Draw(t);
        }
      }
      InitializeMelds(parser.ManzuMeldIds, 0);
      InitializeMelds(parser.PinzuMeldIds, 1);
      InitializeMelds(parser.SouzuMeldIds, 2);

      foreach (var meldId in parser.JihaiMeldIds)
      {
        if (meldId < 7 + 9)
        {
          var index = meldId - 7;
          var tileType = index + 27;
          Draw(tileType);
          Draw(tileType);
          _honorClassifier.MoveNext(GetHonorPonActionId(index));
          _cJihai[index] -= 2;
          _mJihai[index] += 3;
          _tilesInHand += 1;
        }
        else
        {
          var index = meldId - 16;
          var tileType = index + 27;
          Draw(tileType);
          Draw(tileType);
          Draw(tileType);
          _honorClassifier.MoveNext(GetHonorDaiminkanActionId());
          _cJihai[index] -= 3;
          _mJihai[index] += 4;
          _tilesInHand += 1;
        }
      }
    }

    /// <summary>
    /// The current Shanten of the hand.
    /// </summary>
    public int Shanten
    {
      get
      {
        _suitClassifiers[0].SetMelds(_mManzu, _meldCounts[0]);
        var vManzu = _suitClassifiers[0].GetValue(_cManzu);
        _suitClassifiers[1].SetMelds(_mPinzu, _meldCounts[1]);
        var vPinzu = _suitClassifiers[1].GetValue(_cPinzu);
        _suitClassifiers[2].SetMelds(_mSouzu, _meldCounts[2]);
        var vSouzu = _suitClassifiers[2].GetValue(_cSouzu);
        return _arrangementClassifier.Classify(vManzu, vPinzu, vSouzu, _honorClassifier.Value);
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

      var bestDiscard = GetBestDiscard(shanten);

      var suit = bestDiscard / 9;
      var index = bestDiscard % 9;

      InternalDiscard(suit, index);
    }

    /// <summary>
    /// Draws a tile and optionally wins the hand.
    /// </summary>
    /// <param name="tileType">The tile to draw.</param>
    /// <returns>The result of the draw.</returns>
    public DrawResult Draw(int tileType)
    {
      if (_tilesInHand == 14)
      {
        throw new InvalidOperationException("Can't draw with a 14 tile hand.");
      }
      if (_visibleByType[tileType] == 4)
      {
        throw new InvalidOperationException("Can't draw a tile that already has 4 visible.");
      }

      var suit = tileType / 9;
      var index = tileType % 9;

      InternalDraw(suit, index);

      _visibleByType[tileType] += 1;

      return Shanten == -1 ? DrawResult.Tsumo : DrawResult.Draw;
    }

    /// <summary>
    /// Offers to call a tile and potentially wins the hand.
    /// </summary>
    /// <param name="tileType">The tile offered to call.</param>
    /// <param name="canChii">Can a chii be called?</param>
    /// <returns>Whether the tile was called, the hand was won or the offer was ignored.</returns>
    public CallResult OfferCall(int tileType, bool canChii)
    {
      if (_tilesInHand == 14)
      {
        throw new InvalidOperationException("Can't call with a 14 tile hand.");
      }
      if (_visibleByType[tileType] == 4)
      {
        throw new InvalidOperationException("Can't call a tile that already has 4 visible.");
      }
      _visibleByType[tileType] += 1;

      var suit = tileType / 9;
      var index = tileType % 9;

      var bestCall = -1;
      var shantenOfBestCall = 10;
      var ukeIreOfBestCall = -1;

      InternalDraw(suit, index);

      if (Shanten == -1)
      {
        return CallResult.Ron;
      }

      if (suit < 3)
      {
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
            var shantenOfCurrentCall = Shanten;
            if (shantenOfCurrentCall <= shantenOfBestCall)
            {
              var ukeIre = CountUkeIre14();
              if (shantenOfCurrentCall < shantenOfBestCall || ukeIre > ukeIreOfBestCall)
              {
                bestCall = meldId;
                shantenOfBestCall = shantenOfCurrentCall;
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
          var shantenOfCurrentCall = Shanten;
          if (shantenOfCurrentCall <= shantenOfBestCall)
          {
            var ukeIre = CountUkeIre14();
            if (shantenOfCurrentCall < shantenOfBestCall || ukeIre > ukeIreOfBestCall)
            {
              bestCall = meldId;
              shantenOfBestCall = shantenOfCurrentCall;
              ukeIreOfBestCall = ukeIre;
            }
          }
          _meldCounts[suit] -= 1;
          _suits[suit][index] += 3;
        }
      }

      InternalDiscard(suit, index);

      if (suit == 3)
      {
        if (_suits[suit][index] <= 2)
        {
          return CallResult.Ignore;
        }
        var shantenBeforeCall = Shanten;
        var ukeIreBeforeCall = CountUkeIre(shantenBeforeCall);
        _honorClassifier.Push();
        _honorClassifier.MoveNext(GetHonorPonActionId(index));
        _cJihai[index] -= 2;
        _mJihai[index] += 3;
        _tilesInHand += 1;

        if (Shanten < shantenBeforeCall || CountUkeIre14() > ukeIreBeforeCall)
        {
          return CallResult.Call;
        }

        _tilesInHand -= 1;
        _mJihai[index] -= 3;
        _cJihai[index] += 2;
        _honorClassifier.Pop();
        return CallResult.Ignore;
      }

      if (!ShouldCall(shantenOfBestCall, ukeIreOfBestCall))
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
      return GetConcealedString(0, 'm') + GetConcealedString(1, 'p') + GetConcealedString(2, 's') +
             GetConcealedString(3, 'z') +
             GetMeldString(0, 'M') + GetMeldString(1, 'P') + GetMeldString(2, 'S') + GetHonorMeldString();
    }

    private readonly int[] _cManzu = new int[9]; // concealed tiles
    private readonly int[] _cSouzu = new int[9]; // concealed tiles
    private readonly int[] _cPinzu = new int[9]; // concealed tiles
    private readonly int[] _cJihai = new int[7]; // concealed tiles
    private readonly int[] _visibleByType = new int[34]; // visible tile count per type
    private readonly int[][] _suits; // all four
    private int _tilesInHand;
    private readonly ArrangementClassifier _arrangementClassifier = new ArrangementClassifier();
    private readonly SuitClassifer[] _suitClassifiers = {new SuitClassifer(), new SuitClassifer(), new SuitClassifer()};
    private readonly ProgressiveHonorClassifier _honorClassifier = new ProgressiveHonorClassifier();
    private readonly int[][] _melds; // non-honors
    private readonly int[] _meldCounts = new int[3]; // used meldId slots for non-honors
    private readonly int[] _mManzu = new int[4]; // meldIds
    private readonly int[] _mPinzu = new int[4]; // meldIds
    private readonly int[] _mSouzu = new int[4]; // meldIds
    private readonly int[] _mJihai = new int[7]; // melded tiles

    private void InitializeMelds(IEnumerable<int> meldIds, int suitId)
    {
      var list = meldIds.ToList();
      for (var i = 0; i < list.Count; ++i)
      {
        _melds[suitId][i] = list[i];
        _meldCounts[suitId] += 1;
      }
    }

    private int GetHonorPonActionId(int index)
    {
      return 8 + _cJihai[index];
    }

    private int GetHonorDaiminkanActionId()
    {
      return 12;
    }

    private int GetBestDiscard(int shanten)
    {
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

        InternalDiscard(suit, index);

        var shantenAfterDiscard = Shanten;
        if (shantenAfterDiscard <= bestShanten && shantenAfterDiscard <= shanten)
        {
          var count = CountUkeIre(shantenAfterDiscard);
          if (count > ukeIreCount || shantenAfterDiscard < bestShanten)
          {
            ukeIreCount = count;
            bestDiscard = j;
            bestShanten = shantenAfterDiscard;
          }
        }

        InternalDraw(suit, index);
      }
      return bestDiscard;
    }

    private int GetHonorDiscardActionId(int index)
    {
      var melded = _mJihai[index];
      return 4 + _cJihai[index] + melded + (melded & 1);
    }

    private int GetHonorDrawActionId(int index)
    {
      var melded = _mJihai[index];
      return _cJihai[index] + melded + (melded & 1);
    }

    private void InternalDiscard(int suit, int index)
    {
      if (suit == 3)
      {
        _honorClassifier.MoveNext(GetHonorDiscardActionId(index));
      }
      _suits[suit][index] -= 1;
      _tilesInHand -= 1;
    }

    private void InternalDraw(int suit, int index)
    {
      if (suit == 3)
      {
        _honorClassifier.MoveNext(GetHonorDrawActionId(index));
      }
      _suits[suit][index] += 1;
      _tilesInHand += 1;
    }

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
        InternalDiscard(suit, index);

        // TODO this block is repeated
        var shantenAfterDiscard = Shanten;
        if (shantenAfterDiscard <= bestShanten)
        {
          var count = CountUkeIre(shantenAfterDiscard);
          if (count > ukeIreCount || shantenAfterDiscard < bestShanten)
          {
            ukeIreCount = count;
            bestShanten = shantenAfterDiscard;
          }
        }
        InternalDraw(suit, index);
      }
      return ukeIreCount;
    }

    /// <summary>
    /// Determines if a call is beneficial.
    /// </summary>
    /// <param name="shantenAfterCall">The shanten the hand would have after the call.</param>
    /// <param name="ukeIreAfterCall">The UkeIre the hand would have after the call.</param>
    /// <returns>Whether tile should be called.</returns>
    private bool ShouldCall(int shantenAfterCall, int ukeIreAfterCall)
    {
      if (Shanten != shantenAfterCall)
      {
        return Shanten > shantenAfterCall;
      }
      return ukeIreAfterCall > CountUkeIre(Shanten);
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

        InternalDraw(suit, index);

        if (Shanten < currentShanten)
        {
          count += 4 - _visibleByType[k];
        }

        InternalDiscard(suit, index);
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

    private string GetHonorMeldString()
    {
      var sb = new StringBuilder();
      for (var i = 0; i < 7; ++i)
      {
        if (_mJihai[i] > 0)
        {
          sb.Append((char) ('1' + i), 3);
          sb.Append('Z');
        }
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