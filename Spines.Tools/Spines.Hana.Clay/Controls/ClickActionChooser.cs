// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Spines.Hana.Clay.Controls
{
  /// <summary>
  /// Either executes a command on click or opens an action chooser if the button is held down for a while.
  /// </summary>
  [ContentProperty(nameof(Content))]
  internal class ClickActionChooser : Control
  {
    public ICommand Command
    {
      get { return (ICommand) GetValue(CommandProperty); }
      set { SetValue(CommandProperty, value); }
    }

    public object CommandParameter
    {
      get { return GetValue(CommandParameterProperty); }
      set { SetValue(CommandParameterProperty, value); }
    }

    public object Content
    {
      get { return GetValue(ContentProperty); }
      set { SetValue(ContentProperty, value); }
    }

    public IEnumerable<object> Actions
    {
      get { return (IEnumerable<object>) GetValue(ActionsProperty); }
      set { SetValue(ActionsProperty, value); }
    }

    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
      "Content", typeof(object), typeof(ClickActionChooser), new PropertyMetadata(default(object)));

    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
      "Command", typeof(ICommand), typeof(ClickActionChooser), new PropertyMetadata(default(ICommand)));

    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
      "CommandParameter", typeof(object), typeof(ClickActionChooser), new PropertyMetadata(default(object)));

    public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register(
      "Actions", typeof(IEnumerable<object>), typeof(ClickActionChooser), new PropertyMetadata(default(IEnumerable<object>)));

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);
      _actionChooser = CreateActionChooser();
      OpenChooserAsync();
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
      base.OnMouseEnter(e);
      if (e.LeftButton != MouseButtonState.Pressed)
      {
        return;
      }
      _actionChooser = CreateActionChooser();
      OpenChooserAsync();
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonUp(e);

      if (_actionChooser != null)
      {
        Command?.Execute(Actions.FirstOrDefault());

        AdornerLayer.GetAdornerLayer(this).Remove(_actionChooser);
        _actionChooser = null;
      }
      else
      {
        Command?.Execute(Actions.FirstOrDefault());
      }
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
      base.OnMouseLeave(e);

      if (_actionChooser != null && !_actionChooser.IsMouseOver)
      {
        AdornerLayer.GetAdornerLayer(this).Remove(_actionChooser);
        _actionChooser = null;
      }
    }

    static ClickActionChooser()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ClickActionChooser), new FrameworkPropertyMetadata(typeof(ClickActionChooser)));
    }

    private Adorner _actionChooser;

    private async void OpenChooserAsync()
    {
      await Task.Delay(200);
      if (Mouse.LeftButton == MouseButtonState.Pressed && _actionChooser != null)
      {
        AdornerLayer.GetAdornerLayer(this).Add(_actionChooser);
      }
    }

    private class ActionChooserAdorner : Adorner
    {
      public ActionChooserAdorner(UIElement adornedElement)
        : base(adornedElement)
      {
      }

      protected override void OnRender(DrawingContext drawingContext)
      {
        drawingContext.DrawEllipse(CreateBrush(), null, new Point(ActualWidth / 2, ActualHeight / 2), 10, 10);
      }

      private SolidColorBrush CreateBrush()
      {
        var accent = TryFindResource("AccentMedium") as SolidColorBrush;
        if (null == accent)
        {
          var brush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));
          brush.Freeze();
          return brush;
        }
        var accentBased = new SolidColorBrush(accent.Color) { Opacity = 0.5 };
        accentBased.Freeze();
        return accentBased;
      }
    }

    private Adorner CreateActionChooser()
    {
      if (null == Actions)
        return null;
      var actions = Actions.ToList();
      if (actions.Count == 0)
      {
        return null;
      }
      if (actions.Count == 1)
      {
        return new ActionChooserAdorner(this);
      }
      //var path = new Path { Data = Geometry.Parse("M 100,200 C 100,25 400,350 400,175 H 280") };

      return new ActionChooserAdorner(this);
    }


  }
}