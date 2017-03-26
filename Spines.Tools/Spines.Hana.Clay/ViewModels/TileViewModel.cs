// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Hana.Clay.ViewModels
{
  internal class TileViewModel : ViewModelBase
  {
    public TileViewModel(Tile tile, int x, int y, int playerPosition)
    {
      Tile = tile;
      X = x;
      Y = y;

      ImageFileName = GetFileName(tile, playerPosition);
      var path = Path.Combine(PerspectivePath, ImageFileName);
      var image = new BitmapImage(new Uri(path, UriKind.Absolute));
      if (tile.IsTsumokiri)
      {
        OverlayFileName = GetOverlayFileName(tile, playerPosition);
        var overlayPath = Path.Combine(PerspectivePath, OverlayFileName);
        var overlay = new BitmapImage(new Uri(overlayPath, UriKind.Absolute));
        var overlaid = AddOverlay(image, overlay);
        Source = overlaid;
      }
      else
      {
        image.Freeze();
        Source = image;
      }
    }

    public string OverlayFileName { get; }

    public string ImageFileName { get; }

    public Tile Tile { get; }

    public int X { get; }

    public int Y { get; }

    public ImageSource Source { get; }

    private static readonly string PerspectivePath = Path.GetFullPath(@"Resources/Tiles/Perspective");

    private static readonly Dictionary<Suit, char> SuitCharacters = new Dictionary<Suit, char>
    {
      {Suit.Manzu, 'm'},
      {Suit.Pinzu, 'p'},
      {Suit.Souzu, 's'},
      {Suit.Jihai, 'j'}
    };

    private static string GetOverlayFileName(Tile tile, int playerPosition)
    {
      switch (tile.Location)
      {
        case TileLocation.Discarded:
          return $"{1 + playerPosition % 2}o.png";
        case TileLocation.Riichi:
        case TileLocation.Added:
        case TileLocation.Called:
          return $"{1 + (1 + playerPosition) % 2}o.png";
        default:
          throw new InvalidOperationException("Not a valid location for a tsumokiri tile.");
      }
    }

    private static ImageSource AddOverlay(ImageSource image, ImageSource overlay)
    {
      var width = (int) image.Width;
      var height = (int) image.Height;
      var drawingVisual = new DrawingVisual();
      using (var context = drawingVisual.RenderOpen())
      {
        context.DrawImage(image, new Rect(0, 0, width, height));
        context.DrawImage(overlay, new Rect(0, 0, width, height));
      }
      var mergedImage = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
      mergedImage.Render(drawingVisual);
      mergedImage.Freeze();
      return mergedImage;
    }

    private static string GetFileName(Tile tile, int playerPosition)
    {
      var c = SuitCharacters[tile.Suit];
      var tileNumber = tile.Aka ? "e" : (tile.Index + 1).ToString();
      switch (tile.Location)
      {
        case TileLocation.Concealed:
          switch (playerPosition)
          {
            case 0:
              return $"0{c}{tileNumber}.png";
            case 1:
              return "vertr.png";
            case 2:
              return "0j9.png";
            case 3:
              return "vertl.png";
          }
          break;
        case TileLocation.Discarded:
          if (tile.IsGhost)
          {
            var d = 1 + playerPosition % 2;
            return $"{d}c{tile.CalledBy}.png";
          }
          return $"{1 + playerPosition}{c}{tileNumber}.png";
        case TileLocation.Melded:
          return $"{1 + playerPosition}{c}{tileNumber}.png";
        case TileLocation.Riichi:
          if (tile.IsGhost)
          {
            var d = 1 + (1 + playerPosition) % 2;
            return $"{d}c{tile.CalledBy}.png";
          }
          return $"{1 + (playerPosition + 1) % 4}{c}{tileNumber}.png";
        case TileLocation.Added:
        case TileLocation.Called:
          return $"{1 + (playerPosition + 1) % 4}{c}{tileNumber}.png";
        case TileLocation.FaceDown:
          return $"{1 + playerPosition}j9.png";
      }
      return $"{1 + playerPosition}j9.png";
    }
  }
}