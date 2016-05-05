// Spines.Mahjong.Analysis.ShuntsuInsertChecker.cs
// 
// Copyright (C) 2016  Johannes Heckl
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

using System.Collections.Generic;

namespace Spines.Mahjong.Analysis.Combinations
{
  internal class ShuntsuInsertChecker : IInsertChecker
  {
    private readonly int _requiredLeft;
    private readonly int _requiredMiddle;
    private readonly int _requiredRight;

    public ShuntsuInsertChecker(int requiredLeft, int requiredMiddle, int requiredRight)
    {
      _requiredLeft = requiredLeft;
      _requiredMiddle = requiredMiddle;
      _requiredRight = requiredRight;
    }

    public bool CanInsert(IReadOnlyList<int> concealedTiles, IReadOnlyList<int> usedTiles, int offset)
    {
      if (offset > 6)
      {
        return false;
      }
      if (usedTiles[offset + 0] == 4 || 
          usedTiles[offset + 1] == 4 || 
          usedTiles[offset + 2] == 4)
      {
        return false;
      }
      return concealedTiles[offset + 0] >= _requiredLeft || 
             concealedTiles[offset + 1] >= _requiredMiddle ||
             concealedTiles[offset + 2] >= _requiredRight;
    }
  }
}