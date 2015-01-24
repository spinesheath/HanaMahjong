// Spines.Tenhou.Client.DiscardInformation.cs
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

using Spines.Utility;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Data provided by the server when a player discards a tile.
  /// </summary>
  public class DiscardInformation
  {
    internal DiscardInformation(XElement message)
    {
      Callable = message.Attributes("t").Any();
      var name = message.Name.LocalName;
      Tsumokiri = "defg".Contains(name[0]);
      PlayerIndex = name[0] - (Tsumokiri ? 'd' : 'D');
      Tile = new Tile(InvariantConvert.ToInt32(name.Substring(1)));
    }

    /// <summary>
    /// 0, 1, 2, 3 for T, U, V, W.
    /// </summary>
    public int PlayerIndex { get; private set; }

    /// <summary>
    /// Id of the discarded tile.
    /// </summary>
    public Tile Tile { get; private set; }

    /// <summary>
    /// True if the discarded tile is the last tile drawn.
    /// </summary>
    public bool Tsumokiri { get; private set; }

    /// <summary>
    /// True if the discarded tile can be called.
    /// </summary>
    public bool Callable { get; private set; }
  }
}