﻿// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// Calculates arrangement value of a suit.
  /// </summary>
  internal class SuitClassifer
  {
    public SuitClassifer Clone()
    {
      return new SuitClassifer {_entry = _entry, _meldCount = _meldCount, _secondPhase = _secondPhase};
    }

    public void SetMelds(int[] melds, int meldCount)
    {
      _meldCount = meldCount;
      var current = 0;
      for (var i = 0; i < meldCount; ++i)
      {
        current = SuitFirstPhase[current + melds[i] + 1];
      }
      _entry = SuitFirstPhase[current];
      _secondPhase = SuitSecondPhases[_meldCount];
    }

    public int GetValue(int[] tiles)
    {
      var current = _entry;
      current = _secondPhase[current + tiles[0]];
      switch (_meldCount)
      {
        case 1:
          current = _secondPhase[current + tiles[1]];
          current = _secondPhase[current + tiles[2]];
          current = _secondPhase[current + tiles[3]] + 11752;
          current = _secondPhase[current + tiles[4]] + 30650;
          current = _secondPhase[current + tiles[5]] + 55952;
          current = _secondPhase[current + tiles[6]] + 80078;
          current = _secondPhase[current + tiles[7]] + 99750;
          break;
        case 2:
          current = _secondPhase[current + tiles[1]];
          current = _secondPhase[current + tiles[2]] + 22358;
          current = _secondPhase[current + tiles[3]] + 54162;
          current = _secondPhase[current + tiles[4]] + 90481;
          current = _secondPhase[current + tiles[5]] + 120379;
          current = _secondPhase[current + tiles[6]] + 139662;
          current = _secondPhase[current + tiles[7]] + 150573;
          break;
        case 3:
          current = _secondPhase[current + tiles[1]] + 24641;
          current = _secondPhase[current + tiles[2]] + 50680;
          current = _secondPhase[current + tiles[3]] + 76245;
          current = _secondPhase[current + tiles[4]] + 93468;
          current = _secondPhase[current + tiles[5]] + 102953;
          current = _secondPhase[current + tiles[6]] + 107217;
          current = _secondPhase[current + tiles[7]] + 108982;
          break;
        case 0:
        case 4:
          current = _secondPhase[current + tiles[1]];
          current = _secondPhase[current + tiles[2]];
          current = _secondPhase[current + tiles[3]];
          current = _secondPhase[current + tiles[4]];
          current = _secondPhase[current + tiles[5]];
          current = _secondPhase[current + tiles[6]];
          current = _secondPhase[current + tiles[7]];
          break;
      }
      return _secondPhase[current + tiles[8]];
    }

    private ushort[] _secondPhase = SuitSecondPhase0;
    private int _meldCount;
    private int _entry;

    private static readonly ushort[] SuitFirstPhase = Resource.Transitions("SuitFirstPhase.txt");
    private static readonly ushort[] SuitSecondPhase0 = Resource.Transitions("SuitSecondPhase0.txt");
    private static readonly ushort[] SuitSecondPhase1 = Resource.Transitions("SuitSecondPhase1.txt");
    private static readonly ushort[] SuitSecondPhase2 = Resource.Transitions("SuitSecondPhase2.txt");
    private static readonly ushort[] SuitSecondPhase3 = Resource.Transitions("SuitSecondPhase3.txt");
    private static readonly ushort[] SuitSecondPhase4 = Resource.Transitions("SuitSecondPhase4.txt");

    private static readonly ushort[][] SuitSecondPhases =
    {
      SuitSecondPhase0,
      SuitSecondPhase1,
      SuitSecondPhase2,
      SuitSecondPhase3,
      SuitSecondPhase4
    };
  }
}