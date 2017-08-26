// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  internal class Replay
  {
    public Room Room { get; private set; }
    public string Lobby { get; private set; }
    public RuleSet Rules { get; private set; }

    /// <summary>
    /// Per hand: 136 tiles, 2 dice, oya, then actions.
    /// </summary>
    public IReadOnlyList<int> Data { get; private set; }

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
      var data = new List<int>();
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

      foreach (var e in xElement.Elements())
      {
        var name = e.Name.LocalName;
        if (DrawRegex.IsMatch(name))
        {
          if (upcomingRinshan)
          {
            upcomingRinshan = false;
            data.Add(Ids.Rinshan);
          }
          continue;
        }
        var match = DiscardRegex.Match(name);
        if (match.Success)
        {
          data.Add(ToInt(match.Groups[1].Value));
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
            data.Add(Ids.Dora);
            break;
          case "INIT":
            data.Add(Ids.Init);
            data.AddRange(generator.GetWall(gameIndex));
            data.AddRange(generator.GetDice(gameIndex));
            data.Add(ToInt(e.Attribute("oya")?.Value));
            gameIndex += 1;
            upcomingRinshan = false;
            break;
          case "AGARI":
            data.Add(Ids.Agari);
            data.Add(ToInt(e.Attribute("who")?.Value));
            data.Add(ToInt(e.Attribute("fromWho")?.Value));
            break;
          case "N":
            var decoder = new MeldDecoder(e.Attribute("m")?.Value);
            if (IsKan(decoder.MeldType))
            {
              upcomingRinshan = true;
            }
            data.Add(MeldTypeIds[decoder.MeldType]);
            data.AddRange(decoder.Tiles);
            break;
          case "REACH":
            var step = e.Attribute("step")?.Value;
            if (step == "1")
            {
              data.Add(Ids.Reach);
            }
            break;
          case "RYUUKYOKU":
            data.Add(Ids.Ryuukyoku);
            break;
          default:
            throw new NotImplementedException();
        }
      }
      result.Data = data.ToArray();
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
      public const int Init = 300;
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