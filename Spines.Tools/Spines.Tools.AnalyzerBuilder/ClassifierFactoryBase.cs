// Spines.Tools.AnalyzerBuilder.ClassifierFactoryBase.cs
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
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Spines.Mahjong.Analysis.Classification;
using Spines.Utility;

namespace Spines.Tools.AnalyzerBuilder
{
  internal class ClassifierFactoryBase
  {
    public ClassifierFactoryBase(IProgressManager progressManager, string workingDirectory)
    {
      ProgressManager = progressManager;
      WorkingDirectory = workingDirectory;
    }

    protected IProgressManager ProgressManager { get; }

    protected string WorkingDirectory { get; }

    protected async Task CreateAsync(IEnumerable<WordWithValue> wordWithValues, string outputFileName)
    {
      var words = wordWithValues.ToList();
      ProgressManager.Reset(words.Count);

      var wordLength = words.First().Word.Count;
      var alphabetSize = words.SelectMany(w => w.Word).Max() + 1;
      var classifier = await Task.Run(() => Create(words, alphabetSize, wordLength));

      var classifierFile = Path.Combine(WorkingDirectory, outputFileName);
      using (var fileStream = new FileStream(classifierFile, FileMode.Create))
      {
        var formatter = new BinaryFormatter();
        formatter.Serialize(fileStream, classifier);
      }
      ProgressManager.Done();
    }

    private Classifier Create(IEnumerable<WordWithValue> words, int alphabetSize, int wordLength)
    {
      var builder = new ClassifierBuilder(alphabetSize, wordLength);
      foreach (var word in words)
      {
        builder.AddWords(word.Yield());
        ProgressManager.Increment();
      }
      return builder.CreateClassifier();
    }
  }
}