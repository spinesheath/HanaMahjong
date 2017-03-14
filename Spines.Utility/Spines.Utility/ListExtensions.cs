// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

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