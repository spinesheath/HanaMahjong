// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Spines.Hana.Blame.Models.ThreadViewModels
{
  public class CommentViewModel
  {
    public string Message { get; set; }
    public string UserName { get; set; }
    public DateTime Time { get; set; }
  }
}