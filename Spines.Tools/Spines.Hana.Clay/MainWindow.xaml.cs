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
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      var mainView = new MainView {DataContext = new MainViewModel()};
      MainContent.Content = mainView;
    }

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
  }
}