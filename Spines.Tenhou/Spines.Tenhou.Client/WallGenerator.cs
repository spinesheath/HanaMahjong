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
          _seedValues = BytesToInts(Convert.FromBase64String(seedText));
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
      var rnd = GetRandomValues(gameIndex);
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
    public int[] GetDice()
    {
      var temp = new int[2];
      for (var i = 0; i < _dice.Length; i++)
      {
        temp[i] = _dice[i];
      }
      return temp;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public IEnumerable<int> GetWall()
    {
      var temp = new int[136];
      for (var i = 0; i < _wall.Length; i++)
      {
        temp[i] = _wall[i];
      }
      return temp;
    }

    /// <summary>
    /// Converts an array of n*4 bytes into an array of n ints using Buffer.BlockCopy.
    /// </summary>
    /// <param name="bytes">The source bytes.</param>
    /// <returns>The resulting ints.</returns>
    private static int[] BytesToInts(byte[] bytes)
    {
      var numberOfInts = bytes.Length * sizeof (byte) / sizeof (int);
      var t = new int[numberOfInts];
      Buffer.BlockCopy(bytes, 0, t, 0, numberOfInts * sizeof (int));
      return t;
    }

    /// <summary>
    /// Old style seed value
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static int[] DecompositeHexList(string text)
    {
      unchecked
      {
        string[] delimiter = {","};

        if (text == null)
        {
          return null;
        }

        var temp = text.Split(delimiter, StringSplitOptions.None);
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
    }

    /// <summary>
    /// Converts an array of n*4 bytes into an array of n ints using Buffer.BlockCopy.
    /// </summary>
    /// <param name="ints">The source bytes.</param>
    /// <returns>The resulting ints.</returns>
    private static byte[] IntsToBytes(int[] ints)
    {
      var numberOfBytes = ints.Length * sizeof (int) / sizeof (byte);
      var t = new byte[numberOfBytes];
      Buffer.BlockCopy(ints, 0, t, 0, numberOfBytes * sizeof (byte));
      return t;
    }

    private uint[] GetRandomValues(int gameIndex)
    {
      var shuffler = new TenhouShuffler(_seedValues);
      var values =
        Enumerable.Repeat(0, (gameIndex + 1) * 16 * 9 * 2).Select(n => shuffler.GetNext()).Skip(gameIndex * 16 * 9 * 2);
      var srcbyte = IntsToBytes(values.ToArray()); // 128 * 9 bytes
      var rnd = new uint[16 * 9];
      for (var i = 0; i < 9; ++i)
      {
        using (var context = new SHA512Managed())
        {
          var hash = BytesToInts(context.ComputeHash(srcbyte.Skip(i * 128).Take(128).ToArray()));
          for (var k = 0; k < 16; k++)
          {
            rnd[i * 16 + k] = unchecked ((uint) hash[k]);
          }
        }
      }
      return rnd;
    }
  }
}