// Spines.Utility.ReadOnlyListExtensions.cs
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

using System.Collections;
using System.Collections.Generic;

namespace Spines.Utility
{
  /// <summary>
  /// ExtensionMethods for IReadOnlyList.
  /// </summary>
  public static class ReadOnlyListExtensions
  {
    /// <summary>
    /// Creates a list that represents a slice of the source list.
    /// </summary>
    /// <typeparam name="T">The type of the elements contained in the lists.</typeparam>
    /// <param name="source">The source list.</param>
    /// <param name="start">The index of the first element in the slice.</param>
    /// <param name="length">The number of elements in the slice.</param>
    /// <returns>A slice of the source list.</returns>
    public static IReadOnlyList<T> Slice<T>(this IReadOnlyList<T> source, int start, int length)
    {
      Validate.NotNull(source, nameof(source));
      Validate.NotNegative(start, nameof(start));
      Validate.NotNegative(length, nameof(length));
      return new SliceList<T>(source, start, length);
    }

    /// <summary>
    /// A list that represents a slice of another list.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    private sealed class SliceList<T> : IReadOnlyList<T>
    {
      /// <summary>
      /// Creates a new instance of SliceList.
      /// </summary>
      /// <param name="source">The source list.</param>
      /// <param name="start">The index of the first element in the slice.</param>
      /// <param name="length">The number of elements in the slice.</param>
      public SliceList(IReadOnlyList<T> source, int start, int length)
      {
        _source = source;
        _start = start;
        Count = length;
      }

      /// <summary>
      /// Gets the number of elements in the collection.
      /// </summary>
      /// <returns>
      /// The number of elements in the collection.
      /// </returns>
      public int Count { get; }

      /// <summary>
      /// Gets the element at the specified index in the read-only list.
      /// </summary>
      /// <returns>
      /// The element at the specified index in the read-only list.
      /// </returns>
      /// <param name="index">The zero-based index of the element to get. </param>
      public T this[int index]
      {
        get
        {
          Validate.InRange(index, 0, Count, nameof(index));
          return _source[_start + index];
        }
      }

      /// <summary>
      /// Returns an enumerator that iterates through the collection.
      /// </summary>
      /// <returns>
      /// An enumerator that can be used to iterate through the collection.
      /// </returns>
      public IEnumerator<T> GetEnumerator()
      {
        return new SliceEnumerator(this);
      }

      private readonly IReadOnlyList<T> _source;
      private readonly int _start;

      /// <summary>
      /// Returns an enumerator that iterates through a collection.
      /// </summary>
      /// <returns>
      /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
      /// </returns>
      IEnumerator IEnumerable.GetEnumerator()
      {
        return GetEnumerator();
      }

      /// <summary>
      /// An enumerator for IReadOnlyList.
      /// </summary>
      private sealed class SliceEnumerator : IEnumerator<T>
      {
        /// <summary>
        /// Creates a new instance of SliceEnumerator.
        /// </summary>
        /// <param name="source">The list to enumerate.</param>
        public SliceEnumerator(IReadOnlyList<T> source)
        {
          _source = source;
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public T Current => _source[_current];

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the
        /// collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
        public bool MoveNext()
        {
          _current += 1;
          return _current != _source.Count;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
        public void Reset()
        {
          _current = -1;
        }

        private readonly IReadOnlyList<T> _source;

        private int _current = -1;

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        /// <returns>
        /// The current element in the collection.
        /// </returns>
        object IEnumerator.Current => Current;
      }
    }
  }
}