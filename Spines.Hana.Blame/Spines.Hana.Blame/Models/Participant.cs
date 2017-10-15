// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Hana.Blame.Models
{
  /// <summary>
  /// Unique: Player + Match
  /// Unique: Match + Seat
  /// </summary>
  public class Participant
  {
    public int Id { get; set; }
    public int Score { get; set; }
    public decimal Points { get; set; }
    public int PlayerId { get; set; }
    public Player Player { get; set; }
    public int MatchId { get; set; }
    public Match Match { get; set; }
    public int Seat { get; set; }
    public int Placement { get; set; }
    public int Rank { get; set; }
    public decimal Rate { get; set; }
    public string Gender { get; set; }
  }
}