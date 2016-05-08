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

using System;
using System.Diagnostics;
using Spines.Utility;

namespace Spines.Mahjong.Analysis.Combinations
{
  /// <summary>
  /// Information about an arrangement of tiles relevant for counting shanten.
  /// Only for regular hand shapes of one pair and four mentsu.
  /// </summary>
  [DebuggerDisplay("Arrangement {Jantou}, {Mentsu}, {Value}")]
  public class Arrangement : IEquatable<Arrangement>
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
      Id = Jantou * 50 + Mentsu * 7 + Value;
    }

    private Arrangement(int id)
    {
      Jantou = id / 50;
      var withoutJantou = id - Jantou * 50;
      Mentsu = withoutJantou / 7;
      Value = withoutJantou - Mentsu * 7;
      Id = id;
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

    private int Id { get; }

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
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public bool Equals(Arrangement other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }
      if (ReferenceEquals(this, other))
      {
        return true;
      }
      return Id == other.Id;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <returns>
    /// true if the specified object is equal to the current object; otherwise, false.
    /// </returns>
    /// <param name="obj">The object to compare with the current object. </param>
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
      {
        return false;
      }
      if (ReferenceEquals(this, obj))
      {
        return true;
      }
      if (obj.GetType() != GetType())
      {
        return false;
      }
      return Equals((Arrangement) obj);
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>
    /// A hash code for the current object.
    /// </returns>
    public override int GetHashCode()
    {
      return Id;
    }

    /// <summary>
    /// Checks if the Arrangements are equal.
    /// </summary>
    public static bool operator ==(Arrangement left, Arrangement right)
    {
      return Equals(left, right);
    }

    /// <summary>
    /// Checks if the Arrangements are not equal.
    /// </summary>
    public static bool operator !=(Arrangement left, Arrangement right)
    {
      return !Equals(left, right);
    }
  }
}