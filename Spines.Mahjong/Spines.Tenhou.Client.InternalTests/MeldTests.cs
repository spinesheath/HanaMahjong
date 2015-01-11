// Spines.Tenhou.Client.InternalTests.MeldTests.cs
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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spines.Tenhou.Client.InternalTests
{
  [TestClass]
  public class MeldTests
  {
    [TestMethod]
    public void TestFromMeldCode()
    {
      var m = new Meld("42570", 0);
      Assert.AreEqual(0, m.OwnerId, "Wrong owner of meld.");
      Assert.AreEqual(2, m.FromPlayerId, "Wrong player that the tile was called from.");
      Assert.AreEqual(MeldType.Koutsu, m.Type, "Wrong meld type.");
      Assert.IsTrue(m.Tiles.Any(t => t.Tile.Id == 111), "Missing tile 111.");
      Assert.IsTrue(m.Tiles.Any(t => t.Tile.Id == 109), "Missing tile 109.");
      Assert.IsTrue(m.Tiles.Any(t => t.Tile.Id == 108), "Missing tile 108.");
      Assert.AreEqual(MeldTileType.Flipped, m.Tiles.First(t => t.Tile.Id == 111).Type, "Wrong type of tile 111.");
      Assert.AreEqual(MeldTileType.Normal, m.Tiles.First(t => t.Tile.Id == 109).Type, "Wrong type of tile 109");
      Assert.AreEqual(MeldTileType.Normal, m.Tiles.First(t => t.Tile.Id == 108).Type, "Wrong type of tile 108");
    }
  }
}