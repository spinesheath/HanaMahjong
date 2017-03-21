// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Hana.Clay.Controls
{
  /// <summary>
  /// Displays a hand.
  /// </summary>
  internal class HandDisplay : Control
  {
    public IEnumerable<Tile> Tiles
    {
      get { return (IEnumerable<Tile>) GetValue(TilesProperty); }
      set { SetValue(TilesProperty, value); }
    }

    public IEnumerable<Meld> Melds
    {
      get { return (IEnumerable<Meld>) GetValue(MeldsProperty); }
      set { SetValue(MeldsProperty, value); }
    }

    public Tile? Draw
    {
      get { return (Tile?) GetValue(DrawProperty); }
      set { SetValue(DrawProperty, value); }
    }

    public ICommand DiscardCommand
    {
      get { return (ICommand) GetValue(DiscardCommandProperty); }
      set { SetValue(DiscardCommandProperty, value); }
    }

    public static readonly DependencyProperty DrawProperty = DependencyProperty.Register(
      "Draw", typeof(Tile?), typeof(HandDisplay), new PropertyMetadata(default(Tile?)));

    public static readonly DependencyProperty DiscardCommandProperty = DependencyProperty.Register(
      "DiscardCommand", typeof(ICommand), typeof(HandDisplay), new PropertyMetadata(default(ICommand)));

    public static readonly DependencyProperty TilesProperty = DependencyProperty.Register(
      "Tiles", typeof(IEnumerable<Tile>), typeof(HandDisplay), new PropertyMetadata(default(IEnumerable<Tile>)));

    public static readonly DependencyProperty MeldsProperty = DependencyProperty.Register(
      "Melds", typeof(IEnumerable<Meld>), typeof(HandDisplay), new PropertyMetadata(default(IEnumerable<Meld>)));

    static HandDisplay()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(HandDisplay), new FrameworkPropertyMetadata(typeof(HandDisplay)));
    }
  }
}