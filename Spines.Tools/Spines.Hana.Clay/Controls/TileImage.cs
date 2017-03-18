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

    private static readonly string PerspectivePath = Path.GetFullPath(@"Resources/Tiles/Perspective");
    private static readonly string FlatPath = Path.GetFullPath(@"Resources/Tiles/Flat");

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
      var tile = Tile.Value;
      var c = SuitCharacters[tile.Suit];
      var i = tile.Index + 1;
      var path = GetPath(c, i, tile.Location);
      var image = new BitmapImage(new Uri(path, UriKind.Absolute));
      image.Freeze();
      return image;
    }

    private static string GetPath(char c, int tileNumber, TileLocation location)
    {
      switch (location)
      {
        case TileLocation.Concealed:
          return Path.Combine(PerspectivePath, $"0{c}{tileNumber}.png");
        case TileLocation.Discarded:
        case TileLocation.Melded:
          return Path.Combine(PerspectivePath, $"1{c}{tileNumber}.png");
        case TileLocation.Added:
        case TileLocation.Called:
          return Path.Combine(PerspectivePath, $"2{c}{tileNumber}.png");
        case TileLocation.FaceDown:
          return Path.Combine(PerspectivePath, "1j9.png");
        default:
          return Path.Combine(FlatPath, $"{c}{tileNumber}.png");
      }
    }

    private static void OnChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TileImage) d).UpdateSource();
    }
  }
}