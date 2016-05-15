// Spines.Mahjong.Analysis.StateComparer.cs
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
using System.Linq;
using Spines.Utility;

namespace Spines.Mahjong.Analysis.Classification
{
  internal class StateComparer : IEqualityComparer<State>
  {
    public bool Equals(State first, State second)
    {
      if (first == second)
      {
        return true;
      }
      if (first == null || second == null)
      {
        return false;
      }
      return first.TargetStates.SequenceEqual(second.TargetStates);
    }

    public int GetHashCode(State state)
    {
      Validate.NotNull(state, nameof(state));
      var hc = 1;
      for (var i = 0; i < state.AlphabetSize; ++i)
      {
        hc = unchecked(hc * 17 + (state.TargetStates[i] == null ? 0 : state.TargetStates[i].GetHashCode()));
      }
      return hc;
    }
  }
}