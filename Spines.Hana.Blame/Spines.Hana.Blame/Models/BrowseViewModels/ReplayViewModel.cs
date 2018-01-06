// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Spines.Hana.Blame.Models.BrowseViewModels
{
  public class ReplayViewModel
  {
    public string Id { get; set; }
    public IEnumerable<string> Participants { get; set; }
    public DateTime Timestamp { get; set; }
  }
}