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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Mahjong.Analysis.InternalTests
{
  [TestClass]
  public class ClassifierTests
  {
    [TestMethod]
    public void TestClassifierBuilder()
    {
      var language = new List<WordWithValue>
      {
        new WordWithValue(new [] {0, 0}, 1),
        new WordWithValue(new [] {1, 1}, 2)
      };
      var classifier = new Classifier(2, 2, language);
      Assert.AreEqual(1, classifier.Classify(language[0].Word));
    }
  }
}
