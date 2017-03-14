// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spines.Mahjong.Analysis.Classification
{
  internal class ShorthandParser
  {
    public ShorthandParser(string hand)
    {
      _hand = hand;
    }

    public IEnumerable<int> Concealed => Manzu.Concat(Pinzu).Concat(Souzu).Concat(Jihai);

    public IEnumerable<int> Manzu => GetTiles('m', 9);

    public IEnumerable<int> Pinzu => GetTiles('p', 9);

    public IEnumerable<int> Souzu => GetTiles('s', 9);

    public IEnumerable<int> Jihai => GetTiles('z', 7);

    public IEnumerable<int> ManzuMeldIds => GetMelds('M');

    public IEnumerable<int> PinzuMeldIds => GetMelds('P');

    public IEnumerable<int> SouzuMeldIds => GetMelds('S');

    public IEnumerable<int> JihaiMeldIds => GetMelds('Z');

    private readonly string _hand;

    private IEnumerable<int> GetMelds(char tileGroupName)
    {
      var regex = new Regex(@"(\d*)" + tileGroupName);
      var groups = regex.Matches(_hand).OfType<Match>().SelectMany(m => m.Groups.OfType<Group>().Skip(1));
      foreach (var captureGroup in groups)
      {
        var tiles = captureGroup.Value.Select(c => (int) char.GetNumericValue(c) - 1).ToList();
        var i = tiles.Min();
        var isShuntsu = tiles.Any(t => t != i);
        var isKantsu = tiles.Count == 4;
        if (isShuntsu)
        {
          yield return i;
        }
        else if (isKantsu)
        {
          yield return 7 + 9 + i;
        }
        else
        {
          yield return 7 + i;
        }
      }
    }

    private IEnumerable<int> GetTiles(char tileGroupName, int typesInSuit)
    {
      var regex = new Regex(@"(\d*)" + tileGroupName);
      var groups = regex.Matches(_hand).OfType<Match>().SelectMany(m => m.Groups.OfType<Group>().Skip(1));
      var tiles = groups.SelectMany(g => g.Value).Select(c => (int) char.GetNumericValue(c) - 1);
      var idToCount = tiles.GroupBy(t => t).ToDictionary(g => g.Key, g => g.Count());
      return Enumerable.Range(0, typesInSuit).Select(i => idToCount.ContainsKey(i) ? idToCount[i] : 0);
    }
  }
}