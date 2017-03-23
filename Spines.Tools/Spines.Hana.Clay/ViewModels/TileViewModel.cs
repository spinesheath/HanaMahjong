// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
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

      var path = GetPath(tile, playerPosition);
      var image = new BitmapImage(new Uri(path, UriKind.Absolute));
      image.Freeze();
      Source = image;
    }

    private static string GetPath(Tile tile, int playerPosition)
    {
      var c = SuitCharacters[tile.Suit];
      var tileNumber = tile.Aka ? "e" : (tile.Index + 1).ToString();
      var location = tile.Location;
      if (playerPosition == 0)
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
          case TileLocation.Riichi:
            return Path.Combine(PerspectivePath, $"2{c}{tileNumber}.png");
          case TileLocation.FaceDown:
            return Path.Combine(PerspectivePath, "1j9.png");
          default:
            return Path.Combine(FlatPath, $"{c}{tileNumber}.png");
        }
      }
      if (playerPosition == 1)
      {
        switch (location)
        {
          case TileLocation.Concealed:
            return Path.Combine(PerspectivePath, "vertr.png");
          case TileLocation.Discarded:
          case TileLocation.Melded:
            return Path.Combine(PerspectivePath, $"2{c}{tileNumber}.png");
          case TileLocation.Added:
          case TileLocation.Called:
          case TileLocation.Riichi:
            return Path.Combine(PerspectivePath, $"3{c}{tileNumber}.png");
          case TileLocation.FaceDown:
            return Path.Combine(PerspectivePath, "1j9.png");
          default:
            return Path.Combine(FlatPath, $"{c}{tileNumber}.png");
        }
      }
      if (playerPosition == 2)
      {
        switch (location)
        {
          case TileLocation.Concealed:
            return Path.Combine(PerspectivePath, "0j9.png");
          case TileLocation.Discarded:
          case TileLocation.Melded:
            return Path.Combine(PerspectivePath, $"3{c}{tileNumber}.png");
          case TileLocation.Added:
          case TileLocation.Called:
          case TileLocation.Riichi:
            return Path.Combine(PerspectivePath, $"4{c}{tileNumber}.png");
          case TileLocation.FaceDown:
            return Path.Combine(PerspectivePath, "1j9.png");
          default:
            return Path.Combine(FlatPath, $"{c}{tileNumber}.png");
        }
      }
      if (playerPosition == 3)
      {
        switch (location)
        {
          case TileLocation.Concealed:
            return Path.Combine(PerspectivePath, "vertl.png");
          case TileLocation.Discarded:
          case TileLocation.Melded:
            return Path.Combine(PerspectivePath, $"4{c}{tileNumber}.png");
          case TileLocation.Added:
          case TileLocation.Called:
          case TileLocation.Riichi:
            return Path.Combine(PerspectivePath, $"1{c}{tileNumber}.png");
          case TileLocation.FaceDown:
            return Path.Combine(PerspectivePath, "1j9.png");
          default:
            return Path.Combine(FlatPath, $"{c}{tileNumber}.png");
        }
      }
      return Path.Combine(FlatPath, $"{c}{tileNumber}.png");
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

    public Tile Tile { get; }
    public int X { get; }

    public int Y { get; }

    public ImageSource Source { get; }
  }
}