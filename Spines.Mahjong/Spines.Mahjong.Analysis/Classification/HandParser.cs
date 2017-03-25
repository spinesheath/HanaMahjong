// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// Parses a shorthand string, respecting the order of the tiles.
  /// If the hand has 14 tiles (melds counting for 3), the last concealed tile is treated as the draw.
  /// </summary>
  public static class HandParser
  {
    /// <summary>
    /// Parses the string.
    /// </summary>
    /// <param name="shorthand">The string to parse.</param>
    /// <returns>The parsed hand.</returns>
    /// <exception cref="FormatException">Thrown if the string does not specify a valid hand.</exception>
    public static Hand Parse(string shorthand)
    {
      var tiles = CreateTiles(shorthand).ToList();
      var melds = CreateMelds(shorthand).ToList();

      if (melds.Count > 4)
      {
        throw new FormatException("Too many melds.");
      }

      var expected = 13 - melds.Count * 3;
      if (tiles.Count > expected + 1 || tiles.Count < expected)
      {
        throw new FormatException("Wrong number of tiles.");
      }

      var bySuit = tiles.Concat(melds.SelectMany(m => m.Tiles)).GroupBy(t => t.Suit).ToList();
      if (bySuit.Any(g => g.Count(t => t.Aka) > 1))
      {
        throw new FormatException("Too many aka dora.");
      }
      if (bySuit.Any(g => g.GroupBy(t => t.Index).Any(gg => gg.Count() > 4)))
      {
        throw new FormatException("Too many of the same tile type.");
      }

      if (tiles.Count == expected)
      {
        return new Hand(tiles, melds, null);
      }
      return new Hand(tiles.Take(expected), melds, tiles[expected]);
    }

    private static readonly Dictionary<char, Suit> CharToSuit = new Dictionary<char, Suit>
    {
      {'m', Suit.Manzu},
      {'p', Suit.Pinzu},
      {'s', Suit.Souzu},
      {'z', Suit.Jihai},
      {'M', Suit.Manzu},
      {'P', Suit.Pinzu},
      {'S', Suit.Souzu},
      {'Z', Suit.Jihai}
    };

    private static readonly Regex BlockRegex = new Regex("([0-9]+[mps]|[1-7]+z)");
    private static readonly Regex MeldRegex = new Regex("([0-9']+[MPS]|[1-7']+Z)");

    private static IEnumerable<Meld> CreateMelds(string shorthand)
    {
      var matches = MeldRegex.Matches(shorthand).OfType<Match>();
      var groups = matches.SelectMany(m => m.Groups.OfType<Group>().Skip(1));
      var blocks = groups.Select(g => g.Value);
      foreach (var block in blocks)
      {
        var suit = CharToSuit[block.Last()];
        var tiles = new List<Tile>();
        for (var i = 0; i < block.Length - 1; ++i)
        {
          if (block[i] == '\'')
          {
            throw new FormatException("Apostrophe can not lead a meld or occur as a pair.");
          }
          if (block[i + 1] == '\'')
          {
            var firstCall = tiles.All(t => t.Location != TileLocation.Called);
            var location = firstCall ? TileLocation.Called : TileLocation.Added;
            if (!firstCall && tiles.Last().Location != TileLocation.Called)
            {
              throw new FormatException("Second apostrophe in a meld must follow the next tile after the first apostrophe.");
            }
            tiles.Add(CreateTile(block[i], suit, location));
            i += 1;
          }
          else
          {
            tiles.Add(CreateTile(block[i], suit, TileLocation.Melded));
          }
        }
        if (tiles.Count(t => t.Aka) > 1)
        {
          throw new FormatException("Too many aka dora.");
        }
        var min = tiles.Min(t => t.Index);
        if (tiles.Select(t => t.Index).OrderBy(x => x).SequenceEqual(Enumerable.Range(min, 3)))
        {
          if (suit == Suit.Jihai)
          {
            throw new FormatException("Shuntsu for honors not legal.");
          }
          if (tiles[0].Location == TileLocation.Called && tiles.Skip(1).All(t => t.Location == TileLocation.Melded) && tiles[1].Index < tiles[2].Index)
          {
            yield return new Meld {Tiles = tiles};
          }
          else
          {
            throw new FormatException("Shuntsu formatted incorrectly.");
          }
        }
        else if (tiles.Select(t => t.Index).OrderBy(x => x).SequenceEqual(Enumerable.Repeat(min, 3)))
        {
          if (tiles.Count(t => t.Location == TileLocation.Called) != 1 || tiles.Count(t => t.Location == TileLocation.Melded) != 2)
          {
            throw new FormatException("Koutsu formatted incorrectly.");
          }
          yield return new Meld {Tiles = tiles};
        }
        else if (tiles.Select(t => t.Index).OrderBy(x => x).SequenceEqual(Enumerable.Repeat(min, 4)))
        {
          if (tiles.All(t => t.Location == TileLocation.Melded))
          {
            var index = tiles[0].Index;
            var ankanTiles = new List<Tile>
            {
              new Tile {Aka = index == 4, Index = index, Suit = suit, Location = TileLocation.FaceDown},
              new Tile {Index = index, Suit = suit, Location = TileLocation.Melded},
              new Tile {Index = index, Suit = suit, Location = TileLocation.Melded},
              new Tile {Index = index, Suit = suit, Location = TileLocation.FaceDown}
            };
            yield return new Meld {Tiles = ankanTiles};
          }
          else if (tiles.Count(t => t.Location == TileLocation.Called) == 1 && tiles.Count(t => t.Location == TileLocation.Melded) == 3)
          {
            yield return new Meld {Tiles = tiles};
          }
          else if (tiles.Count(t => t.Location == TileLocation.Called) == 1 && tiles.Count(t => t.Location == TileLocation.Melded) == 2
                   && tiles.Count(t => t.Location == TileLocation.Added) == 1)
          {
            yield return new Meld {Tiles = tiles};
          }
          else
          {
            throw new FormatException("Kantsu formatted incorrectly.");
          }
        }
        else
        {
          throw new FormatException("Invalid meld format.");
        }
      }
    }

    private static IEnumerable<Tile> CreateTiles(string shorthand)
    {
      var matches = BlockRegex.Matches(shorthand).OfType<Match>();
      var groups = matches.SelectMany(m => m.Groups.OfType<Group>().Skip(1));
      var blocks = groups.Select(g => g.Value).ToList();
      foreach (var block in blocks)
      {
        var suit = CharToSuit[block.Last()];
        var digits = block.Take(block.Length - 1);
        foreach (var digit in digits)
        {
          yield return CreateTile(digit, suit, TileLocation.Concealed);
        }
      }
    }

    private static Tile CreateTile(char digit, Suit suit, TileLocation location)
    {
      var numericValue = (int) char.GetNumericValue(digit);
      var aka = numericValue == 0;
      var index = aka ? 4 : numericValue - 1;
      return new Tile {Aka = aka, Index = index, Location = location, Suit = suit};
    }
  }
}