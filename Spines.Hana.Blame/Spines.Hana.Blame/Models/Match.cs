// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Spines.Hana.Blame.Models
{
  /// <summary>
  /// PK: Id
  /// Unique: FileName + ContainerName
  /// </summary>
  public class Match
  {
    public Match()
    {
    }

    public Match(IEnumerable<Game> games, IEnumerable<Participant> participants)
    {
      Games = games.ToList();
      Participants = participants.ToList();
    }

    public int Id { get; set; }

    /// <summary>
    /// The ID of the replay.
    /// </summary>
    public string FileName { get; set; }
    public string ContainerName { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime UploadTime { get; set; }
    public ICollection<Participant> Participants { get; } = new List<Participant>();
    public ICollection<Game> Games { get; } = new List<Game>();
    public int RuleSetId { get; set; }
    public RuleSet RuleSet { get; set; }
    public int RoomId { get; set; }
    public Room Room { get; set; }
    public string Lobby { get; set; }
  }
}