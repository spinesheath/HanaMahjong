// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Spines.Hana.Blame.Models
{
  /// <summary>
  /// PK: Id
  /// Unique: Name
  /// </summary>
  public class Player
  {
    public long Id { get; set; }
    public string Name { get; set; }
    public ICollection<Participant> Participants { get; set; }
  }
}