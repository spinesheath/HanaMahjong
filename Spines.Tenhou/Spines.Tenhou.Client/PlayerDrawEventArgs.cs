// Spines.Tenhou.Client.PlayerDrawEventArgs.cs
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
using System.Xml.Linq;
using Spines.Utility;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Data provided by the server when the active player draws a tile.
  /// </summary>
  public class PlayerDrawEventArgs : EventArgs
  {
    internal PlayerDrawEventArgs(XElement message)
    {
      Tile = new Tile(InvariantConvert.ToInt32(message.Name.LocalName.Substring(1)));
    }

    /// <summary>
    /// The drawn tile.
    /// </summary>
    public Tile Tile { get; private set; }
  }
}