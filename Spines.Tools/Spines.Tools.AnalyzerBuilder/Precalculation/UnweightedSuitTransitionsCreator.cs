// Spines.Tools.AnalyzerBuilder.UnweightedSuitTransitionsCreator.cs
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Spines.Mahjong.Analysis.Classification;
using Spines.Utility;

namespace Spines.Tools.AnalyzerBuilder.Precalculation
{
  internal class UnweightedSuitTransitionsCreator
  {
    public UnweightedSuitTransitionsCreator(string workingDirectory)
    {
      _workingDirectory = workingDirectory;
    }

    public IEnumerable<int> Create()
    {
      var path = Path.Combine(_workingDirectory, "UnweightedSuitTransitions.txt");
      if (File.Exists(path))
      {
        return File.ReadAllLines(path).Select(line => Convert.ToInt32(line, CultureInfo.InvariantCulture));
      }
      var language = new CompactAnalyzedDataCreator(_workingDirectory).CreateSuitWords();
      var fullLanguage = CreateFullLanguage(language);
      var builder = new ClassifierBuilder();
      builder.SetLanguage(fullLanguage, 5, 19);
      var transitions = builder.Transitions;
      var lines = transitions.Select(t => t.ToString(CultureInfo.InvariantCulture));
      File.WriteAllLines(path, lines);
      return transitions;
    }

    private readonly string _workingDirectory;

    private static IEnumerable<WordWithValue> CreateFullLanguage(IEnumerable<WordWithValue> language)
    {
      foreach (var word in language)
      {
        yield return word;
        var mc = word[0];
        var m = word.Skip(1).Take(9).Reverse();
        var c = word.Skip(10).Reverse();
        var w = mc.Concat(m).Concat(c);
        var mirrored = new WordWithValue(w, word.Value);
        if (!mirrored.SequenceEqual(word))
        {
          yield return mirrored;
        }
      }
    }
  }
}