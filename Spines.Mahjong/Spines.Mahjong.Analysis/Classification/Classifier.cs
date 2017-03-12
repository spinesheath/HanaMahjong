// Spines.Mahjong.Analysis.Classifier.cs
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

using System.Collections.Generic;
using System.Linq;

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// Calculates the shanten of a set of four arrangements.
  /// </summary>
  internal class Classifier
  {
    /// <summary>
    /// Calculates the shanten of 4 arrangements.
    /// Behavior for invalid inputs is undefined.
    /// Input is invalid if there is no legal 14 tile hand that is represented by these arrangements.
    /// </summary>
    /// <param name="a1">Id of the first arrangement as used in the transition data.</param>
    /// <param name="a2">Id of the second arrangement as used in the transition data.</param>
    /// <param name="a3">Id of the third arrangement as used in the transition data.</param>
    /// <param name="a4">Id of the fourth arrangement as used in the transition data.</param>
    /// <returns>The shanten of the hand.</returns>
    public int ClassifyArrangements(int a1, int a2, int a3, int a4)
    {
      return
        ArrangementTransitions[
          ArrangementTransitions[ArrangementTransitions[ArrangementTransitions[0 + a1] + a2] + a3] + a4] - 1;
    }

    /// <summary>
    /// Returns the Arrangement for a Suit. The word is the sequence
    /// (meld count, melded tile counts, concealed tile counts)
    /// </summary>
    public int ClassifySuits(IReadOnlyList<int> melds, int meldCount, IReadOnlyList<int> concealed)
    {
      var current = 0;
      var secondPhase = SuitSecondPhases[meldCount];
      switch (meldCount)
      {
        case 0:
          current = SuitFirstPhase[current];
          current = secondPhase[current + concealed[0]];
          current = secondPhase[current + concealed[1]];
          current = secondPhase[current + concealed[2]];
          current = secondPhase[current + concealed[3]];
          current = secondPhase[current + concealed[4]];
          current = secondPhase[current + concealed[5]];
          current = secondPhase[current + concealed[6]];
          current = secondPhase[current + concealed[7]];
          current = secondPhase[current + concealed[8]];
          break;
        case 1:
          current = SuitFirstPhase[current + melds[0] + 1];
          current = SuitFirstPhase[current];
          current = secondPhase[current + concealed[0]];
          current = secondPhase[current + concealed[1]];
          current = secondPhase[current + concealed[2]];
          current = secondPhase[current + concealed[3]] + 11752;
          current = secondPhase[current + concealed[4]] + 30650;
          current = secondPhase[current + concealed[5]] + 55952;
          current = secondPhase[current + concealed[6]] + 80078;
          current = secondPhase[current + concealed[7]] + 99750;
          current = secondPhase[current + concealed[8]];
          break;
        case 2:
          current = SuitFirstPhase[current + melds[0] + 1];
          current = SuitFirstPhase[current + melds[1] + 1];
          current = SuitFirstPhase[current];
          current = secondPhase[current + concealed[0]];
          current = secondPhase[current + concealed[1]];
          current = secondPhase[current + concealed[2]] + 22358;
          current = secondPhase[current + concealed[3]] + 54162;
          current = secondPhase[current + concealed[4]] + 90481;
          current = secondPhase[current + concealed[5]] + 120379;
          current = secondPhase[current + concealed[6]] + 139662;
          current = secondPhase[current + concealed[7]] + 150573;
          current = secondPhase[current + concealed[8]];
          break;
        case 3:
          current = SuitFirstPhase[current + melds[0] + 1];
          current = SuitFirstPhase[current + melds[1] + 1];
          current = SuitFirstPhase[current + melds[2] + 1];
          current = SuitFirstPhase[current];
          current = secondPhase[current + concealed[0]];
          current = secondPhase[current + concealed[1]] + 24641;
          current = secondPhase[current + concealed[2]] + 50680;
          current = secondPhase[current + concealed[3]] + 76245;
          current = secondPhase[current + concealed[4]] + 93468;
          current = secondPhase[current + concealed[5]] + 102953;
          current = secondPhase[current + concealed[6]] + 107217;
          current = secondPhase[current + concealed[7]] + 108982;
          current = secondPhase[current + concealed[8]];
          break;
        case 4:
          current = SuitFirstPhase[current + melds[0] + 1];
          current = SuitFirstPhase[current + melds[1] + 1];
          current = SuitFirstPhase[current + melds[2] + 1];
          current = SuitFirstPhase[current + melds[3] + 1];
          current = SuitFirstPhase[current];
          current = secondPhase[current + concealed[0]];
          current = secondPhase[current + concealed[1]];
          current = secondPhase[current + concealed[2]];
          current = secondPhase[current + concealed[3]];
          current = secondPhase[current + concealed[4]];
          current = secondPhase[current + concealed[5]];
          current = secondPhase[current + concealed[6]];
          current = secondPhase[current + concealed[7]];
          current = secondPhase[current + concealed[8]];
          break;
      }
      return current;
    }

    /// <summary>
    /// Returns the Arrangement for Honors. The word is the sequence
    /// (meld count, melded tile counts, concealed tile counts)
    /// </summary>
    public int ClassifyHonors(IReadOnlyList<int> word)
    {
      var current = 0;
      var count = word.Count;
      for (var i = 0; i < count; ++i)
      {
        current = HonorTransitions[current + word[i]];
      }
      return current;
    }

    private static readonly ushort[] ArrangementTransitions = Parse.Transitions("ArrangementTransitions.txt").ToArray();
    private static readonly ushort[] HonorTransitions = Parse.Transitions("HonorTransitions.txt").ToArray();
    private static readonly ushort[] SuitFirstPhase = Parse.Transitions("SuitFirstPhase.txt").ToArray();
    private static readonly ushort[] SuitSecondPhase0 = Parse.Transitions("SuitSecondPhase0.txt").ToArray();
    private static readonly ushort[] SuitSecondPhase1 = Parse.Transitions("SuitSecondPhase1.txt").ToArray();
    private static readonly ushort[] SuitSecondPhase2 = Parse.Transitions("SuitSecondPhase2.txt").ToArray();
    private static readonly ushort[] SuitSecondPhase3 = Parse.Transitions("SuitSecondPhase3.txt").ToArray();
    private static readonly ushort[] SuitSecondPhase4 = Parse.Transitions("SuitSecondPhase4.txt").ToArray();

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