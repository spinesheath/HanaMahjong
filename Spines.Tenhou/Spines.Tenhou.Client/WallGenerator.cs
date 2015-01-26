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
    private readonly int[] _dice = new int[2];
    private readonly int[] _seedValues;
    private int[] _wall;

    /// <summary>
    /// </summary>
    /// <param name="seed"></param>
    public WallGenerator(string seed)
    {
      Validate.NotNull(seed, "seed");
      unchecked
      {
        var delimiterPos = seed.IndexOf(",", StringComparison.Ordinal);
        var seedText = seed.Substring(delimiterPos + 1);
        if (seed.IndexOf("mt19937ar-sha512-n288-base64", StringComparison.Ordinal) == 0)
        {
          _seedValues = BytesToInts(Convert.FromBase64String(seedText)).ToArray();
        }
        else
        {
          _seedValues = DecompositeHexList(seedText);
        }
      }
    }

    /// <summary>
    /// </summary>
    /// <param name="gameIndex">The index of the game within the match.</param>
    public void Generate(int gameIndex)
    {
      var rnd = GetRandomValues(gameIndex).Select(v => unchecked ((uint)v)).ToArray();
      _wall = Enumerable.Range(0, 136).ToArray();

      // Shuffle!
      for (var i = 0; i < _wall.Length - 1; i++)
      {
        var src = i;
        var dst = i + Convert.ToInt16(rnd[i] % (136 - i));

        // Swap 2 tiles
        var swp = _wall[src];
        _wall[src] = _wall[dst];
        _wall[dst] = swp;
      }

      // Dice!
      _dice[0] = 1 + Convert.ToInt16(rnd[135] % 6);
      _dice[1] = 1 + Convert.ToInt16(rnd[136] % 6);
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public IEnumerable<int> GetDice()
    {
      return _dice;
    }

    // TODO store List of walls/dice and add new walls if one is requested that hasn't been built yet, use instance varaible of shuffler, don't skip

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public IEnumerable<int> GetWall()
    {
      return _wall;
    }

    /// <summary>
    /// Converts n*4 bytes into n ints using Buffer.BlockCopy.
    /// </summary>
    /// <param name="source">The source bytes.</param>
    /// <returns>The resulting ints.</returns>
    private static IEnumerable<int> BytesToInts(IEnumerable<byte> source)
    {
      var array = source.ToArray();
      var numberOfInts = array.Length * sizeof(byte) / sizeof(int);
      var t = new int[numberOfInts];
      Buffer.BlockCopy(array, 0, t, 0, numberOfInts * sizeof(int));
      return t;
    }

    /// <summary>
    /// Old style seed value
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static int[] DecompositeHexList(string text)
    {
      var temp = text.Split(new[] {","}, StringSplitOptions.None);
      var result = new int[temp.Length];
      for (var i = 0; i < temp.Length; i++)
      {
        var index = temp[i].IndexOf('.');
        if (index >= 0)
        {
          temp[i] = temp[i].Substring(0, index);
        }
        result[i] = unchecked ((int) Convert.ToUInt32(temp[i], 16));
      }
      return result;
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

    private IEnumerable<int> GetRandomValues(int gameIndex)
    {
      var shuffler = new TenhouShuffler(_seedValues);
      var values = Enumerable.Repeat(0, (gameIndex + 1) * 288).Select(n => shuffler.GetNext()).Skip(gameIndex * 288);
      return CreateChunks(IntsToBytes(values), 128).SelectMany(ComputeHash);
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
      return source.Select((s, i) => new { Value = s, Index = i }).GroupBy(item => item.Index / chunkSize, item => item.Value);
    }
  }
}