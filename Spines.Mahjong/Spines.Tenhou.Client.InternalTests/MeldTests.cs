// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
      Assert.AreEqual(MeldTileType.Called, m.Tiles.First(t => t.Tile.Id == 111).Type, "Wrong type of tile 111.");
      Assert.AreEqual(MeldTileType.Normal, m.Tiles.First(t => t.Tile.Id == 109).Type, "Wrong type of tile 109");
      Assert.AreEqual(MeldTileType.Normal, m.Tiles.First(t => t.Tile.Id == 108).Type, "Wrong type of tile 108");
    }
  }
}