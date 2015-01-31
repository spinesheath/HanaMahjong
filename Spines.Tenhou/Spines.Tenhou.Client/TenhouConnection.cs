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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Spines.Utility;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// A TcpClient connected to tenhou.net.
  /// </summary>
  internal class TenhouConnection : ITenhouConnection, IDisposable
  {
    private const int Port = 10080;
    private readonly IPAddress _address = IPAddress.Parse("133.242.10.78");
    private readonly ILogger _logger;
    private TcpClient _client;
    private CancellationTokenSource _receiverCancellationTokenSource;
    private Task _receiverTask;

    /// <summary>
    /// Creates a new instance of TenhouTcpClient.
    /// </summary>
    /// <param name="logger">A logger.</param>
    public TenhouConnection(ILogger logger)
    {
      _logger = logger;
    }

    /// <summary>
    /// Disposes this.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Is raised every time a message from the connection is received.
    /// </summary>
    public event EventHandler<ReceivedMessageEventArgs> ReceivedMessage;

    /// <summary>
    /// Is raised once the client successfully connected to the connection.
    /// </summary>
    public event EventHandler Connected;

    /// <summary>
    /// Sends a message to the connection.
    /// </summary>
    /// <param name="message">The message to send.</param>
    public void Send(XElement message)
    {
      Validate.NotNull(message, "message");
      Send(message.ToString());
    }

    /// <summary>
    /// Closes the connection to tenhou.net.
    /// </summary>
    public void Close()
    {
      try
      {
        if (_receiverCancellationTokenSource != null && _receiverTask != null)
        {
          _receiverCancellationTokenSource.Cancel();
          _receiverTask.Wait(1000);
          _receiverCancellationTokenSource.Dispose();
          _receiverTask.Dispose();
        }
        if (_client != null)
        {
          _client.Close();
        }
      }
      finally
      {
        _receiverCancellationTokenSource = null;
        _receiverTask = null;
        _client = null;
      }
    }

    /// <summary>
    /// Connects to tenhou.net.
    /// </summary>
    public void Connect()
    {
      _client = new TcpClient();
      _client.Connect(_address, Port);
      EventUtility.CheckAndRaise(Connected, this);
      var stream = _client.GetStream();
      stream.ReadTimeout = 1000;
      RecieveMessagesAsync(stream);
      SendKeepAlivePing();
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

    private void ReadMessage(NetworkStream stream)
    {
      var buffer = new byte[1024];
      stream.Read(buffer, 0, buffer.Length);
      var parts = new string(Encoding.ASCII.GetChars(buffer)).Replace("&", "&amp;")
        .Split(new[] {'\0'}, StringSplitOptions.RemoveEmptyEntries);
      _logger.Trace(string.Join(Environment.NewLine, parts.Select(p => "I: " + p)));
      var xElements = parts.Select(XElement.Parse);
      foreach (var xElement in xElements)
      {
        EventUtility.CheckAndRaise(ReceivedMessage, this, new ReceivedMessageEventArgs(xElement));
      }
    }

    private void RecieveMessages(NetworkStream stream, CancellationToken cancellationToken)
    {
      var sleepCounter = 0;
      while (!cancellationToken.IsCancellationRequested)
      {
        if (sleepCounter > 40)
        {
          SendKeepAlivePing();
          sleepCounter = 0;
        }
        if (!stream.DataAvailable)
        {
          Thread.Sleep(100);
          sleepCounter += 1;
          continue;
        }
        ReadMessage(stream);
      }
      SendBye();
    }

    private async void RecieveMessagesAsync(NetworkStream stream)
    {
      _receiverCancellationTokenSource = new CancellationTokenSource();
      var token = _receiverCancellationTokenSource.Token;
      _receiverTask = Task.Run(() => RecieveMessages(stream, token), token);
      await _receiverTask;
    }

    private void Send(string message)
    {
      var stream = _client.GetStream();
      var data = Encoding.ASCII.GetBytes(message).Concat(new byte[] {0}).ToArray();
      stream.Write(data, 0, data.Length);
      _logger.Trace("O: " + message);
    }

    private void SendBye()
    {
      Send("<BYE />");
    }

    private void SendKeepAlivePing()
    {
      Send("<Z />");
    }
  }
}