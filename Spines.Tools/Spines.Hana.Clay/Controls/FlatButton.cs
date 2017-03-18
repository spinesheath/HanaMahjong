// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Spines.Hana.Clay.Controls
{
  internal class FlatButton : ButtonBase
  {
    public FlatButton()
    {
      Background = (SolidColorBrush) FindResource("PrimaryLight");
      Foreground = (SolidColorBrush) FindResource("PrimaryMedium");
      MouseEnter += OnMouseEnter;
      MouseLeave += OnMouseLeave;
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

    private void OnMouseLeave(object sender, MouseEventArgs e)
    {
      Background = (SolidColorBrush) FindResource("PrimaryLight");
      Foreground = (SolidColorBrush) FindResource("PrimaryMedium");
    }

    private void OnMouseEnter(object sender, MouseEventArgs e)
    {
      Background = (SolidColorBrush) FindResource("AccentMedium");
      Foreground = (SolidColorBrush) FindResource("AccentLight");
    }

    private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var b = (FlatButton) d;
      b.UpdateContent();
    }

    private void UpdateContent()
    {
      var b = new Binding {Source = this, Path = new PropertyPath(nameof(Foreground))};
      var p = new Path
      {
        Data = Data,
        StrokeThickness = 5,
        StrokeStartLineCap = PenLineCap.Round,
        StrokeEndLineCap = PenLineCap.Round,
        StrokeLineJoin = PenLineJoin.Round
      };
      p.SetBinding(Shape.StrokeProperty, b);
      Content = p;
    }
  }
}