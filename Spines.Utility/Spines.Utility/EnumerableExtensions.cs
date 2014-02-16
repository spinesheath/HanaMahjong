using System.Collections.Generic;
using System.Linq;

namespace Spines.Utility
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Determines whether a sequence contains at least <paramref name="count" /> elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="enumerable">The sequence.</param>
        /// <param name="count">The minimum number of elements that the sequence should contain.</param>
        /// <returns>True if the sequence contains at least <paramref name="count" /> elements; otherwise, false.</returns>
        public static bool AtLeast<T>(this IEnumerable<T> enumerable, int count)
        {
            return enumerable.Skip(count - 1).Any();
        }

        /// <summary>
        /// Determines whether a sequence contains at most <paramref name="count" /> elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="enumerable">The sequence.</param>
        /// <param name="count">The maximum number of elements that the sequence should contain.</param>
        /// <returns>True if the sequence contains at most <paramref name="count" /> elements; otherwise, false.</returns>
        public static bool AtMost<T>(this IEnumerable<T> enumerable, int count)
        {
            return !enumerable.AtLeast(count + 1);
        }

        /// <summary>
        /// Determines whether a sequence contains any duplicate elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="enumerable">The sequence.</param>
        /// <returns>True if the sequence contains duplicate elements; otherwise, false.</returns>
        public static bool HasDuplicates<T>(this IEnumerable<T> enumerable)
        {
            var set = new HashSet<T>();
            return enumerable.Any(e => !set.Add(e));
        }
    }
}