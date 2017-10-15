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
using Spines.Hana.Blame.Utility;

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

    [DataMember(Name = "owari")]
    public Owari Owari { get; } = new Owari();

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
    private static readonly Dictionary<string, int> RyuukyokuTypeIds = new Dictionary<string, int>
    {
      {RyuukyokuTypes.FourKan, Ids.FourKan},
      {RyuukyokuTypes.FourRiichi, Ids.FourRiichi},
      {RyuukyokuTypes.FourWind, Ids.FourWind},
      {RyuukyokuTypes.NagashiMangan, Ids.NagashiMangan},
      {RyuukyokuTypes.NineYaochuuHai, Ids.NineYaochuuHai},
      {RyuukyokuTypes.ThreeRon, Ids.ThreeRon}
    };

    private static Replay ParseInternal(string xml)
    {
      var result = new Replay();
      var xElement = XElement.Parse(xml);
      var games = new List<Game>();

      var shuffle = xElement.Element("SHUFFLE");
      var seed = shuffle?.Attribute("seed")?.Value;
      var generator = new WallGenerator(seed);

      var go = xElement.Element("GO");
      result.Lobby = go?.Attribute("lobby")?.Value;
      var flags = (GameTypeFlag) int.Parse(go?.Attribute("type")?.Value);
      result.Rules = RuleSet.Parse(flags);
      if (result.Rules == null)
      {
        return null;
      }

      result.Room = Room.Parse(flags);
      if (result.Room == null)
      {
        return null;
      }

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
            game.Actions.Add(index);
          }
          hands[playerId].Remove(tileId);
          continue;
        }
        switch (name)
        {
          case "SHUFFLE":
          case "TAIKYOKU":
          case "GO":
            break;
          case "BYE":
            game.Actions.Add(Ids.DisconnectBase + ToInt(e.Attribute("who")?.Value));
            break;
          case "UN":
            foreach (var i in Enumerable.Range(0, 4))
            {
              if (e.Attribute($"n{i}") == null)
              {
                continue;
              }
              game.Actions.Add(Ids.ReconnectBase + i);
            }
            break;
          case "DORA":
            game.Actions.Add(Ids.Dora);
            break;
          case "INIT":
            game = new Game();
            
            game.Wall.AddRange(generator.GetWall(games.Count));
            game.Dice.AddRange(generator.GetDice(games.Count));
            game.Oya = ToInt(e.Attribute("oya")?.Value);
            game.Scores.AddRange(ToInts(e.Attribute("ten")?.Value));

            // (round indicator), (honba), (riichi sticks), (dice0), (dice1), (dora indicator)
            var gameSeed = ToInts(e.Attribute("seed")?.Value).ToList();
            game.Round = gameSeed[0];
            game.Honba = gameSeed[1];
            game.Riichi = gameSeed[2];

            if (games.Count > 1)
            {
              var prev = games[games.Count - 1];
              if (prev.Round == game.Round)
              {
                game.Repetition = prev.Repetition + 1;
              }
            }

            games.Add(game);

            upcomingRinshan = false;
            hands = GetStartingHands(e).ToList();

            break;
          case "N":
          {
            var decoder = new MeldDecoder(e.Attribute("m")?.Value);

            game.Actions.Add(MeldTypeIds[decoder.MeldType]);
            var call = new Call();
            call.Tiles.AddRange(decoder.Tiles);
            game.Calls.Add(call);

            var who = ToInt(e.Attribute("who")?.Value);
            hands[who].RemoveAll(t => decoder.Tiles.Contains(t));
            upcomingRinshan = IsKan(decoder.MeldType);
            break;
          }
          case "REACH":
            var step = e.Attribute("step")?.Value;
            game.Actions.Add(step == "1" ? Ids.Riichi : Ids.RiichiPayment);
            break;
          case "AGARI":
          {
            var who = ToInt(e.Attribute("who")?.Value);
            var fromWho = ToInt(e.Attribute("fromWho")?.Value);

            game.Actions.Add(who == fromWho ? Ids.Tsumo : Ids.Ron);

            // Alternating list of yaku ID and yaku value.
            var yakuAndHan = ToInts(e.Attribute("yaku")?.Value).ToList();

            var agari = new Agari();
            agari.Winner = who;
            agari.From = fromWho;
            agari.Fu = ToInts(e.Attribute("ten")?.Value).First();
            agari.ScoreChanges.AddRange(ToInts(e.Attribute("sc")?.Value).Stride(2, 1));
            for (var i = 0; i < yakuAndHan.Count; i += 2)
            {
              agari.Yaku.Add(new Yaku { Id = yakuAndHan[i], Han = yakuAndHan[i + 1] });
            }

            game.Agaris.Add(agari);

            AddOwari(result, e);

            break;
          }
          case "RYUUKYOKU":
            game.Actions.Add(GetRyuukyokuId(e));

            var ryuukyoku = new Ryuukyoku();
            ryuukyoku.ScoreChanges.AddRange(ToInts(e.Attribute("sc")?.Value).Stride(2, 1));
            ryuukyoku.Revealed.AddRange(GetRevealedHands(e));

            AddOwari(result, e);

            break;
          default:
            throw new NotImplementedException();
        }
      }
      result.Games = games;
      return result;
    }

    private static void AddOwari(Replay replay, XElement element)
    {
      var attribute = element.Attribute("owari");
      if (attribute == null)
      {
        return;
      }
      var values = attribute.Value.Split(',');
      replay.Owari.Scores.AddRange(values.Stride(2).Select(ToInt));
      replay.Owari.Points.AddRange(values.Stride(2, 1).Select(ToDecimal));
    }

    private static IEnumerable<bool> GetRevealedHands(XElement e)
    {
      return Enumerable.Range(0, 4).Select(i => e.Attribute($"hai{i}") != null);
    }

    private static int GetRyuukyokuId(XElement element)
    {
      var ryuukyouType = element.Attribute("type")?.Value;
      return ryuukyouType == null ? Ids.ExhaustiveDraw : RyuukyokuTypeIds[ryuukyouType];
    }

    private static IEnumerable<List<int>> GetStartingHands(XElement element)
    {
      return Enumerable.Range(0, 4).Select(i => ToInts(element.Attribute($"hai{i}")?.Value)).Select(t => t.ToList());
    }

    private static IEnumerable<string> GetUserNames(XElement element)
    {
      return Enumerable.Range(0, 4).Select(i => DecodeName(element.Attribute($"n{i}")?.Value));
    }

    private static string DecodeName(string encodedName)
    {
      if (encodedName.Length == 0 || encodedName[0] != '%')
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

    private static IEnumerable<int> ToInts(string value)
    {
      return value.Split(',').Select(ToInt);
    }

    private static int ToInt(string value)
    {
      return Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    private static decimal ToDecimal(string value)
    {
      return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Ids for events during a match. 0-12 are used for discards, defining the index of the discard in a sorted hand.
    /// </summary>
    private struct Ids
    {
      public const int Draw = 40;
      public const int Tsumogiri = 41;

      public const int Ron = 50;
      public const int Tsumo = 51;

      public const int ExhaustiveDraw = 60;
      public const int NineYaochuuHai = 61;
      public const int FourRiichi = 62;
      public const int ThreeRon = 63;
      public const int FourKan = 64;
      public const int FourWind = 65;
      public const int NagashiMangan = 66;

      public const int Pon = 70;
      public const int Chii = 71;
      public const int ClosedKan = 72;
      public const int CalledKan = 73;
      public const int AddedKan = 74;

      public const int Dora = 80;
      public const int Rinshan = 81;
      public const int Riichi = 82;
      public const int RiichiPayment = 83;

      // If player n disconnects, the id is this + n.
      public const int DisconnectBase = 90;

      // If player n reconnects, the id is this + n.
      public const int ReconnectBase = 94;
    }

    private struct RyuukyokuTypes
    {
      public const string NineYaochuuHai = "yao9";
      public const string FourRiichi = "reach4";
      public const string ThreeRon = "ron3";
      public const string FourKan = "kan4";
      public const string FourWind = "kaze4";
      public const string NagashiMangan = "nm";
    }
  }
}