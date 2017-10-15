// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  [DataContract]
  internal class RuleSet
  {
    /// <summary>
    /// Are there aka dora?
    /// </summary>
    [DataMember(Name = "aka")]
    public bool Aka { get; }

    /// <summary>
    /// Is open tanyao allowed?
    /// </summary>
    [DataMember(Name = "kuitan")]
    public bool Kuitan { get; }

    /// <summary>
    /// How many players play the match.
    /// </summary>
    [DataMember(Name = "playerCount")]
    public int PlayerCount { get; }

    /// <summary>
    /// How many seconds the players get per action.
    /// </summary>
    [DataMember(Name = "secondsPerAction")]
    public decimal SecondsPerAction { get; }

    /// <summary>
    /// How many extra seconds players get over the course of a game.
    /// </summary>
    [DataMember(Name = "extraSecondsPerGame")]
    public decimal ExtraSecondsPerGame { get; }

    /// <summary>
    /// How often the dealer position goes around the table in a regular game.
    /// </summary>
    [DataMember(Name = "rounds")]
    public int Rounds { get; set; }

    public static RuleSet Parse(GameTypeFlag flags)
    {
      return new RuleSet(flags);
    }

    private RuleSet(GameTypeFlag flags)
    {
      Aka = !flags.HasFlag(GameTypeFlag.AkaNashi);
      Kuitan = !flags.HasFlag(GameTypeFlag.KuitanNashi);
      Rounds = flags.HasFlag(GameTypeFlag.Tonnansen) ? 2 : 1;
      if (flags.HasFlag(GameTypeFlag.Fast))
      {
        SecondsPerAction = 2.5M;
        ExtraSecondsPerGame = 5M;
      }
      else
      {
        SecondsPerAction = 5M;
        ExtraSecondsPerGame = 5M;
      }
      PlayerCount = flags.HasFlag(GameTypeFlag.Sanma) ? 3 : 4;
    }
  }
}