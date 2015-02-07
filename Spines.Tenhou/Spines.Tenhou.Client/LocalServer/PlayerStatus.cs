// Spines.Tenhou.Client.PlayerStatus.cs
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

namespace Spines.Tenhou.Client.LocalServer
{
  internal class PlayerStatus
  {
    public PlayerStatus(int playerIndex)
    {
      PlayerIndex = playerIndex;
    }

    /// <summary>
    /// The index of the player in the match.
    /// </summary>
    public int PlayerIndex { get; private set; }

    /// <summary>
    /// Whether the player is participating in the match.
    /// </summary>
    public bool IsParticipating { get; set; }

    /// <summary>
    /// Whether the player confirmed the GO message.
    /// </summary>
    public bool AcceptedGo { get; set; }

    /// <summary>
    /// Whether the player sent the NEXTREADY message after the GO message or the most recent game.
    /// </summary>
    public bool IsReadyForNextGame { get; set; }
  }
}