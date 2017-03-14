// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml.Linq;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Carries the message recieved by an XML client.
  /// </summary>
  internal class ReceivedMessageEventArgs : EventArgs
  {
    /// <summary>
    /// Initializes the message.
    /// </summary>
    /// <param name="message">The message that was received.</param>
    public ReceivedMessageEventArgs(XElement message)
    {
      Message = message;
    }

    /// <summary>
    /// The message that was received.
    /// </summary>
    public XElement Message { get; private set; }
  }
}