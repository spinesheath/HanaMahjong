// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Spines.Hana.Clay.Controls
{
  /// <summary>
  /// A button with a path as content.
  /// </summary>
  internal class PathButton : ButtonBase
  {
    public PathButton()
    {
      Background = Brushes.White;
    }

    public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(Geometry), typeof(PathButton), new PropertyMetadata(default(Geometry)));

    public Geometry Data
    {
      get { return (Geometry) GetValue(DataProperty); }
      set { SetValue(DataProperty, value); }
    }

    static PathButton()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PathButton), new FrameworkPropertyMetadata(typeof(PathButton)));
    }
  }
}