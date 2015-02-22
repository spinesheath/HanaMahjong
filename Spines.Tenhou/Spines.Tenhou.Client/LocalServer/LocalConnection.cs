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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Spines.Utility;

namespace Spines.Tenhou.Client.LocalServer
{
  internal class LocalConnection : ITenhouConnection
  {
    private readonly Queue<XElement> _messageQueue = new Queue<XElement>();
    private readonly LocalLobbyServer _server;
    private CancellationTokenSource _receiverCancellationTokenSource;
    private Task _receiverTask;

    public LocalConnection(LocalLobbyServer server)
    {
      _server = server;
    }

    /// <summary>
    /// Sends a message to the server.
    /// </summary>
    /// <param name="message">The message to send.</param>
    public void Send(XElement message)
    {
      Console.WriteLine("me: " + InvariantConvert.ToString(GetHashCode()) + " " + message.Name);
      _server.Process(this, message);
    }

    public event TypedEventHandler<ITenhouConnection, ReceivedMessageEventArgs> ReceivedMessage;
    public event EventHandler Connected;

    public void Connect()
    {
      RecieveMessagesAsync();
      SendKeepAlivePing();
      Validate.InvokeSafely(Connected, this);
    }

    /// <summary>
    /// Used by the server to send messages.
    /// </summary>
    /// <param name="message">The message sent by the server.</param>
    public void Receive(XElement message)
    {
      Console.WriteLine("amanda: " + InvariantConvert.ToString(GetHashCode()) + " " + message.Name);
      lock (_messageQueue)
      {
        _messageQueue.Enqueue(message);
      }
    }

    private IEnumerable<XElement> GetMessages()
    {
      lock (_messageQueue)
      {
        var messages = _messageQueue.ToList();
        _messageQueue.Clear();
        return messages;
      }
    }

    private void RecieveMessages(CancellationToken cancellationToken)
    {
      var sleepCounter = 0;
      while (!cancellationToken.IsCancellationRequested)
      {
        if (sleepCounter > 40)
        {
          SendKeepAlivePing();
          sleepCounter = 0;
        }
        foreach (var message in GetMessages())
        {
          Validate.InvokeSafely(ReceivedMessage, this, new ReceivedMessageEventArgs(message));
        }
        Thread.Sleep(100);
        sleepCounter += 1;
      }
      SendBye();
    }

    private async void RecieveMessagesAsync()
    {
      _receiverCancellationTokenSource = new CancellationTokenSource();
      var token = _receiverCancellationTokenSource.Token;
      _receiverTask = Task.Run(() => RecieveMessages(token), token);
      await _receiverTask;
    }

    private void SendBye()
    {
      Send(new XElement("BYE"));
    }

    private void SendKeepAlivePing()
    {
      Send(new XElement("Z"));
    }
  }
}