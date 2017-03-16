// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Spines.Utility;

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
      Id = Validate.InRange(id, 0, 135, "id");
    }

    /// <summary>
    /// The Id of the tile, from 0 to 135.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// True if both tiles have the same Id.
    /// </summary>
    public static bool operator ==(Tile leftHandSide, Tile rightHandSide)
    {
      return Validate.NotNull(leftHandSide, "leftHandSide").Equals(Validate.NotNull(rightHandSide, "rightHandSide"));
    }

    /// <summary>
    /// False if both tiles have the same Id.
    /// </summary>
    public static bool operator !=(Tile leftHandSide, Tile rightHandSide)
    {
      return !(leftHandSide == rightHandSide);
    }

    /// <summary>
    /// True if both tiles have the same Id.
    /// </summary>
    public override bool Equals(object obj)
    {
      if (obj == null || GetType() != obj.GetType())
      {
        return false;
      }
      return ((Tile) obj).Id == Id;
    }

    /// <summary>
    /// Calculates the hash code of the tile, which is the hash code of the tile id.
    /// </summary>
    public override int GetHashCode()
    {
      return Id.GetHashCode();
    }
  }
}