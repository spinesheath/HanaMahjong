using System.Collections.Generic;

namespace Spines.Utility
{
    /// <summary>
    /// A container for enumeration values.
    /// </summary>
    /// <typeparam name="TContent">The type of the stored values.</typeparam>
    public interface IEnumContainer<TContent>
    {
        /// <summary>
        /// Initializes the container with the given elements.
        /// </summary>
        /// <param name="contents">The elements to store in the container.</param>
        void Initialize(IEnumerable<TContent> contents);

        /// <summary>
        /// Returns the stored contents.
        /// </summary>
        IReadOnlyList<TContent> DefinedValues { get; }
    }
}