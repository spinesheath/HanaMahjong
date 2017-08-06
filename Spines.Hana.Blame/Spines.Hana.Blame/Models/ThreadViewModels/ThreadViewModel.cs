// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace Spines.Hana.Blame.Models.ThreadViewModels
{
  public class ThreadViewModel
  {
    public ThreadViewModel()
    {
      Comments = new List<string>();
    }

    public ThreadViewModel(IEnumerable<string> comments)
    {
      Comments = comments.ToList();
    }

    public List<string> Comments { get; }

    public string Message { get; set; }
  }
}