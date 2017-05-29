// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Input;
using Spines.Hana.Clay.Dialogs;
using Spines.Hana.Clay.ViewModels;
using Spines.Hana.Clay.Views;

namespace Spines.Hana.Clay
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
  internal partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      OnGoToUkeIre(null, null);
    }

    private TableMainView _tableMainView;
    private UkeIreMainView _ukeIreMainView;

    private void OnAbout(object sender, RoutedEventArgs e)
    {
      var about = new AboutDialog();
      about.ShowDialog();
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
      DragMove();
    }

    private void OnClose(object sender, RoutedEventArgs e)
    {
      Application.Current.Shutdown();
    }

    private void OnMaximize(object sender, RoutedEventArgs e)
    {
      if (Application.Current.MainWindow.WindowState == WindowState.Normal)
      {
        Application.Current.MainWindow.WindowState = WindowState.Maximized;
        WindowBorder.Margin = new Thickness(6);
      }
      else
      {
        Application.Current.MainWindow.WindowState = WindowState.Normal;
        WindowBorder.Margin = new Thickness(0);
      }
    }

    private void OnMinimize(object sender, RoutedEventArgs e)
    {
      Application.Current.MainWindow.WindowState = WindowState.Minimized;
    }

    private void OnGoToTable(object sender, RoutedEventArgs e)
    {
      if (MainContent.Content is TableMainView)
      {
        return;
      }
      if (null == _tableMainView)
      {
        _tableMainView = new TableMainView {DataContext = new TableMainViewModel()};
      }
      MainContent.Content = _tableMainView;
    }

    private void OnGoToUkeIre(object sender, RoutedEventArgs e)
    {
      if (MainContent.Content is UkeIreMainView)
      {
        return;
      }
      if (null == _ukeIreMainView)
      {
        _ukeIreMainView = new UkeIreMainView {DataContext = new UkeIreMainViewModel()};
      }
      MainContent.Content = _ukeIreMainView;
    }
  }
}