// Spines.Tenhou.Client.Wall.cs
// 
// Copyright (C) 2015  Johannes Heckl
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Linq;

namespace Spines.Tenhou.Client.LocalServer
{
  internal class Wall
  {
    private readonly IList<Tile> _doras;
    private readonly LinkedList<Tile> _draws;
    private readonly Stack<Tile> _rinshanDraws;

    private readonly IList<IList<Tile>> _startingHands = new List<IList<Tile>>
    {
      new List<Tile>(),
      new List<Tile>(),
      new List<Tile>(),
      new List<Tile>()
    };

    private readonly IList<Tile> _uraDoras;

    public Wall(IEnumerable<Tile> tiles)
    {
      var tileList = tiles.ToList();
      _draws = new LinkedList<Tile>(tileList.Skip(14));
      _rinshanDraws = new Stack<Tile>(new[] {tileList[2], tileList[3], tileList[0], tileList[1]});
      _doras = new List<Tile>(new[] {tileList[5], tileList[7], tileList[9], tileList[11]});
      _uraDoras = new List<Tile>(new[] {tileList[4], tileList[6], tileList[8], tileList[10]});
      for (var i = 0; i < 12 * 4; ++i)
      {
        var drawOrderIndex = (i / 4) % 4;
        _startingHands[drawOrderIndex].Add(PopDraw());
      }
      for (var i = 0; i < 4; ++i)
      {
        _startingHands[i].Add(PopDraw());
      }
    }

    public bool HasNextDraw
    {
      get { return _draws.Count > 0; }
    }

    public Tile GetDora(int index)
    {
      return _doras[index];
    }

    public IEnumerable<Tile> GetStartingHand(int oyaIndex, int playerIndex)
    {
      var drawOrderIndex = (4 + playerIndex - oyaIndex) % 4;
      return _startingHands[drawOrderIndex];
    }

    public Tile GetUraDora(int index)
    {
      return _uraDoras[index];
    }

    public Tile PopDraw()
    {
      var tile = _draws.Last.Value;
      _draws.RemoveLast();
      return tile;
    }

    public Tile PopRinshanDraw()
    {
      _draws.RemoveFirst();
      return _rinshanDraws.Pop();
    }
  }
}