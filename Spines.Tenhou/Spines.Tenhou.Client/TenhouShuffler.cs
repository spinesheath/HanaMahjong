// Spines.Tenhou.Client.TenhouShuffler.cs
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

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// A shuffler used by tenhou.net, based on the mersenne twister MT19937.
  /// </summary>
  internal class TenhouShuffler
  {
    private const int NumberOfWords = 624;
    private const int ParallelSequenceNumber = 397;
    private const uint UpperMask = 0x80000000;
    private const uint LowerMask = 0x7fffffff;
    private readonly uint[] _evenOddXorValues = {0, 0x9908b0df};
    private readonly uint[] _words = new uint[NumberOfWords];
    private uint _nextWordIndex;

    /// <summary>
    /// Creates a new instance of TenhouShuffler and initializes it with a seed.
    /// </summary>
    /// <param name="seed">The seed to initialize the shuffler with.</param>
    public TenhouShuffler(IEnumerable<int> seed)
    {
      unchecked
      {
        var seedArray = seed.Select(x => (uint) x).ToArray();
        Init(19650218);

        uint i = 1;
        uint j = 0;

        for (var k = Math.Max(NumberOfWords, seedArray.Length); k > 0; --k)
        {
          _words[i] ^= PreviousXor(i) * 1664525;
          _words[i] += seedArray[j] + j;

          i += 1;
          j += 1;

          if (i >= NumberOfWords)
          {
            _words[0] = _words[NumberOfWords - 1];
            i = 1;
          }
          if (j >= seedArray.Length)
          {
            j = 0;
          }
        }

        for (var k = NumberOfWords - 1; k > 0; --k)
        {
          _words[i] ^= PreviousXor(i) * 1566083941;
          _words[i] -= i;

          i += 1;
          if (i < NumberOfWords)
          {
            continue;
          }
          _words[0] = _words[NumberOfWords - 1];
          i = 1;
        }
        _words[0] = 0x80000000;
      }
    }

    /// <summary>
    /// Returns the next random number in the sequence.
    /// </summary>
    /// <returns></returns>
    public int GetNext()
    {
      if (_nextWordIndex == 0)
      {
        CreateNextState();
      }
      var t = _words[_nextWordIndex];
      _nextWordIndex = (_nextWordIndex + 1) % NumberOfWords;
      return unchecked ((int) Temper(t));
    }

    private static uint Temper(uint y)
    {
      y ^= (y >> 11);
      y ^= (y << 7) & 0x9d2c5680;
      y ^= (y << 15) & 0xefc60000;
      y ^= (y >> 18);
      return y;
    }

    private void CreateNextState()
    {
      for (var i = 0; i < NumberOfWords; ++i)
      {
        var y = (_words[i] & UpperMask) | (_words[(i + 1) % NumberOfWords] & LowerMask);
        _words[i] = _words[(i + ParallelSequenceNumber) % NumberOfWords] ^ (y >> 1) ^ _evenOddXorValues[y & 1];
      }
    }

    private void Init(uint seed)
    {
      _words[0] = seed;
      for (uint i = 1; i < NumberOfWords; i++)
      {
        unchecked
        {
          _words[i] = PreviousXor(i) * 1812433253;
          _words[i] += i;
        }
      }
    }

    private uint PreviousXor(uint i)
    {
      return _words[i - 1] ^ (_words[i - 1] >> 30);
    }
  }
}