// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;

namespace Spines.Hana.Clay.Controls
{
  [TemplatePart(Name = "PART_decrementButton", Type = typeof(Button))]
  [TemplatePart(Name = "PART_incrementButton", Type = typeof(Button))]
  internal class IntegerRange : Control
  {
    public int Value
    {
      get { return (int) GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    public int Min
    {
      get { return (int) GetValue(MinProperty); }
      set { SetValue(MinProperty, value); }
    }

    public int Max
    {
      get { return (int) GetValue(MaxProperty); }
      set { SetValue(MaxProperty, value); }
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      RegisterClickEvent("PART_decrementButton", OnDecrement);
      RegisterClickEvent("PART_incrementButton", OnIncrement);
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
      nameof(Value), typeof(int), typeof(IntegerRange), new FrameworkPropertyMetadata(default(int), OnValueChanged) {BindsTwoWayByDefault = true});

    public static readonly DependencyProperty MinProperty = DependencyProperty.Register(
      nameof(Min), typeof(int), typeof(IntegerRange), new PropertyMetadata(default(int), OnMinChanged));

    public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(
      nameof(Max), typeof(int), typeof(IntegerRange), new PropertyMetadata(default(int), OnMaxChanged));

    static IntegerRange()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(IntegerRange), new FrameworkPropertyMetadata(typeof(IntegerRange)));
    }

    private void RegisterClickEvent(string name, RoutedEventHandler handler)
    {
      var button = Template.FindName(name, this) as Button;
      if (button != null)
      {
        button.Click += handler;
      }
    }

    private void OnDecrement(object sender, RoutedEventArgs e)
    {
      if (Value > Min)
      {
        Value -= 1;
      }
    }

    private void OnIncrement(object sender, RoutedEventArgs e)
    {
      if (Value < Max)
      {
        Value += 1;
      }
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var r = (IntegerRange) d;
      r.Value = Math.Min(r.Value, r.Max);
      r.Value = Math.Max(r.Value, r.Min);
    }

    private static void OnMinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var r = (IntegerRange) d;
      r.Max = Math.Max(r.Min, r.Max);
      r.Value = Math.Max(r.Min, r.Value);
    }

    private static void OnMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var r = (IntegerRange) d;
      r.Min = Math.Min(r.Min, r.Max);
      r.Value = Math.Min(r.Max, r.Value);
    }
  }
}