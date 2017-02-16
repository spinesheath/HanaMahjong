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

using System.Linq;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using Spines.Tools.AnalyzerBuilder.Precalculation;

namespace Spines.Tools.AnalyzerBuilder
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : IProgressManager
  {
    /// <summary>
    /// Constructor.
    /// </summary>
    public MainWindow()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Resets the ProgressBar.
    /// </summary>
    public void Reset(int max)
    {
      ProgressBar.Minimum = 0;
      ProgressBar.Maximum = max;
      ProgressBar.Value = 0;
    }

    /// <summary>
    /// Increments the ProgressBar.
    /// </summary>
    public void Increment()
    {
      IncrementProgressBar();
    }

    /// <summary>
    /// Shows that the process is done.
    /// </summary>
    public void Done()
    {
      ProgressBar.Value = 0;
    }

    private void IncrementProgressBar()
    {
      Dispatcher.Invoke(() => ProgressBar.Value += 1);
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

    private void AllAtOnce(object sender, RoutedEventArgs e)
    {
      var workingDirectory = GetWorkingDirectory();
      if (null == workingDirectory)
      {
        return;
      }
      new Creator(workingDirectory).Create();
    }
  }
}