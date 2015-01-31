// Spines.Tenhou.Client.ILobbyClient.cs
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
  /// A client for the tenhou lobby.
  /// </summary>
  public interface ILobbyClient
  {
    /// <summary>
    /// Called when the client is logged on.
    /// </summary>
    /// <param name="accountInformation">Information about the account.</param>
    /// <param name="authenticationString"></param>
    void LoggedOn(AccountInformation accountInformation, string authenticationString);

    /// <summary>
    /// Called when a match is started.
    /// </summary>
    /// <param name="matchInformation">Information about the match.</param>
    void MatchStarted(MatchInformation matchInformation);

    /// <summary>
    /// Called when the connection to the server has been established.
    /// </summary>
    void Connected();
  }
}