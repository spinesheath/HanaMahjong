// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Utility
{
  /// <summary>
  /// An array of bits with methods to access single or multiple bits.
  /// </summary>
  public class BitField
  {
    /// <summary>
    /// Instantiates a new instance of BitField.
    /// </summary>
    /// <param name="bits">The source bits.</param>
    public BitField(int bits)
    {
      _bits = bits;
    }

    /// <summary>
    /// Checks if the meld code has the specified bit, starting from the least significant bit at index 0.
    /// </summary>
    public bool HasBit(int bitIndex)
    {
      return (_bits & 1 << bitIndex) != 0;
    }

    /// <summary>
    /// Apply a bitmask with all ones in a single block and then treat the selected bits as an integer.
    /// For example, with letters as arbitrary bits: _code = abcdefgh => ExtractSegment(3, 2) == 00000def.
    /// </summary>
    /// <param name="numberOfBits">The number of bits that will be selected.</param>
    /// <param name="leftShift">How many of the least significant bits to ignore.</param>
    /// <returns>The selected bits from the original value.</returns>
    public int ExtractSegment(int numberOfBits, int leftShift)
    {
      var mask = (1 << numberOfBits) - 1;
      return (_bits >> leftShift) & mask;
    }

    /// <summary>
    /// The underlying bits.
    /// </summary>
    private readonly int _bits;
  }
}