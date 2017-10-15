// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Hana.Blame.Models
{
  public class RuleSet
  {
    /// <summary>
    /// Id of the RuleSet.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the RuleSet.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Are there aka dora?
    /// </summary>
    public bool Aka { get; set; }

    /// <summary>
    /// Is open tanyao allowed?
    /// </summary>
    public bool Kuitan { get; set; }

    /// <summary>
    /// How many players play the match.
    /// </summary>
    public int PlayerCount { get; set; }

    /// <summary>
    /// How many seconds the players get per action.
    /// </summary>
    public decimal SecondsPerAction { get; set; }

    /// <summary>
    /// How many extra seconds players get over the course of a game.
    /// </summary>
    public decimal ExtraSecondsPerGame { get; set; }

    /// <summary>
    /// How often the dealer position goes around the table in a regular game.
    /// </summary>
    public int Rounds { get; set; }
  }
}