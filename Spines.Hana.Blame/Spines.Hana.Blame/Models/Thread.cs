// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Spines.Hana.Blame.Models
{
  public class Thread
  {
    public long Id { get; set; }
    public ICollection<Comment> Comments { get; set; }
  }
}