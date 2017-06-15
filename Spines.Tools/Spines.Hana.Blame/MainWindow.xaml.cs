// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using Spines.Hana.Blame.Collection;

namespace Spines.Hana.Blame
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  internal partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      var d = new Downloader();
    }
  }
}