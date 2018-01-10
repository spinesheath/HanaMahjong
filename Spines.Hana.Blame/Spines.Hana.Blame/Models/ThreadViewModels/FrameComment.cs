// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Spines.Hana.Blame.Models.ThreadViewModels
{
  /// <summary>
  /// A comment for a single frame.
  /// </summary>
  public class FrameComment
  {
    public int GameId { get; set; }
    public int FrameId { get; set; }
    public int PlayerId { get; set; }
    public string Message { get; set; }
    public string UserName { get; set; }
    public DateTime Timestamp { get; set; }
    public bool Editable { get; set; }
    public long Id { get; set; }
  }
}