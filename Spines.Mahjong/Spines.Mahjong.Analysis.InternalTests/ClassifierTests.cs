/*
 *  Copyright (C) 2016  Johannes Heckl
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Spines.Mahjong.Analysis.Classification;
using Spines.Utility;

namespace Spines.Mahjong.Analysis.InternalTests
{
  [TestFixture]
  public class ClassifierTests
  {
    [Test]
    public void TestClassifierBuilder()
    {
      const int wordLength = 5;
      const int alphabetSize = 5;
      var language = CreateLanguage(alphabetSize, wordLength).ToList();
      var builder = new ClassifierBuilder(alphabetSize, wordLength);
      foreach (var word in language)
      {
        builder.AddWord(word);
      }
      var classifier = new Classifier(builder.CreateTransitions());
      foreach (var word in language)
      {
        CheckWord(classifier, word);
      }
    }

    private static IEnumerable<WordWithValue> CreateLanguage(int alphabetSize, int wordLength)
    {
      var a = Enumerable.Range(0, alphabetSize);
      var b = Enumerable.Repeat(a, wordLength);
      var c = b.CartesianProduct();
      foreach (var word in c)
      {
        var list = word.ToList();
        var value = list.Sum() % 13;
        if (value < 8)
        {
          yield return new WordWithValue(list, value);
        }
      }
    }

    private static void CheckWord(Classifier classifier, WordWithValue wordWithValue)
    {
      Assert.AreEqual(wordWithValue.Value, classifier.Classify(wordWithValue.Word));
    }
  }
}
