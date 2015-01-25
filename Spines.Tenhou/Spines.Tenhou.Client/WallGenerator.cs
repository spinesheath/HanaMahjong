using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Spines.Utility;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// 
  /// </summary>
  public class WallGenerator
  {
    private readonly uint[] _seedValues = new uint[624];
    private readonly int[] _wall = new int[136];
    private readonly int[] _dice = new int[2];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="seed"></param>
    public WallGenerator(string seed)
    {
      unchecked
      {
        var delimiterPos = seed.IndexOf(",", StringComparison.Ordinal);

        var seedText = seed.Substring(delimiterPos + 1);

        // mt19937ar-sha512-n288-base64,*data*

        if (seed.IndexOf("mt19937ar-sha512-n288-base64", StringComparison.Ordinal) == 0)
        {
          byte[] seedBytes = Convert.FromBase64String(seedText);

          // Convert to UInt32 array
          for (var i = 0; i < _seedValues.Length; i++)
          {
            _seedValues[i] = (uint)(seedBytes[i * 4 + 0] << 0) |
                             (uint)(seedBytes[i * 4 + 1] << 8) |
                             (uint)(seedBytes[i * 4 + 2] << 16) |
                             (uint)(seedBytes[i * 4 + 3] << 24);
          }
        }
        else
        {
          // Old style seed value
          _seedValues = DecompositeHexList(seedText);
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static uint[] DecompositeHexList(string text)
    {
      unchecked
      {
        string[] delimiter = { "," };

        if (text == null) return null;

        var temp = text.Split(delimiter, StringSplitOptions.None);
        var result = new uint[temp.Length];

        for (var i = 0; i < temp.Length; i++)
        {
          var index = temp[i].IndexOf('.');
          if (index >= 0) temp[i] = temp[i].Substring(0, index);

          result[i] = Convert.ToUInt32(temp[i], 16);
        }

        return result;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameIndex">The index of the game within the match.</param>
    public void Generate(int gameIndex)
    {
      unchecked
      {
        var shuffler = new TenhouShuffler(_seedValues.Select(x => (int) x).ToArray());

        var rnd = new uint[64 / 4 * 9]; // 144

        // SHA512 hash
        for (var j = 0; j < gameIndex + 1; j++)
        {
          var src = new uint[64 / 4 * 9 * 2];

          for (var i = 0; i < src.Length; i++)
          {
            src[i] = (uint) shuffler.GetNext();
          }

          var srcbyte = new byte[src.Length * 4];
          for (var i = 0; i < src.Length; i++)
          {
            srcbyte[i * 4 + 0] = Convert.ToByte((src[i] >> 0) & 0xFF);
            srcbyte[i * 4 + 1] = Convert.ToByte((src[i] >> 8) & 0xFF);
            srcbyte[i * 4 + 2] = Convert.ToByte((src[i] >> 16) & 0xFF);
            srcbyte[i * 4 + 3] = Convert.ToByte((src[i] >> 24) & 0xFF);
          }

          for (var i = 0; i < 9; ++i)
          {
            using(var context = new SHA512Managed())
            {
              var datatohash = new byte[128];
              for (var k = 0; k < 128; k++)
              {
                datatohash[k] = srcbyte[k + i * 128];
              }

              var hash = context.ComputeHash(datatohash);

              for (var k = 0; k < 16; k++)
              {
                rnd[i * 16 + k] = (uint)(hash[k * 4 + 0] << 0) |
                                  (uint)(hash[k * 4 + 1] << 8) |
                                  (uint)(hash[k * 4 + 2] << 16) |
                                  (uint)(hash[k * 4 + 3] << 24);
              }
            }
          }
        }

        // Generate wall
        for (var i = 0; i < _wall.Length; i++) _wall[i] = i;

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
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<int> GetWall()
    {
      var temp = new int[136];
      for (var i = 0; i < _wall.Length; i++) temp[i] = _wall[i];

      return temp;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int[] GetDice()
    {
      var temp = new int[2];
      for (var i = 0; i < _dice.Length; i++) temp[i] = _dice[i];

      return temp;
    }
  }

}