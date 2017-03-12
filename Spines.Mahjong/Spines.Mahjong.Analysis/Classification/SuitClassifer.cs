// Spines.Mahjong.Analysis.SuitClassifer.cs
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

using System.Linq;

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// Calculates arrangement value of a suit.
  /// </summary>
  internal class SuitClassifer
  {
    public void SetMelds(int[] melds, int meldCount)
    {
      _meldCount = meldCount;
      var current = 0;
      for (var i = 0; i < meldCount; ++i)
      {
        current = SuitFirstPhase[current + melds[i] + 1];
      }
      _entry = SuitFirstPhase[current];
    }

    public int GetValue(int[] tiles)
    {
      var current = _entry;
      var secondPhase = SuitSecondPhases[_meldCount];
      current = secondPhase[current + tiles[0]];
      switch (_meldCount)
      {
        case 0:
          current = secondPhase[current + tiles[1]];
          current = secondPhase[current + tiles[2]];
          current = secondPhase[current + tiles[3]];
          current = secondPhase[current + tiles[4]];
          current = secondPhase[current + tiles[5]];
          current = secondPhase[current + tiles[6]];
          current = secondPhase[current + tiles[7]];
          break;
        case 1:
          current = secondPhase[current + tiles[1]];
          current = secondPhase[current + tiles[2]];
          current = secondPhase[current + tiles[3]] + 11752;
          current = secondPhase[current + tiles[4]] + 30650;
          current = secondPhase[current + tiles[5]] + 55952;
          current = secondPhase[current + tiles[6]] + 80078;
          current = secondPhase[current + tiles[7]] + 99750;
          break;
        case 2:
          current = secondPhase[current + tiles[1]];
          current = secondPhase[current + tiles[2]] + 22358;
          current = secondPhase[current + tiles[3]] + 54162;
          current = secondPhase[current + tiles[4]] + 90481;
          current = secondPhase[current + tiles[5]] + 120379;
          current = secondPhase[current + tiles[6]] + 139662;
          current = secondPhase[current + tiles[7]] + 150573;
          break;
        case 3:
          current = secondPhase[current + tiles[1]] + 24641;
          current = secondPhase[current + tiles[2]] + 50680;
          current = secondPhase[current + tiles[3]] + 76245;
          current = secondPhase[current + tiles[4]] + 93468;
          current = secondPhase[current + tiles[5]] + 102953;
          current = secondPhase[current + tiles[6]] + 107217;
          current = secondPhase[current + tiles[7]] + 108982;
          break;
        case 4:
          current = secondPhase[current + tiles[1]];
          current = secondPhase[current + tiles[2]];
          current = secondPhase[current + tiles[3]];
          current = secondPhase[current + tiles[4]];
          current = secondPhase[current + tiles[5]];
          current = secondPhase[current + tiles[6]];
          current = secondPhase[current + tiles[7]];
          break;
      }
      return secondPhase[current + tiles[8]];
    }

    private int _meldCount;
    private int _entry;

    private static readonly ushort[] SuitFirstPhase = Resource.Transitions("SuitFirstPhase.txt").ToArray();
    private static readonly ushort[] SuitSecondPhase0 = Resource.Transitions("SuitSecondPhase0.txt").ToArray();
    private static readonly ushort[] SuitSecondPhase1 = Resource.Transitions("SuitSecondPhase1.txt").ToArray();
    private static readonly ushort[] SuitSecondPhase2 = Resource.Transitions("SuitSecondPhase2.txt").ToArray();
    private static readonly ushort[] SuitSecondPhase3 = Resource.Transitions("SuitSecondPhase3.txt").ToArray();
    private static readonly ushort[] SuitSecondPhase4 = Resource.Transitions("SuitSecondPhase4.txt").ToArray();

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