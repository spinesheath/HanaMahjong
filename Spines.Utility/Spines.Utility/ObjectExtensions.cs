// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

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