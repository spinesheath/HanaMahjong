// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Mahjong.Analysis.Classification
{
  internal struct ChiitoiClassifier
  {
    public static ChiitoiClassifier Create()
    {
      return new ChiitoiClassifier(14, 0);
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
          Shanten -= _usedSlots < 7 ? 1 : 0;
          _usedSlots += 1;
          break;
        case 1:
          Shanten -= 1;
          break;
      }
    }

    public void Discard(int previousTileCount)
    {
      switch (previousTileCount)
      {
        case 1:
          Shanten += _usedSlots > 7 ? 0 : 1;
          _usedSlots -= 1;
          break;
        case 2:
          Shanten += 1;
          break;
      }
    }

    private int _usedSlots;

    private ChiitoiClassifier(int shanten, int usedSlots)
    {
      Shanten = shanten;
      _usedSlots = usedSlots;
    }
  }
}