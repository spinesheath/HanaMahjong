// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
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

    [DataMember(Name = "players")]
    public IReadOnlyList<Player> Players { get; private set; }

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

    private static readonly Regex DiscardRegex = new Regex(@"([DEFG])(\d{1,3})");
    private static readonly Regex DrawRegex = new Regex(@"([TUVW])(\d{1,3})");

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

      var un = xElement.Element("UN");
      var names = GetUserNames(un).ToList();
      var ranks = un?.Attribute("dan")?.Value.Split(',').Select(ToInt).ToList();
      var rates = un?.Attribute("rate")?.Value.Split(',').Select(ToDecimal).ToList();
      var genders = un?.Attribute("sx")?.Value.Split(',').ToList();
      var players = new List<Player>();
      for (var i = 0; i < names.Count; ++i)
      {
        players.Add(new Player(names[i], ranks[i], rates[i], genders[i]));
      }
      result.Players = players;

      var upcomingRinshan = false;
      var game = new Game();
      var hands = new List<List<int>>();

      foreach (var e in xElement.Elements())
      {
        var name = e.Name.LocalName;
        var drawMatch = DrawRegex.Match(name);
        if (drawMatch.Success)
        {
          var tileId = ToInt(drawMatch.Groups[2].Value);
          var playerId = drawMatch.Groups[1].Value[0] - 'T';
          hands[playerId].Add(tileId);
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
        var discardMatch = DiscardRegex.Match(name);
        if (discardMatch.Success)
        {
          var playerId = discardMatch.Groups[1].Value[0] - 'D';
          var tileId = ToInt(discardMatch.Groups[2].Value);
          if (hands[playerId].Last() == tileId)
          {
            game.Actions.Add(Ids.Tsumogiri);
          }
          else
          {
            var index = hands[playerId].OrderBy(x => x).ToList().IndexOf(tileId);
            game.Actions.Add(Ids.DiscardOffset + index);
          }
          hands[playerId].Remove(tileId);
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
            hands = GetStartingHands(e).ToList();
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

            var who = ToInt(e.Attribute("who")?.Value);
            hands[who].RemoveAll(t => decoder.Tiles.Contains(t));
            break;
          case "REACH":
            var step = e.Attribute("step")?.Value;
            if (step == "1")
            {
              game.Actions.Add(Ids.Riichi);
            }
            else
            {
              game.Actions.Add(Ids.RiichiPayment);
            }
            break;
          case "RYUUKYOKU":
            game.Actions.Add(Ids.Ryuukyoku);
            break;
          default:
            throw new NotImplementedException();
        }
      }
      result.Games = games;
      return result;
    }

    private static IEnumerable<List<int>> GetStartingHands(XElement element)
    {
      return Enumerable.Range(0, 4).Select(i => element.Attribute($"hai{i}")?.Value.Split(',').Select(ToInt)).Select(t => t.ToList());
    }

    private static IEnumerable<string> GetUserNames(XElement element)
    {
      return Enumerable.Range(0, 4).Select(i => DecodeName(element.Attribute($"n{i}")?.Value));
    }

    private static string DecodeName(string encodedName)
    {
      if (encodedName.Length == 0)
      {
        return encodedName;
      }
      if (encodedName[0] != '%')
      {
        return encodedName;
      }
      var hexValues = encodedName.Split('%', StringSplitOptions.RemoveEmptyEntries);
      var bytes = hexValues.Select(v => Convert.ToByte(v, 16)).ToArray();
      return new UTF8Encoding().GetString(bytes);
    }

    private static bool IsKan(MeldType meldType)
    {
      return meldType == MeldType.AddedKan || meldType == MeldType.CalledKan || meldType == MeldType.ClosedKan;
    }

    private static int ToInt(string value)
    {
      return Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    private static decimal ToDecimal(string value)
    {
      return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
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
      public const int Draw = 0;
      public const int Tsumogiri = 1;
      public const int DiscardOffset = 2;
      public const int Agari = 50;
      public const int Ryuukyoku = 51;
      public const int Riichi = 52;
      public const int RiichiPayment = 60;
      public const int Dora = 53;
      public const int Rinshan = 54;
      public const int Pon = 55;
      public const int Chii = 56;
      public const int ClosedKan = 57;
      public const int CalledKan = 58;
      public const int AddedKan = 59;
    }
  }
}