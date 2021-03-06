﻿// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Spines.Utility;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// A dummy implementation of ITenhouTcpClient that doesn't connect to tenhou.net.
  /// </summary>
  internal class DummyTenhouConnection : ITenhouConnection
  {
    /// <summary>
    /// Creates a new instance of DummyTenhouTcpClient.
    /// </summary>
    /// <param name="logger">A logger.</param>
    public DummyTenhouConnection(ILogger logger)
    {
      _logger = logger;
    }

    /// <summary>
    /// Is raised in response to Process.
    /// </summary>
    public event TypedEventHandler<ITenhouConnection, ReceivedMessageEventArgs> ReceivedMessage;

    /// <summary>
    /// Is raised once the client successfully connected to the connection.
    /// </summary>
    public event EventHandler Connected;

    /// <summary>
    /// Emulates the tenhou.net connection.
    /// </summary>
    /// <param name="message">Used to determine the next fake message to receive.</param>
    public void Send(XElement message)
    {
      _logger.Trace("O: " + message);
      if (message.Name == "HELO")
      {
        FakeReceiveHelo();
      }
      else if (message.Name == "AUTH")
      {
        FakeReceiveLn();
      }
      else if (message.Name == "JOIN")
      {
        var t = message.Attributes().FirstOrDefault(a => a.Name == "t");
        if (t != null)
        {
          var parts = t.Value.Split(',');
          if (parts.Count() == 2)
          {
            FakeRecieveRejoin(parts);
          }
          else if (parts.Count() == 3)
          {
            FakeRecieveGo(parts);
          }
        }
      }
    }

    /// <summary>
    /// Raises the Connected event.
    /// </summary>
    public void Connect()
    {
      Validate.InvokeSafely(Connected, this);
    }

    private readonly ILogger _logger;

    private void FakeReceiveHelo()
    {
      var uname = new XAttribute("uname", "%71%77%64%66%65%72%67%68");
      var auth = new XAttribute("auth", "20141229-cc32e3fd");
      var expire = new XAttribute("expire", "20141230");
      var days = new XAttribute("expiredays", "2");
      var scale = new XAttribute("ratingscale",
        "PF3=1.000000&PF4=1.000000&PF01C=0.582222&PF02C=0.501632&PF03C=0.414869&PF11C=0.823386&PF12C=0.709416&PF13C=0.586714&PF23C=0.378722&PF33C=0.535594&PF1C00=8.000000");
      RaiseReceive(new XElement("HELO", uname, auth, expire, days, scale));
    }

    private void FakeReceiveLn()
    {
      var n = new XAttribute("n", "Buf1Bke1kV1Hd");
      var j = new XAttribute("j", "C1B3C3C1C4C8D3C1D4B9C3D12C4D8B1C1C1C2C3C2C1B11B");
      var g = new XAttribute("g", "DY1Q2s1Js1Y2Bk1DE4w8IU1I2k1EE4M8Fg4Bs4E8o4M8n1By2BR2z2J1BL1BI1e1h2b9G26E");
      RaiseReceive(new XElement("LN", n, j, g));
    }

    private void FakeRecieveGo(string[] parts)
    {
      var type = new XAttribute("type", parts[1]);
      var lobby = new XAttribute("lobby", parts[0]);
      var gpid = new XAttribute("gpid", "7167A1C7-5FA3ECC6");
      RaiseReceive(new XElement("GO", type, lobby, gpid));

      var n0 = new XAttribute("n0", "%73%70%69%6E%65%62%6F%74");
      var n1 = new XAttribute("n1", "%61%69%6B%6F%31%32");
      var n2 = new XAttribute("n2", "%4E%6F%4E%61%6D%65");
      var n3 = new XAttribute("n3", "%E3%81%88%E3%82%88%E3%81%BE%E3%81%AA");
      var dan = new XAttribute("dan", "0,1,0,10");
      var rate = new XAttribute("rate", "1500.00,1446.02,1500.00,1388.21");
      var sx = new XAttribute("sx", "M,F,M,M");
      RaiseReceive(new XElement("UN", n0, n1, n2, n3, dan, rate, sx));

      var oya = new XAttribute("oya", "1");
      var log = new XAttribute("log", "2012102722gm-0009-0000-9e067f8e");
      RaiseReceive(new XElement("TAIKYOKU", oya, log));
    }

    private void FakeRecieveRejoin(IEnumerable<string> parts)
    {
      var t = new XAttribute("t", string.Join(",", parts.Concat(new[] {"r"})));
      RaiseReceive(new XElement("REJOIN", t));
    }

    private void RaiseReceive(XElement message)
    {
      _logger.Trace("I: " + message);
      Validate.InvokeSafely(ReceivedMessage, this, new ReceivedMessageEventArgs(message));
    }
  }
}