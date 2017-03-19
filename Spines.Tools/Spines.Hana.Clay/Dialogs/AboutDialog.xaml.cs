// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Spines.Hana.Clay.Dialogs
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
      OpenUrl("http://suzume.hakata21.com/5zats/haiga.html");
    }

    private void OnGoToHanaMahjong(object sender, RoutedEventArgs e)
    {
      OpenUrl("https://github.com/spinesheath/HanaMahjong");
    }

    private void OnClose(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
      DragMove();
    }

    private static void OpenUrl(string url)
    {
      try
      {
        Process.Start(url);
      }
      // ReSharper disable once EmptyGeneralCatchClause
      catch
      {
      }
    }
  }
}