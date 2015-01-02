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
using Spines.Utility;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// A dummy implementation of ITenhouTcpClient that doesn't connect to tenhou.net.
  /// </summary>
  public class DummyTenhouTcpClient : ITenhouTcpClient
  {
    private readonly ILogger _logger;

    /// <summary>
    /// Creates a new instance of DummyTenhouTcpClient.
    /// </summary>
    /// <param name="logger">A logger.</param>
    public DummyTenhouTcpClient(ILogger logger)
    {
      _logger = logger;
    }

    /// <summary>
    /// Emulates the tenhou.net server.
    /// </summary>
    /// <param name="message">Used to determine the next fake message to receive.</param>
    public void Send(XElement message)
    {
      message.ThrowIfNull("message");
      _logger.Trace("Sending: " + message);
      if (message.Name == "HELO")
      {
        ReceiveHelo();
      }
      else if (message.Name == "AUTH")
      {
        ReceiveLn();
      }
    }

    /// <summary>
    /// Is raised in response to Send.
    /// </summary>
    public event EventHandler<ReceivedMessageEventArgs> Receive;

    private void RaiseReceive(XElement message)
    {
      _logger.Trace("Receiving: " + message);
      if (null != Receive)
      {
        Receive(this, new ReceivedMessageEventArgs(message));
      }
    }

    private void ReceiveHelo()
    {
      var uname = new XAttribute("uname", "%71%77%64%66%65%72%67%68");
      var auth = new XAttribute("auth", "20141229-cc32e3fd");
      var expire = new XAttribute("expire", "20141230");
      var days = new XAttribute("expiredays", "2");
      var scale = new XAttribute("ratingscale",
        "PF3=1.000000&PF4=1.000000&PF01C=0.582222&PF02C=0.501632&PF03C=0.414869&PF11C=0.823386&PF12C=0.709416&PF13C=0.586714&PF23C=0.378722&PF33C=0.535594&PF1C00=8.000000");
      RaiseReceive(new XElement("HELO", uname, auth, expire, days, scale));
    }

    private void ReceiveLn()
    {
      var n = new XAttribute("n", "Buf1Bke1kV1Hd");
      var j = new XAttribute("j", "C1B3C3C1C4C8D3C1D4B9C3D12C4D8B1C1C1C2C3C2C1B11B");
      var g = new XAttribute("g", "DY1Q2s1Js1Y2Bk1DE4w8IU1I2k1EE4M8Fg4Bs4E8o4M8n1By2BR2z2J1BL1BI1e1h2b9G26E");
      RaiseReceive(new XElement("LN", n, j, g));
    }
  }
}