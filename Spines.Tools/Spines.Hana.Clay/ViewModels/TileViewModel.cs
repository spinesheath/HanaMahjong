// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Spines.Mahjong.Analysis.Classification;

namespace Spines.Hana.Clay.ViewModels
{
  internal class TileViewModel : ViewModelBase
  {
    public TileViewModel(Tile tile, int x, int y)
    {
      Tile = tile;
      X = x;
      Y = y;
    }

    public Tile Tile { get; }

    public int X { get; }

    public int Y { get; }
  }
}