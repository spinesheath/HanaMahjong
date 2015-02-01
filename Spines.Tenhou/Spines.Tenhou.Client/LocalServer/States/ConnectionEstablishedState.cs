// Spines.Tenhou.Client.ConnectionEstablishedState.cs
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

using System.Xml.Linq;
using Spines.Tenhou.Client.LocalServer.Transitions;

namespace Spines.Tenhou.Client.LocalServer.States
{
  /// <summary>
  /// Connection to the server has been established, waiting for login.
  /// </summary>
  internal class ConnectionEstablishedState : LimitedTimeState<LocalConnection, LobbyConnection>
  {
    public override IStateTransition<LocalConnection, LobbyConnection> Process(LocalConnection sender, XElement message)
    {
      ResetTimer();
      if (message.Name == "HELO")
      {
        return new LogOnTransition(message.Attribute("name").Value);
      }
      return new DoNothingTransition<LocalConnection, LobbyConnection>(this);
    }

    protected override IStateTransition<LocalConnection, LobbyConnection> CreateTimeOutTransition()
    {
      return new CloseConnectionTransition();
    }
  }
}