﻿// This file is licensed to you under the MIT license.
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
      switch (tile.Location)
      {
        case TileLocation.Concealed:
          switch (playerPosition)
          {
            case 0:
              return Path.Combine(PerspectivePath, $"0{c}{tileNumber}.png");
            case 1:
              return Path.Combine(PerspectivePath, "vertr.png");
            case 2:
              return Path.Combine(PerspectivePath, "0j9.png");
            case 3:
              return Path.Combine(PerspectivePath, "vertl.png");
          }
          break;
        case TileLocation.Discarded:
        case TileLocation.Melded:
          return Path.Combine(PerspectivePath, $"{1 + playerPosition}{c}{tileNumber}.png");
        case TileLocation.Added:
        case TileLocation.Called:
        case TileLocation.Riichi:
          return Path.Combine(PerspectivePath, $"{1 + (playerPosition + 1) % 4}{c}{tileNumber}.png");
        case TileLocation.FaceDown:
          return Path.Combine(PerspectivePath, $"{1 + playerPosition}j9.png");
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