// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Hana.Blame.Models
{
  /// <summary>
  /// PK: Id
  /// Unique: Match + Index
  /// </summary>
  public class Game
  {
    public long Id { get; set; }
    public Match Match { get; set; }
    public int Index { get; set; }
    public int FrameCount { get; set; }
  }
}