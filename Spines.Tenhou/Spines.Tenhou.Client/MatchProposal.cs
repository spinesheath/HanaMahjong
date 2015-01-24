// Spines.Tenhou.Client.MatchProposal.cs
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
  /// A proposal of a match.
  /// </summary>
  public class MatchProposal
  {
    internal MatchProposal(int lobby, MatchType matchType)
    {
      Lobby = lobby;
      MatchType = matchType;
    }

    /// <summary>
    /// The lobby the match is played in.
    /// </summary>
    public int Lobby { get; private set; }

    /// <summary>
    /// The type of the match.
    /// </summary>
    public MatchType MatchType { get; private set; }
  }
}