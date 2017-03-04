// Spines.Utility.ObjectExtensions.cs
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

namespace Spines.Utility
{
  /// <summary>
  /// Extensions for all types.
  /// </summary>
  public static class ObjectExtensions
  {
    /// <summary>
    /// Creates a sequence of the current element followed by the tail.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <param name="head">The head of the new sequence.</param>
    /// <param name="tail">The tail of the new sequence.</param>
    /// <returns>A sequence of the current element followed by the tail.</returns>
    public static IEnumerable<T> Concat<T>(this T head, IEnumerable<T> tail)
    {
      return head.Yield().Concat(tail);
    }
  }
}