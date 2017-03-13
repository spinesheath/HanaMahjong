// Spines.Mahjong.Analysis.InternalTests.ArrangementClassifierTests.cs
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

using NUnit.Framework;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Mahjong.Analysis.InternalTests
{
  [TestFixture]
  public class ArrangementClassifierTests
  {
    [Test]
    public void ClassifyShouldReturnValueOfWord()
    {
      var a = ArrangementClassifier.Classify(new [] {0, 0, 0, 19});
      const int e = 9;

      Assert.That(a, Is.EqualTo(e));
    }
  }
}