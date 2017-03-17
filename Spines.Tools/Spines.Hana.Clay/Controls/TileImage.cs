// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Hana.Clay.Controls
{
  /// <summary>
  /// An image of a tile.
  /// </summary>
  internal class TileImage : Control
  {
    public Tile? Tile
    {
      get { return (Tile?) GetValue(TileProperty); }
      set { SetValue(TileProperty, value); }
    }

    public ImageSource Source
    {
      get { return (ImageSource) GetValue(SourceProperty); }
      private set { SetValue(SourceProperty, value); }
    }

    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
      "Source", typeof(ImageSource), typeof(TileImage), new PropertyMetadata(default(ImageSource)));

    public static readonly DependencyProperty TileProperty = DependencyProperty.Register(
      "Tile", typeof(Tile?), typeof(TileImage), new PropertyMetadata(default(Tile?), OnChange));

    static TileImage()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TileImage), new FrameworkPropertyMetadata(typeof(TileImage)));
    }

    private static readonly string IconBasePath = Path.GetFullPath(@"Resources/Tiles/Perspective");

    private static readonly Dictionary<Suit, char> SuitCharacters = new Dictionary<Suit, char>
    {
      {Suit.Manzu, 'm'},
      {Suit.Pinzu, 'p'},
      {Suit.Souzu, 's'},
      {Suit.Jihai, 'j'}
    };

    private void UpdateSource()
    {
      Source = GetImageSource();
    }

    private ImageSource GetImageSource()
    {
      if (null == Tile)
      {
        return null;
      }
      var c = SuitCharacters[Tile.Value.Suit];
      var i = Tile.Value.Index + 1;
      var path = Path.Combine(IconBasePath, $"0{c}{i}.png");
      var image = new BitmapImage(new Uri(path, UriKind.Absolute));
      image.Freeze();
      return image;
    }

    private static void OnChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TileImage) d).UpdateSource();
    }
  }
}