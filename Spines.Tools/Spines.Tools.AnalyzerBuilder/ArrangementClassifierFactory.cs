using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Spines.Mahjong.Analysis.Classification;
using Spines.Utility;

namespace Spines.Tools.AnalyzerBuilder
{
  internal class ArrangementClassifierFactory
  {
    private readonly IProgressManager _progressManager;
    private readonly string _workingDirectory;

    public ArrangementClassifierFactory(IProgressManager progressManager, string workingDirectory)
    {
      _progressManager = progressManager;
      _workingDirectory = workingDirectory;
    }

    public async void CreateAsync()
    {
      var wordsFile = Path.Combine(_workingDirectory, "ArrangementWords.txt");
      var lines = File.ReadAllLines(wordsFile);
      var words = lines.Select(CreateWord).ToList();
      _progressManager.Reset(words.Count);

      var alphabetSize = words.SelectMany(w => w.Word).Max() + 1;
      var classifier = await Task.Run(() => Create(words, alphabetSize));

      var classifierFile = Path.Combine(_workingDirectory, "ArrangementClassifier.bin");
      using (var fileStream = new FileStream(classifierFile, FileMode.Create))
      {
        var formatter = new BinaryFormatter();
        formatter.Serialize(fileStream, classifier);
      }
      _progressManager.Done();
    }

    private Classifier Create(IEnumerable<WordWithValue> words, int alphabetSize)
    {
      var builder = new ClassifierBuilder(alphabetSize, 4);
      foreach (var word in words)
      {
        builder.AddWords(word.Yield());
        _progressManager.Increment();
      }
      return builder.CreateClassifier();
    }

    private static WordWithValue CreateWord(string line)
    {
      var a = line.Split(':');
      var b = a[0].Split(',').Select(c => Convert.ToInt32(c));
      return new WordWithValue(b, Convert.ToInt32(a[1]));
    }
  }
}
