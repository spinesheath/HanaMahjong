// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    public LocalConnection(LocalLobbyServer server)
    {
      _server = server;
    }

    public event TypedEventHandler<ITenhouConnection, ReceivedMessageEventArgs> ReceivedMessage;
    public event EventHandler Connected;

    /// <summary>
    /// Sends a message to the server.
    /// </summary>
    /// <param name="message">The message to send.</param>
    public void Send(XElement message)
    {
      Console.WriteLine("me: " + InvariantConvert.ToString(GetHashCode()) + " " + message.Name);
      _server.Process(this, message);
    }

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

    private readonly Queue<XElement> _messageQueue = new Queue<XElement>();
    private readonly LocalLobbyServer _server;
    private CancellationTokenSource _receiverCancellationTokenSource;
    private Task _receiverTask;

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