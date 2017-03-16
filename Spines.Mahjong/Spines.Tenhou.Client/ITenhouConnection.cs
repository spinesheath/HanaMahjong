// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml.Linq;
using Spines.Utility;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// A connection to a server following the tenhou.net protocol.
  /// </summary>
  internal interface ITenhouConnection
  {
    /// <summary>
    /// Is raised every time a message from the server is received.
    /// </summary>
    event TypedEventHandler<ITenhouConnection, ReceivedMessageEventArgs> ReceivedMessage;

    /// <summary>
    /// Is raised once the connection successfully connected to the server.
    /// </summary>
    event EventHandler Connected;

    /// <summary>
    /// Sends a message to the server.
    /// </summary>
    /// <param name="message">The message to send.</param>
    void Send(XElement message);
  }
}