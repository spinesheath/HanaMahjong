// Spines.Tenhou.Client.ReceivedMessageEventArgs.cs
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
  /// Carries the message recieved by an XML client.
  /// </summary>
  public class ReceivedMessageEventArgs : EventArgs
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