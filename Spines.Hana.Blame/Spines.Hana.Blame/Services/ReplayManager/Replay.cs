// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  [DataContract]
  internal class Replay
  {
    [DataMember(Name = "room")]
    public Room Room { get; private set; }

    [DataMember(Name = "lobby")]
    public string Lobby { get; private set; }

    [DataMember(Name = "rules")]
    public RuleSet Rules { get; private set; }

    [DataMember(Name = "games")]
    public IReadOnlyList<Game> Games { get; private set; }

    public static Replay Parse(string xml)
    {
      try
      {
        return ParseInternal(xml);
      }
      catch
      {
        return null;
      }
    }

    private Replay()
    {
    }

    private static readonly Regex DiscardRegex = new Regex(@"[DEFG](\d{1,3})");
    private static readonly Regex DrawRegex = new Regex(@"[TUVW](\d{1,3})");

    private static readonly Dictionary<MeldType, int> MeldTypeIds = new Dictionary<MeldType, int>
    {
      {MeldType.Koutsu, Ids.Pon},
      {MeldType.Shuntsu, Ids.Chii},
      {MeldType.ClosedKan, Ids.ClosedKan},
      {MeldType.CalledKan, Ids.CalledKan},
      {MeldType.AddedKan, Ids.AddedKan}
    };

    private static Replay ParseInternal(string xml)
    {
      var result = new Replay();
      var xElement = XElement.Parse(xml);
      var games = new List<Game>();
      var gameIndex = 0;

      var shuffle = xElement.Element("SHUFFLE");
      var seed = shuffle?.Attribute("seed")?.Value;
      var generator = new WallGenerator(seed);

      var go = xElement.Element("GO");
      result.Lobby = go?.Attribute("lobby")?.Value;
      var flags = (GameTypeFlag) int.Parse(go?.Attribute("type")?.Value);
      result.Rules = RuleSet.Parse(flags);
      result.Room = GetRoom(flags);

      var upcomingRinshan = false;
      var game = new Game();

      foreach (var e in xElement.Elements())
      {
        var name = e.Name.LocalName;
        if (DrawRegex.IsMatch(name))
        {
          if (upcomingRinshan)
          {
            upcomingRinshan = false;
            game.Actions.Add(Ids.Rinshan);
          }
          else
          {
            game.Actions.Add(Ids.Draw);
          }
          continue;
        }
        var match = DiscardRegex.Match(name);
        if (match.Success)
        {
          game.Actions.Add(ToInt(match.Groups[1].Value));
          continue;
        }
        switch (name)
        {
          case "SHUFFLE":
          case "UN":
          case "TAIKYOKU":
          case "GO":
          case "BYE":
            break;
          case "DORA":
            game.Actions.Add(Ids.Dora);
            break;
          case "INIT":
            game = new Game();
            games.Add(game);
            game.Wall.AddRange(generator.GetWall(gameIndex));
            game.Dice.AddRange(generator.GetDice(gameIndex));
            game.Oya = ToInt(e.Attribute("oya")?.Value);
            gameIndex += 1;
            upcomingRinshan = false;
            break;
          case "AGARI":
            game.Actions.Add(Ids.Agari);
            game.Actions.Add(ToInt(e.Attribute("who")?.Value));
            game.Actions.Add(ToInt(e.Attribute("fromWho")?.Value));
            break;
          case "N":
            var decoder = new MeldDecoder(e.Attribute("m")?.Value);
            if (IsKan(decoder.MeldType))
            {
              upcomingRinshan = true;
            }
            game.Actions.Add(MeldTypeIds[decoder.MeldType]);
            game.Actions.AddRange(decoder.Tiles);
            break;
          case "REACH":
            var step = e.Attribute("step")?.Value;
            if (step == "1")
            {
              game.Actions.Add(Ids.Reach);
            }
            break;
          case "RYUUKYOKU":
            game.Actions.Add(Ids.Ryuukyoku);
            break;
          default:
            throw new NotImplementedException();
        }
      }
      result.Games = games.ToArray();
      return result;
    }

    private static bool IsKan(MeldType meldType)
    {
      return meldType == MeldType.AddedKan || meldType == MeldType.CalledKan || meldType == MeldType.ClosedKan;
    }

    private static int ToInt(string value)
    {
      return Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    private static Room GetRoom(GameTypeFlag flags)
    {
      if (flags.HasFlag(GameTypeFlag.Advanced))
      {
        return flags.HasFlag(GameTypeFlag.Expert) ? Room.Phoenix : Room.LowerDan;
      }
      return flags.HasFlag(GameTypeFlag.Expert) ? Room.UpperDan : Room.General;
    }

    private struct Ids
    {
      public const int Draw = 300;
      public const int Agari = 301;
      public const int Ryuukyoku = 302;
      public const int Reach = 303;
      public const int Dora = 304;
      public const int Rinshan = 305;
      public const int Pon = 400;
      public const int Chii = 401;
      public const int ClosedKan = 402;
      public const int CalledKan = 403;
      public const int AddedKan = 404;
    }
  }
}