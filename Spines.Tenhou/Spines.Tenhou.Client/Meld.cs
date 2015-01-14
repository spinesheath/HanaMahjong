// Spines.Tenhou.Client.Meld.cs
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
using System.Globalization;

namespace Spines.Tenhou.Client
{
  internal class Meld
  {
    /// <summary>
    /// Creates a meld from a meld code.
    /// </summary>
    /// <param name="meldCodeString">The meld code that specifies the meld.</param>
    /// <param name="ownerId">The owner of the meld.</param>
    public Meld(string meldCodeString, int ownerId)
    {
      var meldCode = new MeldCode(Convert.ToInt32(meldCodeString, CultureInfo.InvariantCulture), ownerId);
      Type = meldCode.Type;
      Tiles = meldCode.Tiles;
      FromPlayerId = meldCode.FromPlayerId;
      OwnerId = meldCode.OwnerId;
    }

    /// <summary>
    /// The id of the owner of the meld.
    /// </summary>
    public int OwnerId { get; private set; }

    /// <summary>
    /// The id of the player that discarded the called tile, or the owner in case of a closed kan. Not sure about added kan.
    /// </summary>
    public int FromPlayerId { get; private set; }

    /// <summary>
    /// The tiles in the meld.
    /// </summary>
    public MeldTile[] Tiles { get; private set; }

    /// <summary>
    /// The type of the meld.
    /// </summary>
    public MeldType Type { get; private set; }
  }
}