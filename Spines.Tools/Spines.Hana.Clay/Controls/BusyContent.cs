// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Spines.Hana.Clay.Controls
{
  /// <summary>
  /// Waits until the content is done loading before displaying it.
  /// </summary>
  [ContentProperty(nameof(Content))]
  internal class BusyContent : Control
  {
    public bool IsLoading
    {
      get { return (bool) GetValue(IsLoadingProperty); }
      set { SetValue(IsLoadingProperty, value); }
    }

    public object Content
    {
      get { return GetValue(ContentProperty); }
      set { SetValue(ContentProperty, value); }
    }

    public object InternalContent
    {
      get { return GetValue(InternalContentProperty); }
      private set { SetValue(InternalContentProperty, value); }
    }

    public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(
      "IsLoading", typeof(bool), typeof(BusyContent), new PropertyMetadata(default(bool), OnChange));

    public static readonly DependencyProperty InternalContentProperty = DependencyProperty.Register(
      "InternalContent", typeof(object), typeof(BusyContent), new PropertyMetadata(default(object)));

    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
      "Content", typeof(object), typeof(BusyContent), new PropertyMetadata(default(object), OnChange));

    static BusyContent()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(BusyContent), new FrameworkPropertyMetadata(typeof(BusyContent)));
    }

    private void Update()
    {
      InternalContent = IsLoading ? new BusyIcon() : Content;
    }

    private static void OnChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var bc = (BusyContent) d;
      bc.Update();
    }
  }
}