// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Hana.Blame.Models
{
  public class FrameComment : Comment
  {
    public int MatchId { get; set; }
    public Match Match { get; set; }
    public int GameIndex { get; set; }
    public int FrameIndex { get; set; }
    public int SeatIndex { get; set; }
  }
}