// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Spines.Hana.Clay.Controls
{
  internal class FlatButton : ButtonBase
  {
    public FlatButton()
    {
      Background = Brushes.White;
    }

    public Geometry Data
    {
      get { return (Geometry) GetValue(DataProperty); }
      set { SetValue(DataProperty, value); }
    }

    public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(Geometry),
      typeof(FlatButton), new PropertyMetadata(default(Geometry), OnDataChanged));

    static FlatButton()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(FlatButton), new FrameworkPropertyMetadata(typeof(FlatButton)));
    }

    private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var b = (FlatButton) d;
      var stroke = (SolidColorBrush) b.FindResource("FennelPrimaryMedium");
      b.Content = new Path
      {
        Data = b.Data,
        StrokeThickness = 5,
        StrokeStartLineCap = PenLineCap.Round,
        StrokeEndLineCap = PenLineCap.Round,
        StrokeLineJoin = PenLineJoin.Round,
        Stroke = stroke
      };
    }
  }
}