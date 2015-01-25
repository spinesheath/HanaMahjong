// Spines.Tenhou.Client.ITenhouReceiver.cs
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
  /// Recieves and translates messages from the tenhou.net server.
  /// </summary>
  public interface ITenhouReceiver
  {
    /// <summary>
    /// Adds a listener for lobby messages.
    /// </summary>
    /// <param name="listener">The listener.</param>
    void AddLobbyListener(ILobbyClient listener);

    /// <summary>
    /// Adds a listener for match messages.
    /// </summary>
    /// <param name="listener">The listener.</param>
    void AddMatchListener(IMatchClient listener);
  }
}