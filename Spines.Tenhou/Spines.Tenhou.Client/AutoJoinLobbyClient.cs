// Spines.Tenhou.Client.AutoJoinLobbyClient.cs
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
  /// Lobby client that automatically tries to join a match.
  /// </summary>
  internal class AutoJoinLobbyClient : ILobbyClient
  {
    private readonly TenhouSender _sender;
    private bool _joinedOnce;

    /// <summary>
    /// Creates a new instance of AutoJoinLobbyClient.
    /// </summary>
    /// <param name="sender">Used to send messages to the server.</param>
    internal AutoJoinLobbyClient(TenhouSender sender)
    {
      _sender = sender;
    }

    /// <summary>
    /// Called when the client is logged on.
    /// </summary>
    /// <param name="accountInformation">Information about the account.</param>
    /// <param name="authenticationValue"></param>
    public void LoggedOn(AccountInformation accountInformation, string authenticationValue)
    {
      _sender.Authenticate(authenticationValue);
      if(!_joinedOnce)
      {
        _sender.RequestMatch();
        _joinedOnce = true;
      }
    }

    /// <summary>
    /// Called when a match is started.
    /// </summary>
    /// <param name="matchInformation">Information about the match.</param>
    public void MatchStarted(MatchInformation matchInformation)
    {
      
    }

    /// <summary>
    /// Called when the connection to the server has been established.
    /// </summary>
    public void Connected()
    {
      _sender.LogOn();
    }
  }
}