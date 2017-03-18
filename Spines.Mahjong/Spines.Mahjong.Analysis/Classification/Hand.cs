// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// Maintains tile counts and calculates the Shanten of a hand.
  /// </summary>
  public class Hand
  {
    /// <summary>
    /// Creates a new instance of Hand.
    /// </summary>
    public Hand()
    {
      // Don't need to initialize _arrangementValues here because the value for an empty hand is 0.
      // Don't need to set the melds in the suit classifiers here because entry state for concealed suits for a hand without melds is 0.
      _suits = new[] { new int[9], new int[9], new int[9], _cJihai };
      _melds = new[] { new int[4], new int[4], new int[4] };
    }

    /// <summary>
    /// Creates a hand initialized with the tiles specified in shorthand form.
    /// </summary>
    /// <param name="initial">The shorthand form of the initial hand.</param>
    public Hand(ShorthandParser initial)
      : this()
    {
      InitializeMelds(initial.ManzuMeldIds, 0);
      InitializeMelds(initial.PinzuMeldIds, 1);
      InitializeMelds(initial.SouzuMeldIds, 2);

      foreach (var meldId in initial.JihaiMeldIds)
      {
        if (meldId < 7 + 9)
        {
          var index = meldId - 7;
          var tileType = index + 27;
          Draw(tileType);
          Draw(tileType);
          _arrangementValues[3] = _honorClassifier.Pon(_cJihai[index]);
          _cJihai[index] -= 2;
          _mJihai[index] += 3;
          _meldCount += 1;
          _tilesInHand += 1;
          _visibleByType[tileType] += 1;
          _inHandByType[tileType] += 1;
        }
        else
        {
          var index = meldId - 16;
          var tileType = index + 27;
          Draw(tileType);
          Draw(tileType);
          Draw(tileType);
          _arrangementValues[3] = _honorClassifier.Daiminkan();
          _cJihai[index] -= 3;
          _mJihai[index] += 4;
          _meldCount += 1;
          _visibleByType[tileType] += 1;
          _inHandByType[tileType] += 1;
        }
      }

      var concealed = initial.Concealed.ToList();
      for (var t = 0; t < concealed.Count; ++t)
      {
        for (var i = 0; i < concealed[t]; ++i)
        {
          Draw(t);
        }
      }
    }

    /// <summary>
    /// Is the hand in a valid state?
    /// </summary>
    public bool IsValid => _tilesInHand == 13 || _tilesInHand == 14;

    /// <summary>
    /// The current Shanten of the hand.
    /// </summary>
    public int Shanten => CalculateShanten(_arrangementValues) - 1;

    /// <summary>
    /// Calculates the UkeIre of the hand.
    /// </summary>
    /// <returns>Information about the UkeIre of the hand.</returns>
    public IEnumerable<UkeIreInfo> GetUkeIre()
    {
      if (_tilesInHand == 13)
      {
        yield return new UkeIreInfo(null, GetUkeIreFor13());
        yield break;
      }
      if (_tilesInHand != 14)
      {
        throw new InvalidOperationException("Can only calculate UkeIre for discards with 14 tiles in hand.");
      }
      var currentShanten = CalculateShanten(_arrangementValues);

      for (var j = 0; j < 34; ++j)
      {
        var discardSuit = j / 9;
        var discardIndex = j % 9;

        if (_suits[discardSuit][discardIndex] == 0)
        {
          continue;
        }
        InternalDiscard(discardSuit, discardIndex);

        var shantenAfterDiscard = CalculateShanten(_arrangementValues);
        if (shantenAfterDiscard <= currentShanten)
        {
          var ukeIre = GetUkeIreFor13();
          if (ukeIre.Any())
          {
            yield return new UkeIreInfo(new Tile { Suit = IdToSuit[discardSuit], Index = discardIndex, Location = TileLocation.Discarded }, ukeIre);
          }
        }

        InternalDraw(discardSuit, discardIndex);
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

      var shanten = CalculateShanten(_arrangementValues);
      if (shanten == 0)
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

      return CalculateShanten(_arrangementValues) == 0 ? DrawResult.Tsumo : DrawResult.Draw;
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

      if (CalculateShanten(_arrangementValues) == 0)
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
            AddMeld(suit, meldId);
            RemoveShuntsu(suit, c);
            var shantenOfCurrentCall = CalculateShanten(_arrangementValues);
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
            RemoveMeld(suit);
            AddShuntsu(suit, c);
          }
        }

        if (_suits[suit][index] > 2)
        {
          var meldId = 7 + index;
          AddMeld(suit, meldId);
          RemoveKoutsu(suit, index);
          var shantenOfCurrentCall = CalculateShanten(_arrangementValues);
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
          RemoveMeld(suit);
          AddKoutsu(suit, index);
        }
      }

      InternalDiscard(suit, index);

      if (suit == 3)
      {
        if (_cJihai[index] <= 2)
        {
          return CallResult.Ignore;
        }
        var shantenBeforeCall = CalculateShanten(_arrangementValues);
        var ukeIreBeforeCall = CountUkeIre(shantenBeforeCall);
        var hc = _honorClassifier.Fork();
        var a = _arrangementValues[3];
        _arrangementValues[3] = _honorClassifier.Pon(_cJihai[index]);
        _cJihai[index] -= 2;
        _mJihai[index] += 3;
        _meldCount += 1;
        _tilesInHand += 1;

        if (CalculateShanten(_arrangementValues) < shantenBeforeCall || CountUkeIre14() > ukeIreBeforeCall)
        {
          _inHandByType[tileType] += 1;
          return CallResult.Call;
        }

        _meldCount -= 1;
        _tilesInHand -= 1;
        _mJihai[index] -= 3;
        _cJihai[index] += 2;
        _arrangementValues[3] = a;
        _honorClassifier = hc;
        return CallResult.Ignore;
      }

      if (!ShouldCall(shantenOfBestCall, ukeIreOfBestCall))
      {
        return CallResult.Ignore;
      }

      _suits[suit][index] += 1;
      _tilesInHand += 1;
      _inHandByType[tileType] += 1;

      AddMeld(suit, bestCall);
      if (bestCall < 7)
      {
        RemoveShuntsu(suit, bestCall);
      }
      else if (bestCall < 16)
      {
        RemoveKoutsu(suit, bestCall - 7);
      }

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

    private static readonly Suit[] IdToSuit = { Suit.Manzu, Suit.Pinzu, Suit.Souzu, Suit.Jihai };

    private readonly int[] _cJihai = new int[7]; // concealed tiles
    private readonly byte[] _visibleByType = new byte[34]; // visible tile count per type
    private readonly byte[] _inHandByType = new byte[34]; // visible tile count per type
    private readonly int[][] _suits; // all four
    private int _tilesInHand;
    private readonly SuitClassifer[] _suitClassifiers = { new SuitClassifer(), new SuitClassifer(), new SuitClassifer() };
    private ProgressiveHonorClassifier _honorClassifier;
    private readonly int[][] _melds; // non-honors
    private readonly int[] _meldCounts = new int[3]; // used meldId slots for non-honors
    private readonly int[] _mJihai = new int[7]; // melded tiles
    private readonly int[] _arrangementValues = new int[4];
    private KokushiClassifier _kokushi = KokushiClassifier.Create();
    private ChiitoiClassifier _chiitoi = ChiitoiClassifier.Create();
    private int _meldCount;

    /// <summary>
    /// Calculates the UkeIre of the hand.
    /// </summary>
    /// <returns>Information about the UkeIre of the hand.</returns>
    private Dictionary<Tile, int> GetUkeIreFor13()
    {
      var currentShanten = CalculateShanten(_arrangementValues);

      var ukeIre = new Dictionary<Tile, int>();
      var tileType = 0;
      var localArrangements = new[] { _arrangementValues[0], _arrangementValues[1], _arrangementValues[2], _arrangementValues[3] };
      for (var suit = 0; suit < 3; ++suit)
      {
        for (var index = 0; index < 9; ++index)
        {
          if (_inHandByType[tileType] != 4)
          {
            if (index == 0 || index == 8)
            {
              _kokushi.Draw(_suits[suit][index]);
            }
            _chiitoi.Draw(_suits[suit][index]);

            _suits[suit][index] += 1;
            localArrangements[suit] = _suitClassifiers[suit].GetValue(_suits[suit]);
            if (CalculateShanten(localArrangements) < currentShanten)
            {
              ukeIre.Add(new Tile { Suit = IdToSuit[suit], Index = index }, 4 - _visibleByType[tileType]);
            }

            _chiitoi.Discard(_suits[suit][index]);
            if (index == 0 || index == 8)
            {
              _kokushi.Discard(_suits[suit][index]);
            }
            _suits[suit][index] -= 1;
          }
          tileType += 1;
        }
        localArrangements[suit] = _arrangementValues[suit];
      }
      for (var index = 0; index < 7; ++index)
      {
        if (_inHandByType[tileType] != 4)
        {
          var previousTileCount = _cJihai[index];
          _kokushi.Draw(previousTileCount);
          _chiitoi.Draw(previousTileCount);
          localArrangements[3] = _honorClassifier.Fork().Draw(_cJihai[index], _mJihai[index]);
          if (CalculateShanten(localArrangements) < currentShanten)
          {
            ukeIre.Add(new Tile { Suit = Suit.Jihai, Index = index }, 4 - _visibleByType[tileType]);
          }
          _chiitoi.Discard(previousTileCount + 1);
          _kokushi.Discard(previousTileCount + 1);
        }
        tileType += 1;
      }

      return ukeIre;
    }

    private void InitializeMelds(IEnumerable<int> meldIds, int suitId)
    {
      var list = meldIds.ToList();
      for (var i = 0; i < list.Count; ++i)
      {
        var meldId = list[i];
        AddMeld(suitId, meldId);

        if (meldId < 7)
        {
          var start = 9 * suitId + meldId;
          _visibleByType[start + 0] += 1;
          _inHandByType[start + 0] += 1;
          _visibleByType[start + 1] += 1;
          _inHandByType[start + 1] += 1;
          _visibleByType[start + 2] += 1;
          _inHandByType[start + 2] += 1;
        }
        else if (meldId < 16)
        {
          var start = 9 * suitId + meldId - 7;
          _visibleByType[start] += 3;
          _inHandByType[start] += 3;
        }
        else
        {
          var start = 9 * suitId + meldId - 16;
          _visibleByType[start] += 4;
          _inHandByType[start] += 4;
        }
      }
      _tilesInHand += list.Count * 3;
    }

    private void AddMeld(int suitId, int meldId)
    {
      _melds[suitId][_meldCounts[suitId]] = meldId;
      _meldCounts[suitId] += 1;
      _meldCount += 1;
      _suitClassifiers[suitId].SetMelds(_melds[suitId], _meldCounts[suitId]);
      UpdateValue(suitId);
    }

    private void RemoveMeld(int suitId)
    {
      _meldCounts[suitId] -= 1;
      _meldCount -= 1;
      _suitClassifiers[suitId].SetMelds(_melds[suitId], _meldCounts[suitId]);
      UpdateValue(suitId);
    }

    private void InternalDiscard(int suit, int index)
    {
      _inHandByType[suit * 9 + index] -= 1;
      _tilesInHand -= 1;
      if (suit == 3)
      {
        _kokushi.Discard(_cJihai[index]);
        _chiitoi.Discard(_cJihai[index]);
        _cJihai[index] -= 1;
        _arrangementValues[3] = _honorClassifier.Discard(_cJihai[index], _mJihai[index]);
      }
      else
      {
        if (index == 0 || index == 8)
        {
          _kokushi.Discard(_suits[suit][index]);
        }
        _chiitoi.Discard(_suits[suit][index]);
        _suits[suit][index] -= 1;
        UpdateValue(suit);
      }
    }

    private void InternalDraw(int suit, int index)
    {
      _inHandByType[suit * 9 + index] += 1;
      _tilesInHand += 1;
      if (suit == 3)
      {
        _arrangementValues[3] = _honorClassifier.Draw(_cJihai[index], _mJihai[index]);
        _kokushi.Draw(_cJihai[index]);
        _chiitoi.Draw(_cJihai[index]);
        _cJihai[index] += 1;
      }
      else
      {
        if (index == 0 || index == 8)
        {
          _kokushi.Draw(_suits[suit][index]);
        }
        _chiitoi.Draw(_suits[suit][index]);
        _suits[suit][index] += 1;
        UpdateValue(suit);
      }
    }

    /// <summary>
    /// Adds the tiles of a shuntsu to the concealed part of the hand.
    /// </summary>
    /// <param name="suit">The suit of the shuntsu.</param>
    /// <param name="index">The index of the first tile of the shuntsu.</param>
    private void AddShuntsu(int suit, int index)
    {
      _suits[suit][index + 0] += 1;
      _suits[suit][index + 1] += 1;
      _suits[suit][index + 2] += 1;
      UpdateValue(suit);
    }

    /// <summary>
    /// Removed the tiles of a shuntsu from the concealed part of the hand.
    /// </summary>
    /// <param name="suit">The suit of the shuntsu.</param>
    /// <param name="index">The index of the first tile of the shuntsu.</param>
    private void RemoveShuntsu(int suit, int index)
    {
      _suits[suit][index + 0] -= 1;
      _suits[suit][index + 1] -= 1;
      _suits[suit][index + 2] -= 1;
      UpdateValue(suit);
    }

    /// <summary>
    /// Adds the tiles of a koutsu to the concealed part of the hand.
    /// </summary>
    /// <param name="suit">The suit of the shuntsu.</param>
    /// <param name="index">The index of the tiles of the koutsu.</param>
    private void AddKoutsu(int suit, int index)
    {
      _suits[suit][index] += 3;
      UpdateValue(suit);
    }

    /// <summary>
    /// Removes the tiles of a koutsu from the concealed part of the hand.
    /// </summary>
    /// <param name="suit">The suit of the shuntsu.</param>
    /// <param name="index">The index of the tiles of the koutsu.</param>
    private void RemoveKoutsu(int suit, int index)
    {
      _suits[suit][index] -= 3;
      UpdateValue(suit);
    }

    private void UpdateValue(int suit)
    {
      _arrangementValues[suit] = _suitClassifiers[suit].GetValue(_suits[suit]);
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

        var shantenAfterDiscard = CalculateShanten(_arrangementValues);
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

        var shantenAfterDiscard = CalculateShanten(_arrangementValues);
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
      var shantenBeforeCall = CalculateShanten(_arrangementValues);
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
      var tileType = 0;
      var localArrangements = new[]
      {_arrangementValues[0], _arrangementValues[1], _arrangementValues[2], _arrangementValues[3]};
      for (var suit = 0; suit < 3; ++suit)
      {
        for (var index = 0; index < 9; ++index)
        {
          if (_inHandByType[tileType] != 4)
          {
            if (index == 0 || index == 8)
            {
              _kokushi.Draw(_suits[suit][index]);
            }
            _chiitoi.Draw(_suits[suit][index]);
            _suits[suit][index] += 1;
            localArrangements[suit] = _suitClassifiers[suit].GetValue(_suits[suit]);
            if (CalculateShanten(localArrangements) < currentShanten)
            {
              count += 4 - _visibleByType[tileType];
            }
            if (index == 0 || index == 8)
            {
              _kokushi.Discard(_suits[suit][index]);
            }
            _chiitoi.Discard(_suits[suit][index]);
            _suits[suit][index] -= 1;
          }
          tileType += 1;
        }
        localArrangements[suit] = _arrangementValues[suit];
      }
      for (var index = 0; index < 7; ++index)
      {
        if (_inHandByType[tileType] != 4)
        {
          var previousTileCount = _cJihai[index];
          _kokushi.Draw(previousTileCount);
          _chiitoi.Draw(previousTileCount);
          localArrangements[3] = _honorClassifier.Fork().Draw(_cJihai[index], _mJihai[index]);
          if (CalculateShanten(localArrangements) < currentShanten)
          {
            count += 4 - _visibleByType[tileType];
          }
          _chiitoi.Discard(previousTileCount + 1);
          _kokushi.Discard(previousTileCount + 1);
        }
        tileType += 1;
      }
      return count;
    }

    private int CalculateShanten(int[] arrangementValues)
    {
      var shanten = ArrangementClassifier.Classify(arrangementValues);
      if (_meldCount > 0)
      {
        return shanten;
      }
      return Math.Min(shanten, Math.Min(_kokushi.Shanten, _chiitoi.Shanten));
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
            sb.Append((char)('1' + m));
          }
        }
        else if (meldId < 16)
        {
          sb.Append((char)('1' + meldId - 7), 3);
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
          sb.Append((char)('1' + i), 3);
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
          sb.Append((char)('1' + i));
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