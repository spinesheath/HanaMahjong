// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// Progressively calculates the Shanten for Kokushi.
  /// </summary>
  internal struct KokushiClassifier
  {
    public static KokushiClassifier Create()
    {
      return new KokushiClassifier(14, 0);
    }

    /// <summary>
    /// Shanten + 1 because in Hand calculations are done with that value instead of real shanten.
    /// </summary>
    public int Shanten;

    public void Draw(int previousTileCount)
    {
      switch (previousTileCount)
      {
        case 0:
          Shanten += -1;
          break;
        case 1:
          Shanten -= _pairs == 0 ? 1 : 0;
          _pairs += 1;
          break;
      }
    }

    public void Discard(int previousTileCount)
    {
      switch (previousTileCount)
      {
        case 1:
          Shanten += 1;
          break;
        case 2:
          _pairs -= 1;
          Shanten += _pairs == 0 ? 1 : 0;
          break;
      }
    }

    private int _pairs;

    private KokushiClassifier(int shanten, int pairs)
    {
      Shanten = shanten;
      _pairs = pairs;
    }
  }
}