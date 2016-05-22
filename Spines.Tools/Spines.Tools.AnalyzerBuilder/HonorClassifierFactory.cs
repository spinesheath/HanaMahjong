// Spines.Tools.AnalyzerBuilder.HonorClassifierFactory.cs
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
using System.IO;
using System.Linq;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Tools.AnalyzerBuilder
{
  internal class HonorClassifierFactory : ClassifierFactoryBase
  {
    public HonorClassifierFactory(IProgressManager progressManager, string workingDirectory)
      : base(progressManager, workingDirectory)
    {
    }

    public async void CreateAsync()
    {
      var arrangementIndices = GetArrangementDictionary();

      var suitPrefix = CreationData.Prefixes[CreationType.AnalyzedHonors];
      var files = Directory.GetFiles(WorkingDirectory).Where(f => f.Contains(suitPrefix));
      var lines = files.SelectMany(File.ReadAllLines);
      var wordWithValues = lines.Select(s => CreateWord(s, arrangementIndices));
      await CreateAsync(wordWithValues, "HonorClassifier.bin");
    }

    private Dictionary<string, int> GetArrangementDictionary()
    {
      var arrangementFile = Path.Combine(WorkingDirectory, "ArrangementCombinations.txt");
      var lines = File.ReadAllLines(arrangementFile);
      var withIndices = lines.Select((a, i) => new {Arrangement = a, Index = i});
      return withIndices.ToDictionary(p => p.Arrangement, p => p.Index);
    }

    private static WordWithValue CreateWord(string line, Dictionary<string, int> arrangementIndices)
    {
      var index = line.IndexOf('(');
      var a = line.Substring(0, index);
      var b = line.Substring(index);
      var word = a.Select(c => c - '0');
      var value = arrangementIndices[b];
      return new WordWithValue(word, value);
    }
  }
}