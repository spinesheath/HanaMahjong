// Spines.Tenhou.Client.InMatchState.cs
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

namespace Spines.Tenhou.Client.LocalServer.States
{
  internal class InMatchState : StateBase
  {
    // TODO use LogOnService in LobbyStates too
    private readonly string _accountId;
    private readonly LobbyConnection _connection;

    public InMatchState(LobbyConnection connection, string accountId)
    {
      _connection = connection;
      _accountId = accountId;
    }

    public override IState Process(Message message)
    {
      RestartTimer();
      if (message.Content.Name == "BYE")
      {
        return new IdleState(_connection, _accountId);
      }
      _connection.MatchServer.ProcessMessage(new Message(_accountId, message.Content));
      return this;
    }
  }
}