// Spines.Tenhou.Client.Tile.cs
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

using System;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// A mahjong tile.
  /// </summary>
  public class Tile
  {
    /// <summary>
    /// Instantiates a new Tile.
    /// </summary>
    /// <param name="id">The id of the tile, from 0 to 135.</param>
    public Tile(int id)
    {
      if (id < 0 || id > 135)
      {
        throw new ArgumentOutOfRangeException("id", id, "The id of a tile must be between 0 and 135.");
      }
      Id = id;
    }

    /// <summary>
    /// The Id of the tile, from 0 to 135.
    /// </summary>
    public int Id { get; private set; }
  }
}