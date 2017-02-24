// Spines.Tools.AnalyzerBuilder.TransitionsCreator.cs
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

namespace Spines.Tools.AnalyzerBuilder.Precalculation
{
  internal class TransitionsCreator
  {
    private readonly string _workingDirectory;

    /// <summary>
    /// Creates a new Instance of TransitionsCreator.
    /// </summary>
    /// <param name="workingDirectory">The directory where intermediate results are stored.</param>
    public TransitionsCreator(string workingDirectory)
    {
      _workingDirectory = workingDirectory;
    }

    public void CreateArrangementTransitions()
    {
      CreateTransitions("ArrangementTransitions.txt", () => new ArrangementWordCreator(_workingDirectory).CreateOrdered());
    }

    public void CreateSuitTransitions()
    {
      CreateTransitions("SuitTransitions.txt", () => new CompactAnalyzedDataCreator(_workingDirectory).CreateSuitWords());
    }

    public void CreateHonorTransitions()
    {
      CreateTransitions("HonorTransitions.txt", () => new CompactAnalyzedDataCreator(_workingDirectory).CreateHonorWords());
    }

    /// <summary>
    /// Creates the transitions file if it doesn't exist.
    /// </summary>
    private void CreateTransitions(string fileName, Func<IEnumerable<WordWithValue>> wordCreator)
    {
      var targetPath = Path.Combine(_workingDirectory, fileName);
      if (File.Exists(targetPath))
      {
        return;
      }

      var words = wordCreator();
      var compactedTransitions = GetCompactedTransitions(words);

      var lines = compactedTransitions.Select(t => t.ToString(CultureInfo.InvariantCulture));
      File.WriteAllLines(targetPath, lines);
    }

    private static IEnumerable<int> GetCompactedTransitions(IEnumerable<WordWithValue> words)
    {
      var classifierBuilder = new ClassifierBuilder();
      classifierBuilder.SetLanguage(words);
      var transitions = classifierBuilder.CreateTransitions().ToList();
      var resultIndices = new HashSet<int>(classifierBuilder.GetResultIndexes());
      var alphabetSize = classifierBuilder.AlphabetSize;
      return Transition.Compact(transitions, alphabetSize, t => transitions[t] == 0 && !resultIndices.Contains(t), t => resultIndices.Contains(t));
    }
  }
}