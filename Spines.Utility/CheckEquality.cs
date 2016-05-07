// Spines.Utility.CheckEquality.cs
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

namespace Spines.Utility
{
  /// <summary>
  /// Methods for checking equality of objects.
  /// </summary>
  public static class CheckEquality
  {
    /// <summary>
    /// Checks if lhs and rhs are equal, using the lhs.Equals.
    /// Requires that lhs and rhs have exactly the same type.
    /// </summary>
    public static bool WithIdenticalTypes<T>(T lhs, object rhs)
      where T : class
    {
      if (ReferenceEquals(null, rhs))
      {
        return false;
      }
      if (ReferenceEquals(lhs, rhs))
      {
        return true;
      }
      if (rhs.GetType() != lhs.GetType())
      {
        return false;
      }
      return lhs.Equals((T) rhs);
    }
  }
}