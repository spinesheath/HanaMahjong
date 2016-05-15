// Spines.Mahjong.Analysis.WordWithValue.cs
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

using System.Collections.Generic;
using System.Linq;

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// A word with an associated value.
  /// </summary>
  public class WordWithValue
  {
    /// <summary>
    /// Creates a new instance of WordWithValue.
    /// </summary>
    /// <param name="word">The characters of the word.</param>
    /// <param name="value">The value of the word. This is what the word will be classified as.</param>
    public WordWithValue(IEnumerable<int> word, int value)
    {
      Word = word.ToList();
      Value = value;
    }

    /// <summary>
    /// The word.
    /// </summary>
    public IReadOnlyList<int> Word { get; }

    /// <summary>
    /// The value of the Word.
    /// </summary>
    public int Value { get; }
  }
}