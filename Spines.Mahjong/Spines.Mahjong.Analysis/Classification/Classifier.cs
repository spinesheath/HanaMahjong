// Spines.Mahjong.Analysis.Classifier.cs
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

using System;
using System.Collections.Generic;
using Spines.Utility;

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// Classifies words of a language with equal-length words.
  /// </summary>
  [Serializable]
  public class Classifier
  {
    private readonly int[] _transitions;

    /// <summary>
    /// Creates a new instance of Classifier.
    /// </summary>
    /// <param name="alphabetSize">The size of the alphabet.</param>
    /// <param name="wordLength">The length of the words in the language.</param>
    /// <param name="language">The language that will be classified.</param>
    public Classifier(int alphabetSize, int wordLength, IEnumerable<WordWithValue> language)
    {
      var c = new ClassifierBuilder(alphabetSize, wordLength, language);
      _transitions = c.GetTransitions();
    }

    /// <summary>
    /// Classifies a word.
    /// </summary>
    public int Classify(IEnumerable<int> word)
    {
      var validWord = Validate.NotNull(word, nameof(word));
      var current = 0;
      foreach (var c in validWord)
      {
        current = _transitions[current + c];
      }
      return current;
    }
  }
}