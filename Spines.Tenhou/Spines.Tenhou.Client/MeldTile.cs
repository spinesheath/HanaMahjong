// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Tenhou.Client
{
  internal class MeldTile
  {
    public MeldTile(Tile tile, MeldTileType type)
    {
      Tile = tile;
      Type = type;
    }

    public Tile Tile { get; private set; }
    public MeldTileType Type { get; private set; }
  }
}