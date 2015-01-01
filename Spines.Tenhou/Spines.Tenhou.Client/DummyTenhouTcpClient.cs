// Spines.Tenhou.Client.DummyTenhouTcpClient.cs
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
  /// A dummy implementation of ITenhouTcpClient that doesn't connect to tenhou.net.
  /// </summary>
  public class DummyTenhouTcpClient : ITenhouTcpClient
  {
    /// <summary>
    /// Does nothing.
    /// </summary>
    /// <param name="message">Is ignored.</param>
    public void Send(XElement message)
    {
    }

#pragma warning disable 0067
    /// <summary>
    /// Is never raised.
    /// </summary>
    public event EventHandler<ReceivedMessageEventArgs> Receive;
#pragma warning restore 0067
  }
}