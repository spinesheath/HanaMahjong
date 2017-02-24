// Spines.Tools.AnalyzerBuilder.OrderedArrangementsCreator.cs
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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spines.Mahjong.Analysis.Classification;
using Spines.Mahjong.Analysis.Combinations;

namespace Spines.Tools.AnalyzerBuilder.Precalculation
{
  internal class OrderedArrangementsCreator
  {
    private readonly string _workingDirectory;

    public OrderedArrangementsCreator(string workingDirectory)
    {
      _workingDirectory = workingDirectory;
    }

    /// <summary>
    /// Creates a file with all arrangements in the order that is used for the alphabet of the classifier.
    /// </summary>
    public IEnumerable<IList<Arrangement>> Create()
    {
      var orderedPath = Path.Combine(_workingDirectory, "OrderedArrangements.txt");
      if (File.Exists(orderedPath))
      {
        return File.ReadAllLines(orderedPath).Select(Arrangement.MultipleFromString).Select(a => a.ToList());
      }

      var words = new ArrangementWordCreator(_workingDirectory).CreateUnordered();
      var classifierBuilder = new ClassifierBuilder();
      classifierBuilder.SetLanguage(words);

      var transitions = classifierBuilder.CreateTransitions().ToList();
      var resultIndices = new HashSet<int>(classifierBuilder.GetResultIndexes());
      var alphabetSize = classifierBuilder.AlphabetSize;

      var nullTransitions = Transition.CountNullTransitions(transitions, alphabetSize, t => transitions[t] == 0, t => resultIndices.Contains(t));
      var ordered = nullTransitions.Select((n, i) => new {n, i}).OrderBy(p => p.n).Select(p => p.i).ToList();
      
      var oldArrangements = new CompactAnalyzedDataCreator(_workingDirectory).GetUniqueArrangements().ToList();
      var newArrangements = ordered.Select(i => oldArrangements[i]).ToList();

      var lines = newArrangements.Select(a => string.Join("", a)).ToList();
      File.WriteAllLines(orderedPath, lines);
      return newArrangements;
    }
  }
}