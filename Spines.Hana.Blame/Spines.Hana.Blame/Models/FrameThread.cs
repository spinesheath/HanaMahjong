// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Hana.Blame.Models
{
  public class FrameThread : Thread
  {
    public int MatchId { get; set; }
    public Match Match { get; set; }
    public int GameId { get; set; }
    public Game Game { get; set; }
    public int FrameId { get; set; }
    public int ParticipantId { get; set; }
    public Participant Participant { get; set; }
  }
}