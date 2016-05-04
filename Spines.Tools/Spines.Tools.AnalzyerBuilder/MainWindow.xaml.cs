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
  public partial class MainWindow : Window
  {
    /// <summary>
    /// Constructor.
    /// </summary>
    public MainWindow()
    {
      InitializeComponent();
    }

    private void CreateCombinations(object sender, RoutedEventArgs e)
    {
      ProgressBar.Minimum = 0;
      ProgressBar.Maximum = 15;
      ProgressBar.Value = 0;

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
        CreateCombinationsAsync(workingDirectory);
      }
    }

    private async void CreateCombinationsAsync(string workingDirectory)
    {
      var counts = Enumerable.Range(0, 15);
      await Task.Run(() => Parallel.ForEach(counts, c => CreateCombinationFile(c, workingDirectory)));
    }

    private void CreateCombinationFile(int count, string workingDirectory)
    {
      var creator = new ConcealedSuitCombinationCreator();
      var combinations = creator.Create(count);
      var text = string.Join(string.Empty, combinations.SelectMany(c => c.Counts));
      var fileName = $"ConcealedSuitCombinations_{count}.txt";
      var path = Path.Combine(workingDirectory, fileName);
      File.WriteAllText(path, text);

      IncrementProgressBar();
    }

    private void IncrementProgressBar()
    {
      Dispatcher.Invoke(() => ProgressBar.Value += 1);
    }
  }
}