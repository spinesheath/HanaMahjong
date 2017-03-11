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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Spines.Utility;

namespace Spines.Tools.AnalyzerBuilder.Combinations
{
  /// <summary>
  /// Information about an arrangement of tiles relevant for counting shanten.
  /// Only for regular hand shapes of one pair and four mentsu.
  /// </summary>
  public class Arrangement : IEquatable<Arrangement>
  {
    /// <summary>
    /// Creates a new Arrangement.
    /// </summary>
    public Arrangement(int jantouValue, int mentsuCount, int mentsuValue)
    {
      JantouValue = Validate.InRange(jantouValue, 0, 2, nameof(jantouValue));
      MentsuCount = Validate.InRange(mentsuCount, 0, 4, nameof(mentsuCount));
      MentsuValue = Validate.InRange(mentsuValue, 0, 12, nameof(mentsuValue));
      if (mentsuValue < mentsuCount)
      {
        throw new ArgumentException("mentsuValue can not be less than mentsuCount.");
      }
      Id = JantouValue * 25 + MentsuCount * (MentsuCount - 1) + MentsuValue;
    }

    /// <summary>
    /// The valye of the pair (if present) in the arrangement (complete or incomplete).
    /// </summary>
    public int JantouValue { get; }

    /// <summary>
    /// The number of mentsu (complete or incomplete).
    /// </summary>
    public int MentsuCount { get; }

    /// <summary>
    /// The number of tiles used in mentsu.
    /// </summary>
    public int MentsuValue { get; }

    /// <summary>
    /// The total value of the arrangement.
    /// </summary>
    public int TotalValue => MentsuValue + JantouValue;

    /// <summary>
    /// Does the arrangement have a jantou?
    /// </summary>
    public bool HasJantou => 0 != JantouValue;

    /// <summary>
    /// The Id of the arrangement.
    /// </summary>
    public int Id { get; }

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
    /// Creates a new instance of Arrangement with a different jantou value.
    /// </summary>
    public Arrangement SetJantouValue(int value)
    {
      Validate.InRange(value, 0, 2, nameof(value));
      return new Arrangement(value, MentsuCount, MentsuValue);
    }

    /// <summary>
    /// Creates a new instance of Arrangement with an added mentsu.
    /// </summary>
    public Arrangement AddMentsu(int value)
    {
      Validate.InRange(value, 0, 3, nameof(value));
      return new Arrangement(JantouValue, MentsuCount + 1, MentsuValue + value);
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

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// A string that represents the current object.
    /// </returns>
    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider",
      MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)")]
    public override string ToString()
    {
      return $"{ToChar(JantouValue)}{ToChar(MentsuCount)}{ToChar(MentsuValue)}";
    }

    /// <summary>
    /// Creates an arrangement from a 3 character string.
    /// </summary>
    /// <param name="arrangement">The string to parse.</param>
    /// <returns>An instance of Arrangement.</returns>
    public static Arrangement FromString(string arrangement)
    {
      Validate.NotNull(arrangement, nameof(arrangement));
      if (arrangement.Length != 3)
      {
        throw new ArgumentException("s must be exactly three characters long.");
      }
      return new Arrangement(FromChar(arrangement[0]), FromChar(arrangement[1]), FromChar(arrangement[2]));
    }

    /// <summary>
    /// Creates multiple arrangements from a string with 3 characters per arrangement.
    /// </summary>
    /// <param name="arrangements">The string to parse.</param>
    /// <returns>A sequence of arrangements.</returns>
    public static IEnumerable<Arrangement> MultipleFromString(string arrangements)
    {
      Validate.NotNull(arrangements, nameof(arrangements));
      for (var i = 0; i < arrangements.Length; i += 3)
      {
        var arrangement = arrangements.Substring(i, 3);
        yield return FromString(arrangement);
      }
    }

    private static char ToChar(int n)
    {
      if (n < 10)
      {
        return (char) ('0' + n);
      }
      return (char) ('A' + n - 10);
    }

    private static int FromChar(char n)
    {
      if (n > '9')
      {
        return n - 'A' + 10;
      }
      return n - '0';
    }
  }
}