// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

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