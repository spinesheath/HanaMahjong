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
  /// 
  /// </summary>
  public class Mt19937Ar
  {
    private const int N = 624;
        private const int M = 397;

        private const uint MATRIX_A = 0x9908b0df;   // constant vector a
        private const uint UPPER_MASK = 0x80000000; // most significant w-r bits
        private const uint LOWER_MASK = 0x7fffffff; // least significant r bits

        private uint[] mt = new uint[N]; // the array for the state vector
        private uint mti = N + 1;        // means mt[N] is not initialized

        private uint[] mag01 = new uint[2] { 0x0, MATRIX_A };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Seed"></param>
        public void Init(uint Seed)
        {
          unchecked
          {
            mt[0] = Seed & 0xffffffff;
            for (mti = 1; mti < N; mti++)
            {
              mt[mti] = (1812433253 * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + mti);
              // See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier.
              // In the previous versions, MSBs of the seed affect
              // only MSBs of the array mt[].
              // 2002/01/09 modified by Makoto Matsumoto

              mt[mti] &= 0xffffffff;
              // for >32 bit machines
            }
          }
        }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Seed"></param>
        public void InitByArray(uint[] Seed)
        {
          unchecked
          {
            uint i, j, k;
            Init(19650218);

            i = 1;
            j = 0;

            k = (N > (uint)Seed.Length ? N : (uint)Seed.Length);
            for (; k > 0; k--)
            {
              mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1664525)) + Seed[j] + j; // non linear
              mt[i] &= 0xffffffff; // for WORDSIZE > 32 machines

              i++;
              j++;

              if (i >= N) { mt[0] = mt[N - 1]; i = 1; }
              if (j >= Seed.Length) j = 0;
            }

            for (k = N - 1; k > 0; k--)
            {
              mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1566083941)) - i; // non linear
              mt[i] &= 0xffffffff; // for WORDSIZE > 32 machines

              i++;
              if (i >= N) { mt[0] = mt[N - 1]; i = 1; }
            }

            mt[0] = 0x80000000; // MSB is 1; assuring non-zero initial array
          }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public uint GetNext()
        {
          unchecked
          {
            uint y;
           
            // mag01[x] = x * MATRIX_A  for x = 0,1

            if (mti >= N)
            { // generate N words at one time
              int kk;

              // if Init() has not been called,
              // a default initial seed is used
              if (mti == N + 1) Init(5489);

              for (kk = 0; kk < N - M; kk++)
              {
                y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                mt[kk] = mt[kk + M] ^ (y >> 1) ^ mag01[y & 0x1];
              }

              for (;kk < N - 1; kk++)
              {
                y = (mt[kk] & UPPER_MASK)|(mt[kk + 1] & LOWER_MASK);
                mt[kk] = mt[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 0x1];
              }

              y = (mt[N-1] & UPPER_MASK)|(mt[0] & LOWER_MASK);
              mt[N-1] = mt[M-1] ^ (y >> 1) ^ mag01[y & 0x1];

              mti = 0;
            }
         
            y = mt[mti++];

            // Tempering
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680;
            y ^= (y << 15) & 0xefc60000;
            y ^= (y >> 18);

            return y;
          }
        }

  }
}