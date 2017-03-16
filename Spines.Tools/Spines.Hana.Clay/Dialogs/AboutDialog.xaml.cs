// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Windows;

namespace Spines.MachiKae.Dialogs
{
  /// <summary>
  /// Interaction logic for AboutDialog.xaml
  /// </summary>
  internal partial class AboutDialog : Window
  {
    public AboutDialog()
    {
      InitializeComponent();
    }

    private void OnGoToTileImages(object sender, RoutedEventArgs e)
    {
      try
      {
        Process.Start("http://suzume.hakata21.com/5zats/haiga.html");
      }
        // ReSharper disable once EmptyGeneralCatchClause
      catch
      {
      }
    }
  }
}