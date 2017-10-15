// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  [DataContract]
  internal class RuleSet
  {
    public static RuleSet Parse(GameTypeFlag flags)
    {
      return new RuleSet(flags);
    }

    private RuleSet(GameTypeFlag flags)
    {
      _akaNashi = flags.HasFlag(GameTypeFlag.AkaNashi);
      _kuitanNashi = flags.HasFlag(GameTypeFlag.KuitanNashi);
      _rounds = flags.HasFlag(GameTypeFlag.Tonnansen) ? 2 : 1;
      _speed = flags.HasFlag(GameTypeFlag.Fast) ? Speed.Fast : Speed.Normal;
      _sanma = flags.HasFlag(GameTypeFlag.Sanma);
    }

    private bool _akaNashi;
    private bool _kuitanNashi;
    private int _rounds;
    private Speed _speed;
    private bool _sanma;
  }
}