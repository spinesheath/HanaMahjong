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