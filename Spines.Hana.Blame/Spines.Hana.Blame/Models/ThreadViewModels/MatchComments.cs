// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Spines.Hana.Blame.Models.ThreadViewModels
{
  public class MatchComments
  {
    public string ReplayId { get; set; }
    public List<FrameComment> Comments { get; set; }
  }
}