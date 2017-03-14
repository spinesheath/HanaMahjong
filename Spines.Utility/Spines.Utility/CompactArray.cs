// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Spines.Utility
{
  /// <summary>
  /// An array of unsigned integral values with bit lengths that are not a power of two.
  /// </summary>
  public class CompactArray
  {
    /// <summary>
    /// Constructs a new instance of CompactArray initialized with the specified values.
    /// </summary>
    /// <param name="values">The values to initialize the CompactArray with.</param>
    public CompactArray(IEnumerable<int> values)
    {
      var raw = values.ToList();
      if (raw.Any(i => i < 0))
      {
        throw new ArgumentException("Values for compact array must not be negative.");
      }
      var max = raw.Max();
      _bitCount = GetBitCount(max);
      _mask = (1 << _bitCount) - 1;
      var totalBits = _bitCount * raw.Count;
      var totalIntegers = totalBits / 32 + ((totalBits & 31) > 0 ? 1 : 0);
      _data = new uint[totalIntegers];

      var offset = 0;
      for (var i = 0; i < raw.Count; ++i)
      {
        var s = offset / 32;
        var lshift = offset % 32;
        offset += _bitCount;
        var e = offset / 32;
        var v = (uint) raw[i];
        _data[s] |= v << lshift;
        if (s != e)
        {
          _data[e] |= v >> (32 - lshift);
        }
      }
    }

    /// <summary>
    /// Gets the value at the specified index.
    /// </summary>
    /// <param name="index">The index of the value.</param>
    /// <returns>The value at the given index.</returns>
    public int this[int index]
    {
      get
      {
        var offset = _bitCount * index;
        var s = offset / 32;
        var lshift = offset % 32;
        offset += _bitCount;
        var e = offset / 32;
        var p1 = (_data[s] & (_mask << lshift)) >> lshift;
        if (s != e)
        {
          var rshift = 32 - lshift;
          p1 |= (_data[e] & (_mask >> rshift)) << rshift;
        }
        return (int) p1;
      }
    }

    private readonly int _bitCount;
    private readonly uint[] _data;
    private readonly int _mask;

    private static int GetBitCount(int max)
    {
      if (max == int.MaxValue)
      {
        return 31;
      }
      return Enumerable.Range(0, 32).First(i => 1 << i > max);
    }
  }
}