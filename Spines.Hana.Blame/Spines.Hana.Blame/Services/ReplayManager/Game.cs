// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  [DataContract]
  internal class Game
  {
    /// <summary>
    /// 0-3 for E1-E4, 4-7 for S1-S4 etc.
    /// </summary>
    [DataMember(Name = "round")]
    public int Round { get; set; }

    /// <summary>
    /// The index of the repetition of the round. 0 for E1-1, 1 for E1-2 etc.
    /// </summary>
    [DataMember(Name = "repetition")]
    public int Repetition { get; set; }

    /// <summary>
    /// Dealer for the game.
    /// </summary>
    [DataMember(Name = "oya")]
    public int Oya { get; set; }

    /// <summary>
    /// Number of honba on the table at the start of the game.
    /// </summary>
    [DataMember(Name = "honba")]
    public int Honba { get; set; }

    /// <summary>
    /// Number of riichi sticks on the table at the start of the game.
    /// </summary>
    [DataMember(Name = "riichi")]
    public int Riichi { get; set; }

    /// <summary>
    /// Scores for players in seat order at the start of the game.
    /// </summary>
    [DataMember(Name = "scores")]
    public List<int> Scores { get; } = new List<int>();

    /// <summary>
    /// Dice rolled for the game. Two values in the range [1,6].
    /// </summary>
    [DataMember(Name = "dice")]
    public List<int> Dice { get; } = new List<int>();

    /// <summary>
    /// The tiles in the wall at the start of the the game, before any tiles have been drawn.
    /// </summary>
    [DataMember(Name = "wall")]
    public List<int> Wall { get; } = new List<int>();

    /// <summary>
    /// Encoded list of actions.
    /// </summary>
    [DataMember(Name = "actions")]
    public List<int> Actions { get; } = new List<int>();

    /// <summary>
    /// List of discards.
    /// </summary>
    [DataMember(Name = "discards")]
    public List<int> Discards { get; } = new List<int>();

    /// <summary>
    /// List of agaris.
    /// </summary>
    [DataMember(Name = "agaris")]
    public List<Agari> Agaris { get; } = new List<Agari>();

    /// <summary>
    /// Details if the game ended in a draw.
    /// </summary>
    [DataMember(Name = "ryuukyoku")]
    public Ryuukyoku Ryuukyoku { get; set; }

    /// <summary>
    /// List of calls.
    /// </summary>
    [DataMember(Name = "calls")]
    public List<Call> Calls { get; } = new List<Call>();
  }
}