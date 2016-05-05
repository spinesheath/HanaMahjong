// Spines.Mahjong.Analysis.Arrangement.cs
// 
// Copyright (C) 2016  Johannes Heckl
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

using Spines.Utility;

namespace Spines.Mahjong.Analysis.Combinations
{
  /// <summary>
  /// Information about an arrangement of tiles relevant for counting shanten.
  /// Only for regular hand shapes of one pair and four mentsu.
  /// </summary>
  internal class Arrangement
  {
    /// <summary>
    /// Creates a new Arrangement.
    /// </summary>
    /// <param name="jantou">The number of pairs (complete or incomplete).</param>
    /// <param name="mentsu">The number of groups (complete or incomplete).</param>
    /// <param name="value">The number of tiles used in the pairs and groups.</param>
    public Arrangement(int jantou, int mentsu, int value)
    {
      Jantou = Validate.InRange(jantou, 0, 1, nameof(jantou));
      Mentsu = Validate.InRange(mentsu, 0, 4, nameof(jantou));
      Value = Validate.InRange(value, 0, 14, nameof(jantou));
    }

    /// <summary>
    /// The number of pairs (complete or incomplete).
    /// </summary>
    public int Jantou { get; }

    /// <summary>
    /// The number of groups (complete or incomplete).
    /// </summary>
    public int Mentsu { get; }

    /// <summary>
    /// The number of tiles used in the pairs and groups.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Creates a new instance of Arrangement with an added jantou.
    /// </summary>
    public Arrangement AddJantou(int value)
    {
      Validate.InRange(value, 0, 3, nameof(value));
      return new Arrangement(Jantou + 1, Mentsu, Value + value);
    }

    /// <summary>
    /// Creates a new instance of Arrangement with an added mentsu.
    /// </summary>
    public Arrangement AddMentsu(int value)
    {
      Validate.InRange(value, 0, 3, nameof(value));
      return new Arrangement(Jantou, Mentsu + 1, Value + value);
    }

    /// <summary>
    /// Determines whether an arrangement is worse than another.
    /// Correctness pending.
    /// </summary>
    public bool IsWorseThan(Arrangement other)
    {
      // Equal pairs.
      if (Jantou == other.Jantou)
      {
        if (Mentsu == other.Mentsu)
        {
          return Value < other.Value;
        }
        if (Mentsu > other.Mentsu)
        {
          return Value <= other.Value;
        }

        // What if Mentsu = other.Mentsu - 1 and Value > other.Value - 3?
        return false;
      }
      // More pairs than other.
      if (Jantou == 1)
      {
        if (Mentsu == other.Mentsu)
        {
          return Value <= other.Value;
        }
        if (Mentsu > other.Mentsu)
        {
          return Value <= other.Value;
        }
        return false;
      }
      // less pairs
      return false;
    }
  }
}