// Spines.Tools.AnalyzerBuilder.ClassifierFactory.cs
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
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Tools.AnalyzerBuilder.Precalculation
{
  internal class ClassifierFactory
  {
    public ClassifierBuilder Create(IEnumerable<WordWithValue> wordWithValues)
    {
      var words = wordWithValues.ToList();
      var wordLength = words.First().Word.Count;
      var alphabetSize = words.SelectMany(w => w.Word).Max() + 1;
      var classifierBuilder = Create(words, alphabetSize, wordLength);
      return classifierBuilder;
    }

    private static ClassifierBuilder Create(IEnumerable<WordWithValue> words, int alphabetSize, int wordLength)
    {
      var builder = new ClassifierBuilder(alphabetSize, wordLength);
      foreach (var word in words)
      {
        builder.AddWord(word);
      }
      return builder;
    }
  }
}