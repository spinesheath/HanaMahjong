// Spines.Tenhou.Client.MatchType.cs
// 
// Copyright (C) 2015  Johannes Heckl
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

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// The type of a match.
  /// </summary>
  public class MatchType
  {
    internal MatchType(int typeId)
    {
      AkaAri = typeId == 9;
      TypeId = typeId;
    }

    /// <summary>
    /// The id of the match type as used by tenhou.net.
    /// </summary>
    internal int TypeId { get; set; }

    /// <summary>
    /// True if the match is played with 3 red fives.
    /// </summary>
    public bool AkaAri { get; private set; }
  }
}