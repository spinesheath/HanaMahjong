﻿// Spines.Mahjong.Analysis.IProtoGroupInserter.cs
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
  internal interface IProtoGroupInserter
  {
    bool CanInsert(IReadOnlyList<int> concealedTiles, IReadOnlyList<int> usedTiles, int offset);

    void Insert(IList<int> concealedTiles, IList<int> usedTiles, int offset);

    void Remove(IList<int> concealedTiles, IList<int> usedTiles, int offset);
  }
}