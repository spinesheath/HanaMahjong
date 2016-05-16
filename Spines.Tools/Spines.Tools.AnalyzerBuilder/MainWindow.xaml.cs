// Spines.Tools.AnalyzerBuilder.MainWindow.xaml.cs
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using Spines.Mahjong.Analysis.Classification;
using Spines.Mahjong.Analysis.Combinations;
using Spines.Utility;

namespace Spines.Tools.AnalyzerBuilder
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    private readonly IDictionary<CreationType, int> _creationCounts = new Dictionary<CreationType, int>
    {
      {CreationType.Concealed, 15},
      {CreationType.Melded, 5},
      {CreationType.Mixed, 15},
      {CreationType.Analyzed, 15},
      {CreationType.ArrangementCsv, 15}
    };

    private readonly IDictionary<CreationType, Func<int, IEnumerable<string>>> _creatorFuncs = new Dictionary<CreationType, Func<int, IEnumerable<string>>>
    {
      {CreationType.Concealed, CreateConcealedCombinations},
      {CreationType.Melded, CreateMeldedCombinations},
      {CreationType.Mixed, CreateMixedCombinations},
      {CreationType.Analyzed, CreateAnalyzedCombinations},
      {CreationType.ArrangementCsv, CreateArrangementCsvLines}
    };

    private readonly IDictionary<CreationType, string> _prefixes = new Dictionary<CreationType, string>
    {
      {CreationType.Concealed, "ConcealedSuitCombinations"},
      {CreationType.Melded, "MeldedSuitCombinations"},
      {CreationType.Mixed, "MixedSuitCombinations"},
      {CreationType.Analyzed, "AnayzedSuitCombinations"},
      {CreationType.ArrangementCsv, "ArrangementCsv"}
    };

    private readonly IDictionary<CreationType, string> _fileTypes = new Dictionary<CreationType, string>
    {
      {CreationType.Concealed, "txt"},
      {CreationType.Melded, "txt"},
      {CreationType.Mixed, "txt"},
      {CreationType.Analyzed, "txt"},
      {CreationType.ArrangementCsv, "csv"}
    };

    /// <summary>
    /// Constructor.
    /// </summary>
    public MainWindow()
    {
      InitializeComponent();
    }

    private static IEnumerable<string> CreateAnalyzedCombinations(int count)
    {
      if (count <= 6)
      {
        yield break;
      }
      var maxMelds = (14 - count) / 3;
      for (var meldCount = 0; meldCount <= maxMelds; ++meldCount)
      {
        var meldedCreator = new MeldedSuitCombinationsCreator();
        var meldedCombinations = meldedCreator.Create(meldCount);
        foreach (var meldedCombination in meldedCombinations)
        {
          var m = meldCount;
          var concealedCreator = new ConcealedSuitCombinationCreator();
          var combinations = concealedCreator.Create(count, meldedCombination);
          foreach (var combination in combinations)
          {
            var analyzer = new SuitAnalyzer(combination, meldedCombination, m);
            var arrangements = analyzer.Analyze();
            var formattedArrangements = arrangements.Select(a => $"({a.JantouValue},{a.MentsuCount},{a.MentsuValue})");
            var arrangementsString = string.Join("", formattedArrangements);
            yield return $"{m}{string.Join("", meldedCombination.Counts)}{string.Join("", combination.Counts)}{arrangementsString}";
          }
        }
      }
    }

    private void Create(CreationType creationType)
    {
      var workingDirectory = GetWorkingDirectory();
      if (workingDirectory == null)
      {
        return;
      }

      CreateCombinationsAsync(workingDirectory, creationType);
    }

    private void UniqueArrangementCombinations(object sender, RoutedEventArgs e)
    {
      var workingDirectory = GetWorkingDirectory();
      if (workingDirectory == null)
      {
        return;
      }

      var prefix = _prefixes[CreationType.Analyzed];
      var files = Directory.GetFiles(workingDirectory).Where(f => f.Contains(prefix));
      var lines = files.SelectMany(File.ReadAllLines);
      var combinations = lines.Select(line => line.Substring(19));
      var unique = combinations.Distinct();
      var ordered = unique.OrderBy(u => u);
      var targetFile = Path.Combine(workingDirectory, "ArrangementCombinations.txt");
      File.WriteAllLines(targetFile, ordered);
    } 

    private void CreateArrangementWords(object sender, RoutedEventArgs e)
    {
      var workingDirectory = GetWorkingDirectory();
      if (workingDirectory == null)
      {
        return;
      }

      CreateArrangementWordsAsync(workingDirectory);
    }

    private async void CreateArrangementWordsAsync(string workingDirectory)
    {
      ProgressBar.Minimum = 0;
      ProgressBar.Maximum = 100;
      ProgressBar.Value = 0;
      await Task.Run(() => CreateArrangementWords(workingDirectory));
    }

    private void CreateArrangementWords(string workingDirectory)
    {
      var arrangementFile = Path.Combine(workingDirectory, "ArrangementCombinations.txt");
      var words = CreateWords(arrangementFile);
      var wordsFile = Path.Combine(workingDirectory, "ArrangementWords.txt");
      File.WriteAllLines(wordsFile, words.Select(w => string.Join(",", w.Word) + ":" + w.Value));
    }

    private void CreateArrangementClassifier(object sender, RoutedEventArgs e)
    {
      var workingDirectory = GetWorkingDirectory();
      if (workingDirectory == null)
      {
        return;
      }

      CreateArrangementClassifierAsync(workingDirectory);
    }

    private async void CreateArrangementClassifierAsync(string workingDirectory)
    {
      var wordsFile = Path.Combine(workingDirectory, "ArrangementWords.txt");
      var lines = File.ReadAllLines(wordsFile);
      var words = lines.Select(CreateWord).ToList();
      ProgressBar.Minimum = 0;
      ProgressBar.Maximum = words.Count;
      ProgressBar.Value = 0;

      var alphabetSize = words.SelectMany(w => w.Word).Max() + 1;
      var classifier = await Task.Run(() => CreateArrangementClassifier(words, alphabetSize));

      var classifierFile = Path.Combine(workingDirectory, "ArrangementClassifier.bin");
      using (var fileStream = new FileStream(classifierFile, FileMode.Create))
      {
        var formatter = new BinaryFormatter();
        formatter.Serialize(fileStream, classifier);
      }
    }

    private Classifier CreateArrangementClassifier(IEnumerable<WordWithValue> words, int alphabetSize)
    {
      var builder = new ClassifierBuilder(alphabetSize, 4);
      foreach (var word in words)
      {
        builder.AddWords(word.Yield());
        IncrementProgressBar();
      }
      return builder.CreateClassifier();
    }

    private static WordWithValue CreateWord(string line)
    {
      var a = line.Split(':');
      var b = a[0].Split(',').Select(c => Convert.ToInt32(c));
      return new WordWithValue(b, Convert.ToInt32(a[1]));
    }

    private IEnumerable<WordWithValue> CreateWords(string arrangementsFile)
    {
      var lines = File.ReadAllLines(arrangementsFile);
      var arrangements = lines.Select(ParseArrangements).Select(a => a.ToList()).ToList();
      var alphabetSize = arrangements.Count;
      var language = CreateBaseLanguage(alphabetSize);
      var tilesInArrangements = arrangements.Select(a => a.Max(b => b.TotalValue)).ToList();

      var max = alphabetSize * ((long)alphabetSize + 1) * ((long)alphabetSize + 2) * ((long)alphabetSize + 3) / 24;
      long count = 0;
      long current = 0;
      foreach (var word in language)
      {
        var sumOfTiles = word.Sum(c => tilesInArrangements[c]);
        if (sumOfTiles <= 14)
        {
          var analyzer = new ArrangementAnalyzer();
          foreach (var character in word)
          {
            analyzer.AddSetOfArrangements(arrangements[character]);
          }
          var shanten = analyzer.CalculateShanten();
          if (shanten < 9)
          {
            yield return new WordWithValue(word, shanten);
          }
        }

        count += 1;
        if (current * (max / 100) < count)
        {
          IncrementProgressBar();
          current += 1;
        }
      }
    }

    private static IEnumerable<IList<int>> CreateBaseLanguage(int alphabetSize)
    {
      for (var a = 0; a < alphabetSize; ++a)
      {
        for (var b = a; b < alphabetSize; ++b)
        {
          for (var c = b; c < alphabetSize; ++c)
          {
            for (var d = c; d < alphabetSize; ++d)
            {
              yield return new [] {a, b, c, d};
            }
          }
        }
      }
    }

    private static IEnumerable<Arrangement> ParseArrangements(string line)
    {
      var regex = new Regex(@"\((\d+),(\d+),(\d+)\)");
      var matches = regex.Matches(line);
      foreach (Match match in matches)
      {
        var jantouValue = Convert.ToInt32(match.Groups[1].Value);
        var mentsuCount = Convert.ToInt32(match.Groups[2].Value);
        var mentsuValue = Convert.ToInt32(match.Groups[3].Value);
        yield return new Arrangement(jantouValue, mentsuCount, mentsuValue);
      }
    }

    private async void CreateCombinationsAsync(string workingDirectory, CreationType creationType)
    {
      var creationCount = _creationCounts[creationType];
      ProgressBar.Minimum = 0;
      ProgressBar.Maximum = creationCount;
      ProgressBar.Value = 0;
      var counts = Enumerable.Range(0, creationCount);
      await Task.Run(() => Parallel.ForEach(counts, c => CreateCombinationFile(c, workingDirectory, creationType)));
    }

    private void CreateCombinationFile(int count, string workingDirectory, CreationType creationType)
    {
      var lines = _creatorFuncs[creationType].Invoke(count);
      WriteToFile(workingDirectory, count, lines, creationType);
      IncrementProgressBar();
    }

    private void WriteToFile(string workingDirectory, int count, IEnumerable<string> lines, CreationType creationType)
    {
      var prefix = _prefixes[creationType];
      var fileType = _fileTypes[creationType];
      var fileName = $"{prefix}_{count}.{fileType}";
      var path = Path.Combine(workingDirectory, fileName);
      File.WriteAllLines(path, lines);
    }

    private static IEnumerable<string> CreateLines(IEnumerable<Combination> combinations)
    {
      return combinations.Select(c => string.Join(string.Empty, c.Counts));
    }

    private static IEnumerable<string> CreateConcealedCombinations(int count)
    {
      return CreateLines(new ConcealedSuitCombinationCreator().Create(count));
    }

    private static IEnumerable<string> CreateMeldedCombinations(int count)
    {
      return CreateLines(new MeldedSuitCombinationsCreator().Create(count));
    }

    private static IEnumerable<string> CreateMixedCombinations(int count)
    {
      var maxMelds = (14 - count) / 3;
      for (var meldCount = 0; meldCount <= maxMelds; ++meldCount)
      {
        var meldedCreator = new MeldedSuitCombinationsCreator();
        var meldedCombinations = meldedCreator.Create(meldCount);
        foreach (var meldedCombination in meldedCombinations)
        {
          var m = meldCount;
          var concealedCreator = new ConcealedSuitCombinationCreator();
          var combinations = concealedCreator.Create(count, meldedCombination);
          foreach (var combination in combinations)
          {
            yield return $"{m}{string.Join("", meldedCombination.Counts)}{string.Join("", combination.Counts)}";
          }
        }
      }
    }

    private void IncrementProgressBar()
    {
      Dispatcher.Invoke(() => ProgressBar.Value += 1);
    }

    private void CreateConcealedCombinations(object sender, RoutedEventArgs e)
    {
      Create(CreationType.Concealed);
    }

    private void CreateMeldedCombinations(object sender, RoutedEventArgs e)
    {
      Create(CreationType.Melded);
    }

    private void CreateMixedCombinations(object sender, RoutedEventArgs e)
    {
      Create(CreationType.Mixed);
    }

    private void AnlyzeCombinations(object sender, RoutedEventArgs e)
    {
      Create(CreationType.Analyzed);
    }

    private void CreateArrangementCsv(object sender, RoutedEventArgs e)
    {
      Create(CreationType.ArrangementCsv);
    }

    private static IEnumerable<string> CreateArrangementCsvLines(int tileCount)
    {
      var arrangements = CreateAllArrangements(tileCount).ToList();
      var comparer = new ArrangementComparer(tileCount);
      yield return ";" + string.Join(";", arrangements);
      foreach (var a in arrangements)
      {
        var sb = new StringBuilder();
        sb.Append(a);
        sb.Append(";");
        foreach (var b in arrangements)
        {
          var worse = comparer.IsWorseThan(a, b);
          var better = comparer.IsWorseThan(b, a);
          if (worse && better)
          {
            sb.Append("x;");
          }
          else if(better)
          {
            sb.Append("b;");
          }
          else if (worse)
          {
            sb.Append("w;");
          }
          else
          {
            sb.Append("o;");
          }
        }
        yield return sb.ToString();
      }
    }

    private string GetWorkingDirectory()
    {
      using (var dialog = new CommonOpenFileDialog())
      {
        dialog.IsFolderPicker = true;
        dialog.EnsurePathExists = true;
        var result = dialog.ShowDialog(this);
        if (result != CommonFileDialogResult.Ok)
        {
          return null;
        }
        return dialog.FileNames.Single();
      }
    }

    private static IEnumerable<Arrangement> CreateAllArrangements(int totalTiles)
    {
      for (var jantouValue = 0; jantouValue <= 2; ++jantouValue)
      {
        for (var mentsuCount = 0; mentsuCount <= 4; ++mentsuCount)
        {
          for (var mentsuValue = mentsuCount; mentsuValue <= mentsuCount * 3; ++mentsuValue)
          {
            var arrangement = new Arrangement(jantouValue, mentsuCount, mentsuValue);
            if (arrangement.TotalValue <= totalTiles)
            {
              yield return arrangement;
            }
          }
        }
      }
    }
  }
}