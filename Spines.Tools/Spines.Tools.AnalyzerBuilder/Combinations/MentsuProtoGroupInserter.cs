// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Spines.Tools.AnalyzerBuilder.Combinations
{
  internal class MentsuProtoGroupInserter : IProtoGroupInserter
  {
    public MentsuProtoGroupInserter(int required, int occupied)
    {
      _required = required;
      _occupied = occupied;
    }

    public bool CanInsert(IReadOnlyList<int> concealedTiles, IReadOnlyList<int> usedTiles, int offset)
    {
      return (concealedTiles[offset] == _required || concealedTiles[offset] > _occupied) &&
             usedTiles[offset] <= 4 - _occupied;
    }

    public void Insert(IList<int> concealedTiles, IList<int> usedTiles, int offset)
    {
      concealedTiles[offset] -= _required;
      usedTiles[offset] += _occupied;
    }

    public void Remove(IList<int> concealedTiles, IList<int> usedTiles, int offset)
    {
      concealedTiles[offset] += _required;
      usedTiles[offset] -= _occupied;
    }

    private readonly int _occupied;
    private readonly int _required;
  }
}