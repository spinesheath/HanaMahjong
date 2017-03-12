// Spines.Mahjong.Analysis.ArrangementClassifier.cs
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
  /// Calculates shanten from arrangement values.
  /// </summary>
  internal class ArrangementClassifier
  {
    /// <summary>
    /// Calculates the shanten of 4 arrangements.
    /// Behavior for invalid inputs is undefined.
    /// Input is invalid if there is no legal 13 or 14 tile hand that is represented by these arrangements.
    /// </summary>
    /// <param name="values">The arrangement values for the 4 suits.</param>
    /// <returns>The shanten of the hand.</returns>
    public int Classify(int[] values)
    {
      var current = 0;
      for (var i = 0; i < 4; ++i)
      {
        current = Arrangement[current + values[i]];
      }
      return current - 1;
    }

    private static readonly ushort[] Arrangement = Resource.Transitions("ArrangementTransitions.txt").ToArray();
  }
}