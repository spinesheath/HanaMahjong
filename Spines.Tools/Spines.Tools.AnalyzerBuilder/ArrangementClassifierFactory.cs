// Spines.Tools.AnalyzerBuilder.ArrangementClassifierFactory.cs
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
using System.IO;
using System.Linq;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Tools.AnalyzerBuilder
{
  internal class ArrangementClassifierFactory : ClassifierFactoryBase
  {
    public ArrangementClassifierFactory(IProgressManager progressManager, string workingDirectory)
      : base(progressManager, workingDirectory)
    {
    }

    public async void CreateAsync()
    {
      var wordsFile = Path.Combine(WorkingDirectory, "ArrangementWords.txt");
      var lines = File.ReadAllLines(wordsFile);
      var wordWithValues = lines.Select(CreateWord);
      await CreateAsync(wordWithValues, "ArrangementClassifier.bin");
    }

    private static WordWithValue CreateWord(string line)
    {
      var a = line.Split(':');
      var b = a[0].Split(',').Select(c => Convert.ToInt32(c));
      return new WordWithValue(b, Convert.ToInt32(a[1]));
    }
  }
}