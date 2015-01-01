// Spines.Tenhou.Client.TenhouTcpClient.cs
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
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// A TcpClient connected to tenhou.net.
  /// </summary>
  public class TenhouTcpClient : ITenhouTcpClient, IDisposable
  {
    private const int Port = 10080;
    private readonly IPAddress _address = IPAddress.Parse("133.242.10.78");
    private TcpClient _client;
    private BackgroundWorker _receiver;

    /// <summary>
    /// Disposes this.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Sends a message to the server.
    /// </summary>
    /// <param name="message">The message to send.</param>
    public void Send(XElement message)
    {
      message.ThrowIfNull("message");
      Send(message.ToString());
    }

    /// <summary>
    /// Is raised every time a message from the server is received.
    /// </summary>
    public event EventHandler<ReceivedMessageEventArgs> Receive;

    /// <summary>
    /// Connects to tenhou.net.
    /// </summary>
    public void Connect()
    {
      _client = new TcpClient();
      _client.Connect(_address, Port);
      _receiver = new BackgroundWorker {WorkerSupportsCancellation = true};
      _receiver.DoWork += RecieveMessages;
      _receiver.RunWorkerAsync();
      SendKeepAlivePing();
    }

    private void SendKeepAlivePing()
    {
      Send("<Z />");
    }

    private void Send(string message)
    {
      var stream = _client.GetStream();
      var data = Encoding.ASCII.GetBytes(message).Concat(new byte[] {0}).ToArray();
      stream.Write(data, 0, data.Length);
      Console.WriteLine("Sent message: " + message);
    }

    /// <summary>
    /// Closes the connection to tenhou.net.
    /// </summary>
    public void Close()
    {
      if (_client != null)
      {
        _client.Close();
        _client = null;
      }
      if (_receiver != null)
      {
        _receiver.CancelAsync();
        _receiver.Dispose();
        _receiver = null;
      }
    }

    /// <summary>
    /// Disposes this.
    /// </summary>
    /// <param name="disposing">If true, managed resources are cleaned up.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        Close();
      }
    }

    private void RecieveMessages(object sender, DoWorkEventArgs e)
    {
      var worker = (BackgroundWorker) sender;
      var stream = _client.GetStream();
      stream.ReadTimeout = 1000;
      while (!worker.CancellationPending)
      {
        if (!stream.DataAvailable)
        {
          Thread.Sleep(100);
          continue;
        }
        var buffer = new byte[1024];
        stream.Read(buffer, 0, buffer.Length);
        var parts = new string(Encoding.ASCII.GetChars(buffer)).Replace("&", "&amp;")
          .Split(new[] {'\0'}, StringSplitOptions.RemoveEmptyEntries);
        Console.WriteLine("Received message: " + Environment.NewLine + string.Join(Environment.NewLine, parts));
        var xElements = parts.Select(XElement.Parse);
        foreach (var xElement in xElements)
        {
          RaiseRecieve(xElement);
        }
      }
    }

    private void RaiseRecieve(XElement xElement)
    {
      if (Receive != null)
      {
        Receive(this, new ReceivedMessageEventArgs(xElement));
      }
    }
  }
}