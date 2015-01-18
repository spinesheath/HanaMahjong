// Spines.Tenhou.Client.LogOnInformation.cs
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
  /// Information necessary to log onto tenhou.net.
  /// </summary>
  public class LogOnInformation
  {
    /// <summary>
    /// Initializes a new instance of LogOnInformation.
    /// </summary>
    /// <param name="tenhouId">The Id of the Tenhou Account.</param>
    /// <param name="gender">The gender of the Tenhou Account.</param>
    /// <param name="lobby">The lobby to connect to.</param>
    public LogOnInformation(string tenhouId, string gender, int lobby)
    {
      TenhouId = tenhouId;
      Gender = gender;
      Lobby = lobby;
    }

    /// <summary>
    /// The Id of the Tenhou Account.
    /// </summary>
    public string TenhouId { get; private set; }

    /// <summary>
    /// The gender of the Tenhou Account.
    /// </summary>
    public string Gender { get; private set; }

    /// <summary>
    /// The lobby to connect to.
    /// </summary>
    public int Lobby { get; private set; }
  }
}