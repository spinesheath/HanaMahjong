// Spines.Tenhou.Client.PlayersConnectingState.cs
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
using Spines.Utility;

namespace Spines.Tenhou.Client.LocalServer.States
{
  internal class PlayersConnectingState : LimitedTimeState<LobbyConnection, Match>
  {
    public override IStateTransition<LobbyConnection, Match> Process(XElement message)
    {
      Validate.NotNull(message, "message");
      ResetTimer();
      if (message.Name == "JOIN")
      {
        return new PlayerConnectedTransition(message, this);
      }
      return new DoNothingTransition<LobbyConnection, Match>(this);
    }

    protected override IStateTransition<LobbyConnection, Match> CreateTimeOutTransition()
    {
      return new DoNothingTransition<LobbyConnection, Match>(new FinalState<LobbyConnection, Match>());
    }
  }
}