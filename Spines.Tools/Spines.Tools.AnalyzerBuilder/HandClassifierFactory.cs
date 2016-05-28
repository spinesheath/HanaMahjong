// Spines.Tools.AnalyzerBuilder.HandClassifierFactory.cs
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
using Spines.Utility;

namespace Spines.Tools.AnalyzerBuilder
{
  internal class HandClassifierFactory : ClassifierFactoryBase
  {
    private readonly string _sourceFilePrefix;
    private readonly string _targetFile;

    private HandClassifierFactory(IProgressManager progressManager, string workingDirectory, string sourceFilePrefix,
      string targetFile)
      : base(progressManager, workingDirectory)
    {
      _sourceFilePrefix = sourceFilePrefix;
      _targetFile = targetFile;
    }

    public static HandClassifierFactory CreateSuitClassifierFactory(IProgressManager progressManager, string workingDirectory, string filename)
    {
      var prefix = CreationData.Prefixes[CreationType.AnalyzedSuit];
      return new HandClassifierFactory(progressManager, workingDirectory, prefix, filename);
    }

    public static HandClassifierFactory CreateHonorClassifierFactory(IProgressManager progressManager, string workingDirectory, string filename)
    {
      var prefix = CreationData.Prefixes[CreationType.AnalyzedHonors];
      return new HandClassifierFactory(progressManager, workingDirectory, prefix, filename);
    }

    public async void CreateAsync(bool mirrorWords)
    {
      var arrangementIndices = GetArrangementDictionary();

      var files = Directory.GetFiles(WorkingDirectory).Where(f => f.Contains(_sourceFilePrefix));
      var lines = files.SelectMany(File.ReadAllLines);
      var wordWithValues = lines.SelectMany(s => CreateSuitOrHonorWord(s, arrangementIndices, mirrorWords));
      await CreateAsync(wordWithValues, _targetFile);
    }

    private static IEnumerable<WordWithValue> CreateSuitOrHonorWord(string line, IReadOnlyDictionary<string, int> arrangementIndices, bool mirrorWords)
    {
      var index = line.IndexOf('(');
      var hand = line.Substring(0, index);
      var arrangements = line.Substring(index);
      var word = hand.Select(c => c - '0');
      var value = arrangementIndices[arrangements];
      yield return new WordWithValue(word, value);

      if (!mirrorWords)
      {
        yield break;
      }

      var meldCount = hand[0];
      var tileTypes = (index - 1) / 2;
      var meldTiles = hand.Substring(1, tileTypes);
      var handTiles = hand.Substring(1 + tileTypes);
      var reverseMeldTiles = meldTiles.Select(c => c - '0').Reverse();
      var reverseHandTiles = handTiles.Select(c => c - '0').Reverse();
      var mirroredWord = (meldCount - '0').Yield().Concat(reverseMeldTiles).Concat(reverseHandTiles);
      yield return new WordWithValue(mirroredWord, value);
    }
  }
}