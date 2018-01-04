// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Hana.Blame.Models.ThreadViewModels
{
  /// <summary>
  /// Data used to create a new comment.
  /// </summary>
  public class CreateFrameComment
  {
    public int GameId { get; set; }
    public int FrameId { get; set; }
    public int PlayerId { get; set; }
    public string Message { get; set; }
    public string ReplayId { get; set; }
  }
}