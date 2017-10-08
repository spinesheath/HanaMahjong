// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Spines.Hana.Blame.Models
{
  /// <summary>
  /// PK: Id
  /// Unique: FileName + ContainerName
  /// </summary>
  public class Match
  {
    public int Id { get; set; }
    public string FileName { get; set; }
    public string ContainerName { get; set; }
    public DateTime UploadTime { get; set; }
    public ICollection<Participant> Participants { get; set; }
    public ICollection<Game> Games { get; set; }
  }
}