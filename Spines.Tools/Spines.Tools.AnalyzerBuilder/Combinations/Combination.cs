// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace Spines.Tools.AnalyzerBuilder.Combinations
{
  /// <summary>
  /// A combination of tiles.
  /// </summary>
  internal class Combination
  {
    /// <summary>
    /// Constructs a new Combination for a given number of types.
    /// </summary>
    public Combination(IEnumerable<int> counts)
    {
      Counts = counts.ToList();
    }

    /// <summary>
    /// The counts for each tile type in the Combination.
    /// </summary>
    public IReadOnlyCollection<int> Counts { get; }
  }
}