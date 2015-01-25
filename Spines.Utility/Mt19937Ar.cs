// Spines.Utility.Mt19937Ar.cs
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

namespace Spines.Utility
{
  /// <summary>
  /// </summary>
  public class Mt19937Ar
  {
    private const int StateSize = 624;
    private const int M = 397;

    private const uint MatrixA = 0x9908b0df; // constant vector a
    private const uint UpperMask = 0x80000000; // most significant w-r bits
    private const uint LowerMask = 0x7fffffff; // least significant r bits
    private readonly uint[] _mag01 = {0x0, MatrixA};

    private readonly uint[] _state = new uint[StateSize]; // the array for the state vector
    private uint _nextStateIndex;

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public int GetNext()
    {
      if (_nextStateIndex == 0)
      {
        CreateNextState();
      }

      var t = _state[_nextStateIndex];
      _nextStateIndex = (_nextStateIndex + 1) % StateSize;
      return unchecked ((int)Temper(t));
    }

    private void CreateNextState()
    {
      for (var k = 0; k < StateSize - M; ++k)
      {
        ApplyMatrix(k, k + M, GetUpperLower(k, k + 1));
      }
      for (var k = StateSize - M; k < StateSize - 1; ++k)
      {
        ApplyMatrix(k, k + (M - StateSize), GetUpperLower(k, k + 1));
      }
      ApplyMatrix(StateSize - 1, M - 1, GetUpperLower(StateSize - 1, 0));
    }

    private void ApplyMatrix(int targetIndex, int sourceIndex, uint matrixIndex)
    {
      _state[targetIndex] = _state[sourceIndex] ^ (matrixIndex >> 1) ^ _mag01[matrixIndex & 0x1];
    }

    private uint GetUpperLower(int upperIndex, int lowerIndex)
    {
      return (_state[upperIndex] & UpperMask) | (_state[lowerIndex] & LowerMask);
    }

    private static uint Temper(uint y)
    {
      y ^= (y >> 11);
      y ^= (y << 7) & 0x9d2c5680;
      y ^= (y << 15) & 0xefc60000;
      y ^= (y >> 18);
      return y;
    }

    /// <summary>
    /// </summary>
    /// <param name="seed"></param>
    private void Init(uint seed)
    {
      _state[0] = seed;
      for (uint i = 1; i < StateSize; i++)
      {
        unchecked
        {
          _state[i] = PreviousXor(i) * 1812433253;
          _state[i] += i;
        }
      }
    }

    private uint PreviousXor(uint i)
    {
      return _state[i - 1] ^ (_state[i - 1] >> 30);
    }

    /// <summary>
    /// </summary>
    /// <param name="seed"></param>
    public void InitByArray(IEnumerable<int> seed)
    {
      unchecked
      {
        var seedArray = seed.Select(x => (uint)x).ToArray();
        Init(19650218);

        uint i = 1;
        uint j = 0;

        for (var k = Math.Max(StateSize, seedArray.Length); k > 0; --k)
        {
          _state[i] ^= PreviousXor(i) * 1664525;
          _state[i] += seedArray[j] + j;

          i += 1;
          j += 1;

          if (i >= StateSize)
          {
            _state[0] = _state[StateSize - 1];
            i = 1;
          }
          if (j >= seedArray.Length)
          {
            j = 0;
          }
        }

        for (var k = StateSize - 1; k > 0; --k)
        {
          _state[i] ^= PreviousXor(i) * 1566083941;
          _state[i] -= i;

          i += 1;
          if (i < StateSize)
          {
            continue;
          }
          _state[0] = _state[StateSize - 1];
          i = 1;
        }
        _state[0] = 0x80000000;
      }
    }
  }
}