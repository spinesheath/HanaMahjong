// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Spines.Hana.Clay.ViewModels;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Hana.Clay.Models
{
  internal class LayoutGenerator
  {
    public IEnumerable<TileViewModel> Create(IReadOnlyList<PlayerViewModel> players)
    {
      _tiles.Clear();
      AddPlayer1Tiles(players[0]);
      AddPlayer2Tiles(players[1]);
      AddPlayer3Tiles(players[2]);
      AddPlayer4Tiles(players[3]);
      return _tiles.OrderBy(t => t.Y);
    }

    private readonly List<TileViewModel> _tiles = new List<TileViewModel>();

    private void AddPlayer4Tiles(PlayerViewModel p)
    {
      const int playerId = 3;
      var kanShift = GetKanShift(p);
      var y = TableLayout.HalfTableWidth - 3 * TableLayout.TileWidth - 3 * TableLayout.TileHeight - TableLayout.VerticalTileThickness - TableLayout.HorizontalPondToHand;
      var x = TableLayout.HalfTableHeight - 7 * TableLayout.TileWidth - TableLayout.VerticalTileThickness - kanShift;
      AddConcealedTiles(p, x, y, playerId);

      y = TableLayout.HalfTableWidth - 3 * TableLayout.TileWidth - 4 * TableLayout.TileHeight - TableLayout.HorizontalPondToHand;
      x = TableLayout.HalfTableHeight + 3 * TableLayout.TileWidth + 3 * TableLayout.TileHeight + TableLayout.TileHeight + TableLayout.HorizontalTileThickness + TableLayout.VerticalPondToHand;
      foreach (var meld in p.Melds.Reverse())
      {
        foreach (var tile in meld.Tiles.Reverse())
        {
          if (tile.Location == TileLocation.Added)
          {
            _tiles.Add(CreateTileWithXySwitch(tile, x - TableLayout.TileHeight, y + TableLayout.TileWidth, playerId));
          }
          else
          {
            x -= tile.Location == TileLocation.Called ? TableLayout.TileHeight : TableLayout.TileWidth;
            _tiles.Add(CreateTileWithXySwitch(tile, x, y, playerId));
          }
        }
        x -= TableLayout.MeldDistance;
      }

      y = TableLayout.HalfTableWidth - 3 * TableLayout.TileWidth - TableLayout.TileHeight;
      x = TableLayout.HalfTableHeight - 3 * TableLayout.TileWidth;
      AddPond(p, x, y, playerId);
    }

    private void AddPlayer3Tiles(PlayerViewModel p)
    {
      const int playerId = 2;
      var kanShift = GetKanShift(p);
      var x = TableLayout.HalfTableWidth + 6 * TableLayout.TileWidth + kanShift;
      var y = TableLayout.HalfTableHeight - 3 * TableLayout.TileWidth - 4 * TableLayout.TileHeight - TableLayout.VerticalPondToHand - TableLayout.HorizontalTileThickness;
      AddConcealedTiles(p, x, y, playerId);

      x = TableLayout.HalfTableWidth - (3 * TableLayout.TileWidth + 3 * TableLayout.TileHeight + TableLayout.HorizontalPondToHand + TableLayout.TileHeight);
      foreach (var meld in p.Melds.Reverse())
      {
        foreach (var tile in meld.Tiles.Reverse())
        {
          if (tile.Location == TileLocation.Added)
          {
            x += TableLayout.TileHeight;
            _tiles.Add(CreateTileWithXySwitch(tile, x, y - TableLayout.TileHeight - TableLayout.TileWidth - TableLayout.TileWidth, playerId));
            x -= TableLayout.TileHeight;
          }
          else
          {
            _tiles.Add(CreateTileWithXySwitch(tile, x, y, playerId));
            x += tile.Location == TileLocation.Called ? TableLayout.TileHeight : TableLayout.TileWidth;
          }
        }
        x += TableLayout.MeldDistance;
      }

      x = TableLayout.HalfTableWidth + 2 * TableLayout.TileWidth;
      y = TableLayout.HalfTableHeight - 3 * TableLayout.TileWidth - TableLayout.TileHeight;
      AddPond(p, x, y, playerId);
    }

    private void AddPlayer2Tiles(PlayerViewModel p)
    {
      const int playerId = 1;
      var kanShift = GetKanShift(p);
      var y = TableLayout.HalfTableWidth + 3 * TableLayout.TileWidth + 3 * TableLayout.TileHeight + TableLayout.HorizontalPondToHand;
      var x = TableLayout.HalfTableHeight + 6 * TableLayout.TileWidth + kanShift;
      AddConcealedTiles(p, x, y, playerId);

      x = TableLayout.HalfTableHeight - (3 * TableLayout.TileWidth + 3 * TableLayout.TileHeight + TableLayout.TileHeight + TableLayout.HorizontalTileThickness + TableLayout.VerticalPondToHand);
      foreach (var meld in p.Melds.Reverse())
      {
        foreach (var tile in meld.Tiles.Reverse())
        {
          if (tile.Location == TileLocation.Added)
          {
            _tiles.Add(CreateTileWithXySwitch(tile, x, y + TableLayout.TileHeight - TableLayout.TileWidth - TableLayout.TileWidth, playerId));
          }
          else
          {
            var h = tile.Location == TileLocation.Called ? TableLayout.TileHeight - TableLayout.TileWidth : 0;
            _tiles.Add(CreateTileWithXySwitch(tile, x, y + h, playerId));
            x += tile.Location == TileLocation.Called ? TableLayout.TileHeight : TableLayout.TileWidth;
          }
        }
        x += TableLayout.MeldDistance;
      }

      y = TableLayout.HalfTableWidth + 3 * TableLayout.TileWidth;
      x = TableLayout.HalfTableHeight + 2 * TableLayout.TileWidth;
      AddPond(p, x, y, playerId);
    }

    private void AddPlayer1Tiles(PlayerViewModel p)
    {
      const int playerId = 0;
      var kanShift = GetKanShift(p);
      var x = TableLayout.HalfTableWidth - 7 * TableLayout.TileWidth - kanShift;
      var y = TableLayout.HalfTableHeight + 3 * TableLayout.TileWidth + 3 * TableLayout.TileHeight + TableLayout.HorizontalTileThickness + TableLayout.VerticalPondToHand;
      AddConcealedTiles(p, x, y, playerId);

      x = TableLayout.HalfTableWidth + 3 * TableLayout.TileWidth + 3 * TableLayout.TileHeight + TableLayout.HorizontalPondToHand + TableLayout.TileHeight;
      foreach (var meld in p.Melds.Reverse())
      {
        foreach (var tile in meld.Tiles.Reverse())
        {
          if (tile.Location == TileLocation.Added)
          {
            x -= TableLayout.TileHeight;
            _tiles.Add(CreateTileWithXySwitch(tile, x, y + TableLayout.TileHeight - TableLayout.TileWidth - TableLayout.TileWidth, playerId));
            x += TableLayout.TileHeight;
          }
          else
          {
            var h = tile.Location == TileLocation.Called ? TableLayout.TileHeight - TableLayout.TileWidth : 0;
            x -= tile.Location == TileLocation.Called ? TableLayout.TileHeight : TableLayout.TileWidth;
            _tiles.Add(CreateTileWithXySwitch(tile, x, y + h, playerId));
          }
        }
        x -= TableLayout.MeldDistance;
      }

      x = TableLayout.HalfTableWidth - 3 * TableLayout.TileWidth;
      y = TableLayout.HalfTableHeight + 3 * TableLayout.TileWidth;
      AddPond(p, x, y, playerId);
    }

    private void AddPond(PlayerViewModel p, int x, int y, int playerId)
    {
      var factorX = playerId == 0 || playerId == 3 ? 1 : -1;
      var factorY = playerId < 2 ? 1 : -1;
      var riichiX = playerId == 0 || playerId == 3 ? 0 : TableLayout.TileHeight - TableLayout.TileWidth;
      var riichiY = playerId < 2 ? TableLayout.RiichiDistance : TableLayout.TileWidth - TableLayout.TileHeight + TableLayout.RiichiDistance;

      var startX = x;
      var pondColumn = 0;
      var pondRow = 0;
      foreach (var tile in p.Pond)
      {
        if (tile.Location == TileLocation.Riichi)
        {
          _tiles.Add(CreateTileWithXySwitch(tile, x - riichiX, y + riichiY, playerId));
          x += TableLayout.TileHeight * factorX;
        }
        else
        {
          _tiles.Add(CreateTileWithXySwitch(tile, x, y, playerId));
          x += TableLayout.TileWidth * factorX;
        }
        pondColumn += 1;
        if (pondColumn == 6 && pondRow != 2)
        {
          pondRow += 1;
          pondColumn = 0;
          x = startX;
          y += TableLayout.TileHeight * factorY;
        }
      }
    }

    private void AddConcealedTiles(PlayerViewModel p, int x, int y, int playerId)
    {
      var factor = playerId == 0 || playerId == 3 ? 1 : -1;
      foreach (var tile in p.Tiles)
      {
        _tiles.Add(CreateTileWithXySwitch(tile, x, y, playerId));
        x += TableLayout.TileWidth * factor;
      }
      if (p.Draw.HasValue)
      {
        x += TableLayout.DrawDistance * factor;
        _tiles.Add(CreateTileWithXySwitch(p.Draw.Value, x, y, playerId));
      }
    }

    private static TileViewModel CreateTileWithXySwitch(Tile tile, int x, int y, int playerId)
    {
      return playerId % 2 == 0 ? new TileViewModel(tile, x, y, playerId) : new TileViewModel(tile, y, x, playerId);
    }

    private static int GetKanShift(PlayerViewModel player)
    {
      var wideKanCount = player.Melds.Count(m => m.Tiles.Count() == 4 && m.Tiles.All(t => t.Location != TileLocation.Added));
      return wideKanCount < 4 ? 0 : TableLayout.TileWidth;
    }
  }
}