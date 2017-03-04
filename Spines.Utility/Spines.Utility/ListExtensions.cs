// Spines.Utility.ListExtensions.cs
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

namespace Spines.Utility
{
  /// <summary>
  /// Extenstion methods for IList.
  /// </summary>
  public static class ListExtensions
  {
    /// <summary>
    /// Sets each element in the list to the specified value. Elements are not cloned.
    /// </summary>
    /// <typeparam name="TList">The type of the list.</typeparam>
    /// <typeparam name="TElement">The type of the elements in the list.</typeparam>
    /// <param name="list">The list to populate.</param>
    /// <param name="element">The element to populate the list with.</param>
    /// <returns>The origial list, with modified content.</returns>
    public static TList Populate<TList, TElement>(this TList list, TElement element)
      where TList : IList<TElement>
    {
      for (var i = 0; i < list.Count; ++i)
      {
        list[i] = element;
      }
      return list;
    }
  }
}