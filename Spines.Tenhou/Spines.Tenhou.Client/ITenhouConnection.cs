// Spines.Tenhou.Client.ITenhouTcpClient.cs
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

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// A connection to a server following the tenhou.net protocol.
  /// </summary>
  internal interface ITenhouConnection
  {
    /// <summary>
    /// Sends a message to the server.
    /// </summary>
    /// <param name="message">The message to send.</param>
    void Send(XElement message);

    /// <summary>
    /// Is raised every time a message from the server is received.
    /// </summary>
    event EventHandler<ReceivedMessageEventArgs> ReceivedMessage;

    /// <summary>
    /// Is raised once the connection successfully connected to the server.
    /// </summary>
    event EventHandler Connected;
  }
}