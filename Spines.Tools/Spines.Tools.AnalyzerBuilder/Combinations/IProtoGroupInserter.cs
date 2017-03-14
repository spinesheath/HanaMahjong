// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Spines.Tools.AnalyzerBuilder.Combinations
{
  internal interface IProtoGroupInserter
  {
    bool CanInsert(IReadOnlyList<int> concealedTiles, IReadOnlyList<int> usedTiles, int offset);

    void Insert(IList<int> concealedTiles, IList<int> usedTiles, int offset);

    void Remove(IList<int> concealedTiles, IList<int> usedTiles, int offset);
  }
}