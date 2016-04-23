// Spines.Mahjong.Analysis.Combination.cs
// 
// Copyright (C) 2014  Johannes Heckl
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

namespace Spines.Mahjong.Analysis
{
  /// <summary>
  /// A combination of tiles.
  /// </summary>
  public class Combination
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