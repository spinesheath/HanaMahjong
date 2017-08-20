// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
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
    /// Per hand: 136 tiles, 2 dice, then actions: discarded tiles or call id followed by 3-4 tiles or agari id followed by who and fromWho.
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
      var flags = (GameTypeFlag)int.Parse(go?.Attribute("type")?.Value);
      result.Rules = RuleSet.Parse(flags);
      result.Room = GetRoom(flags);

      foreach (var e in xElement.Elements())
      {
        var name = e.Name.LocalName;
        var match = DiscardRegex.Match(name);
        if (match.Success)
        {
          var tile = Convert.ToInt32(match.Groups[1].Value);
          data.Add(tile);
          continue;
        }
        switch (name)
        {
          case "SHUFFLE":
          case "UN":
          case "TAIKYOKU":
          case "GO":
          case "BYE":
          case "DORA":
            break;
          case "INIT":
            data.Add(InitId);
            data.AddRange(generator.GetWall(gameIndex));
            data.AddRange(generator.GetDice(gameIndex));
            gameIndex += 1;
            break;
          case "AGARI":
            var who = Convert.ToInt32(e.Attribute("who")?.Value);
            var fromWho = Convert.ToInt32(e.Attribute("fromWho")?.Value);
            data.Add(AgariId);
            data.Add(who);
            data.Add(fromWho);
            break;
          case "N":
            var m = e.Attribute("m")?.Value;
            var decoder = new MeldDecoder(m);
            data.Add(MeldTypeIds[decoder.MeldType]);
            data.AddRange(decoder.Tiles);
            break;
          case "REACH":
            throw new NotImplementedException();
          case "RYUUKYOKU":
            throw new NotImplementedException();
        }
      }
      result.Data = data.ToArray();
      return result;
    }

    private Replay()
    {
    }

    private const int AgariId = 300;
    private const int InitId = 400;

    private static readonly Regex DiscardRegex = new Regex(@"[DEFG](\d{1,3})");

    private static readonly Dictionary<MeldType, int> MeldTypeIds = new Dictionary<MeldType, int>
    {
      {MeldType.AddedKan, 200},
      {MeldType.CalledKan, 201},
      {MeldType.ClosedKan, 202},
      {MeldType.Koutsu, 203},
      {MeldType.Shuntsu, 204}
    };

    private static Room GetRoom(GameTypeFlag flags)
    {
      if (flags.HasFlag(GameTypeFlag.Advanced))
      {
        return flags.HasFlag(GameTypeFlag.Expert) ? Room.Phoenix : Room.LowerDan;
      }
      return flags.HasFlag(GameTypeFlag.Expert) ? Room.UpperDan : Room.General;
    }
  }
}