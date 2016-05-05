﻿// Spines.Tools.AnalyzerBuilder.MainWindow.xaml.cs
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
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using Spines.Mahjong.Analysis.Combinations;

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
      {CreationType.Mixed, 15}
    };

    private readonly IDictionary<CreationType, Func<int, IEnumerable<string>>> _creatorFuncs = new Dictionary<CreationType, Func<int, IEnumerable<string>>>
    {
      {CreationType.Concealed, CreateConcealedCombinations},
      {CreationType.Melded, CreateMeldedCombinations},
      {CreationType.Mixed, CreateMixedCombinations}
    };

    private readonly IDictionary<CreationType, string> _prefixes = new Dictionary<CreationType, string>
    {
      {CreationType.Concealed, "ConcealedSuitCombinations"},
      {CreationType.Melded, "MeldedSuitCombinations"},
      {CreationType.Mixed, "MixedSuitCombinations"}
    };

    /// <summary>
    /// Constructor.
    /// </summary>
    public MainWindow()
    {
      InitializeComponent();
    }

    private void AnlyzeCombinations(object sender, RoutedEventArgs e)
    {
      
    }

    private void Create(CreationType creationType)
    {
      using (var dialog = new CommonOpenFileDialog())
      {
        dialog.IsFolderPicker = true;
        dialog.EnsurePathExists = true;
        var result = dialog.ShowDialog(this);
        if (result != CommonFileDialogResult.Ok)
        {
          return;
        }
        var workingDirectory = dialog.FileNames.Single();
        CreateCombinationsAsync(workingDirectory, creationType);
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
      var prefix = _prefixes[creationType];
      WriteToFile(workingDirectory, prefix, count, lines);
      IncrementProgressBar();
    }

    private static void WriteToFile(string workingDirectory, string prefix, int count, IEnumerable<string> lines)
    {
      var fileName = $"{prefix}_{count}.txt";
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
  }
}