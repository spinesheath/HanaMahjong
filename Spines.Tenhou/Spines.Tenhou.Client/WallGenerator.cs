// Spines.Tenhou.Client.WallGenerator.cs
// 
// Copyright (C) 2015  Johannes Heckl
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Spines.Utility;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// </summary>
  public class WallGenerator
  {
    private readonly IList<IEnumerable<int>> _dice = new List<IEnumerable<int>>();
    private readonly TenhouShuffler _shuffler;
    private readonly IList<IEnumerable<int>> _walls = new List<IEnumerable<int>>();

    /// <summary>
    /// Creates a new instance of WallGenerator.
    /// </summary>
    /// <param name="seed">The seed that is used to initialize the shuffler.</param>
    public WallGenerator(string seed)
    {
      Validate.NotNull(seed, "seed");
      _shuffler = new TenhouShuffler(CreateSeeds(seed));
    }

    /// <summary>
    /// Gets the dice for a game.
    /// </summary>
    /// <param name="gameIndex">The index of the game within the match.</param>
    /// <returns>A sequence of 2 dice.</returns>
    public IEnumerable<int> GetDice(int gameIndex)
    {
      Validate.NotNegative(gameIndex, "gameIndex");
      while (_dice.Count <= gameIndex)
      {
        Generate();
      }
      return _dice[gameIndex];
    }

    /// <summary>
    /// Gets the wall of a game. The tiles 0 through 13 in the sequence form the dead wall.
    /// 5,7,9,11 are dora indicators, 4,6,8,10 are ura indicators.
    /// The dealer gets the last 4 tiles, the next player the second to last 4 tiles and so on.
    /// </summary>
    /// <param name="gameIndex">The index of the game within the match.</param>
    /// <returns>A sequence of 136 tiles.</returns>
    public IEnumerable<int> GetWall(int gameIndex)
    {
      Validate.NotNegative(gameIndex, "gameIndex");
      while (_walls.Count <= gameIndex)
      {
        Generate();
      }
      return _walls[gameIndex];
    }

    /// <summary>
    /// Converts n*4 bytes into n ints using Buffer.BlockCopy.
    /// </summary>
    /// <param name="source">The source bytes.</param>
    /// <returns>The resulting ints.</returns>
    private static IEnumerable<int> BytesToInts(IEnumerable<byte> source)
    {
      var array = source.ToArray();
      var numberOfInts = array.Length * sizeof (byte) / sizeof (int);
      var t = new int[numberOfInts];
      Buffer.BlockCopy(array, 0, t, 0, numberOfInts * sizeof (int));
      return t;
    }

    private static IEnumerable<int> ComputeHash(IEnumerable<byte> chunk)
    {
      using (var context = new SHA512Managed())
      {
        return BytesToInts(context.ComputeHash(chunk.ToArray()));
      }
    }

    private static IEnumerable<IEnumerable<T>> CreateChunks<T>(IEnumerable<T> source, int chunkSize)
    {
      var indexedValues = source.Select((s, i) => new {Value = s, Index = i});
      return indexedValues.GroupBy(item => item.Index / chunkSize, item => item.Value);
    }

    private static IEnumerable<int> CreateSeeds(string seed)
    {
      var parts = seed.Split(new[] {','}, StringSplitOptions.None);
      if (parts.Length == 2 && parts[0] == "mt19937ar-sha512-n288-base64")
      {
        return BytesToInts(Convert.FromBase64String(parts[1]));
      }
      return ConvertOldSeed(parts.Skip(1));
    }

    /// <summary>
    /// Converts the old style seed value (I've never even seen that one).
    /// </summary>
    /// <param name="parts">The original seed string split at each ',', skipping the first part of the split.</param>
    /// <returns>A sequence of ints.</returns>
    private static IEnumerable<int> ConvertOldSeed(IEnumerable<string> parts)
    {
      var prefixes = parts.Select(s => s.Split(new[] {'.'}, StringSplitOptions.None).First());
      return prefixes.Select(s => unchecked ((int) Convert.ToUInt32(s, 16)));
    }

    /// <summary>
    /// Converts n ints into n*4 bytes using Buffer.BlockCopy.
    /// </summary>
    /// <param name="source">The source ints.</param>
    /// <returns>The resulting bytes.</returns>
    private static IEnumerable<byte> IntsToBytes(IEnumerable<int> source)
    {
      var array = source.ToArray();
      var numberOfBytes = array.Length * sizeof (int) / sizeof (byte);
      var t = new byte[numberOfBytes];
      Buffer.BlockCopy(array, 0, t, 0, numberOfBytes * sizeof (byte));
      return t;
    }

    private void Generate()
    {
      var rnd = GetRandomValues().Select(v => unchecked ((uint) v)).ToArray();

      var wall = Enumerable.Range(0, 136).ToArray();
      // Shuffle!
      for (var i = 0; i < wall.Length - 1; i++)
      {
        var src = i;
        var dst = i + Convert.ToInt16(rnd[i] % (136 - i));
        // Swap 2 tiles
        var swp = wall[src];
        wall[src] = wall[dst];
        wall[dst] = swp;
      }
      _walls.Add(wall);

      var d1 = 1 + Convert.ToInt16(rnd[135] % 6);
      var d2 = 1 + Convert.ToInt16(rnd[136] % 6);
      _dice.Add(new[] {d1, d2});
    }

    private IEnumerable<int> GetRandomValues()
    {
      var values = Enumerable.Repeat(0, 288).Select(n => _shuffler.GetNext());
      return CreateChunks(IntsToBytes(values), 128).SelectMany(ComputeHash);
    }
  }
}