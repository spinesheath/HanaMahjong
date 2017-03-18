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

    /// <summary>
    /// 0: draw with 0 in hand
    /// 1: draw with 1 in hand
    /// 2: discard with 1 in hand
    /// 3: discard with 2 in hand
    /// </summary>
    /// <param name="actionId">The id of the action used to change the value.</param>
    /// <returns></returns>
    public void MoveNext(int actionId)
    {
      switch (actionId)
      {
        case 0:
        case 2:
          Shanten += actionId - 1;
          break;
        case 1:
          Shanten -= _pairs == 0 ? 1 : 0;
          _pairs += 1;
          break;
        case 3:
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