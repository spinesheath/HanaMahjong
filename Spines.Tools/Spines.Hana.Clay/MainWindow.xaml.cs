// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using Spines.MachiKae.Dialogs;
using Spines.MachiKae.ViewModels;
using Spines.MachiKae.Views;

namespace Spines.MachiKae
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
  }
}