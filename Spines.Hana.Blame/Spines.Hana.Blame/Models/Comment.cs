// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Spines.Hana.Blame.Models
{
  public class Comment
  {
    public long Id { get; set; }
    public ApplicationUser User { get; set; }
    public DateTime Time { get; set; }
    public string Message { get; set; }
    public string UserId { get; set; }
  }
}