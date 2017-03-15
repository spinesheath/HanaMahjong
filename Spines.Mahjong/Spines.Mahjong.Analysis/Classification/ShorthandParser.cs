// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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

    public IEnumerable<int> Manzu => GetTiles('m', 9, new char[0]);

    public IEnumerable<int> Pinzu => GetTiles('p', 9, new char[0]);

    public IEnumerable<int> Souzu => GetTiles('s', 9, new char[0]);

    public IEnumerable<int> Jihai => GetTiles('z', 7, new[] {'0', '8', '9'});

    public IEnumerable<int> ManzuMeldIds => GetMelds('M', new char[0]);

    public IEnumerable<int> PinzuMeldIds => GetMelds('P', new char[0]);

    public IEnumerable<int> SouzuMeldIds => GetMelds('S', new char[0]);

    public IEnumerable<int> JihaiMeldIds => GetMelds('Z', new[] {'0', '8', '9'});

    private readonly string _hand;

    private IEnumerable<int> GetMelds(char tileGroupName, char[] forbidden)
    {
      var regex = new Regex(@"(\d*)" + tileGroupName);
      var groups = regex.Matches(_hand).OfType<Match>().SelectMany(m => m.Groups.OfType<Group>().Skip(1));
      foreach (var captureGroup in groups)
      {
        if (forbidden.Intersect(captureGroup.Value).Any())
        {
          throw GetForbiddenDigitsException(tileGroupName, forbidden);
        }
        var tiles = captureGroup.Value.Select(GetTileTypeIndex).OrderBy(x => x).ToList();
        var i = tiles.Min();
        if (tiles.SequenceEqual(Enumerable.Range(i, 3)))
        {
          yield return i;
        }
        else if (tiles.SequenceEqual(Enumerable.Repeat(i, 3)))
        {
          yield return 7 + i;
        }
        else if (tiles.SequenceEqual(Enumerable.Repeat(i, 4)))
        {
          yield return 7 + 9 + i;
        }
        else
        {
          throw new FormatException(captureGroup.Value + " is not a valid meld.");
        }
      }
    }

    private IEnumerable<int> GetTiles(char tileGroupName, int typesInSuit, char[] forbidden)
    {
      var regex = new Regex(@"(\d*)" + tileGroupName);
      var groups = regex.Matches(_hand).OfType<Match>().SelectMany(m => m.Groups.OfType<Group>().Skip(1));
      var digits = groups.SelectMany(g => g.Value).ToList();
      if (digits.Intersect(forbidden).Any())
      {
        throw GetForbiddenDigitsException(tileGroupName, forbidden);
      }
      var tiles = digits.Select(GetTileTypeIndex);
      var idToCount = tiles.GroupBy(t => t).ToDictionary(g => g.Key, g => g.Count());
      return Enumerable.Range(0, typesInSuit).Select(i => idToCount.ContainsKey(i) ? idToCount[i] : 0);
    }

    private static FormatException GetForbiddenDigitsException(char tileGroupName, char[] forbidden)
    {
      return new FormatException(string.Join(",", forbidden) + " are not allowed in group " + tileGroupName + ".");
    }

    /// <summary>
    /// Retruns the index of a tile type within a suit.
    /// </summary>
    /// <param name="digit">The digit that represents the tile in shorthand notation.</param>
    /// <returns>The index of the tile type.</returns>
    private static int GetTileTypeIndex(char digit)
    {
      var numericValue = (int) char.GetNumericValue(digit);
      if (numericValue == 0)
      {
        return 4;
      }
      return numericValue - 1;
    }
  }
}