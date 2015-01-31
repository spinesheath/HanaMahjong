// Spines.Tenhou.Client.LocalConnection.cs
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

using System;
using System.Xml.Linq;
using Spines.Utility;

namespace Spines.Tenhou.Client.LocalServer
{
  internal class LocalConnection : ITenhouConnection
  {
    private readonly AccountInformation _accountInformation;
    private readonly LocalLobbyServer _server;

    public LocalConnection(LocalLobbyServer server, AccountInformation accountInformation)
    {
      _server = server;
      _accountInformation = accountInformation;
    }

    /// <summary>
    /// Sends a message to the server.
    /// </summary>
    /// <param name="message">The message to send.</param>
    public void Send(XElement message)
    {
      _server.Send(this, message);
    }

    public event EventHandler<ReceivedMessageEventArgs> ReceivedMessage;
    public event EventHandler<EventArgs> Connected;

    public void Connect()
    {
      _server.Send(this, new XElement("Z"));
      EventUtility.CheckAndRaise(Connected, this, new EventArgs());
    }

    /// <summary>
    /// User by the server to send messages.
    /// </summary>
    /// <param name="message">The message sent by the server.</param>
    public void Receive(XElement message)
    {
      EventUtility.CheckAndRaise(ReceivedMessage, this, new ReceivedMessageEventArgs(message));
    }
  }
}