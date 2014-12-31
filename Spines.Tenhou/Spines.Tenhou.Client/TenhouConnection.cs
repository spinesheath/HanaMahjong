// Spines.Tenhou.Client.TenhouClient.cs
// 
// Copyright (C) 2014  Johannes Heckl
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
  /// Connection to Tenhou.net.
  /// </summary>
  public class TenhouConnection : IDisposable
  {
    private const int Port = 10080;
    private TcpClient _client;
    private BackgroundWorker _reciever;
    private readonly IPAddress _address = IPAddress.Parse("133.242.10.78");

    /// <summary>
    /// Disposes the Connection.
    /// </summary>
    /// <param name="disposing">If false, only native Resources are cleaned up. If true, native and managed Resources are cleaned up.</param>
    protected virtual void Dispose(bool disposing)
    {
      if(disposing)
      {
        Close();
      }
    }

    /// <summary>
    /// Disposes the Connection.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Opens the Connection.
    /// </summary>
    public void Open()
    {
      _client = new TcpClient();
      _client.Connect(_address, Port);
      if (!_client.Connected)
      {
        Console.WriteLine("Failed to connect to IP: " + _address + " on port: " + Port);
        return;
      }
      Console.WriteLine("Connected to IP: " + _address + " on port: " + Port);
      _reciever = new BackgroundWorker {WorkerSupportsCancellation = true, WorkerReportsProgress = true};
      _reciever.DoWork += RecieveMessages;
      _reciever.RunWorkerAsync();
      SendKeepAlivePing();
      Thread.Sleep(2000);
      ContactServer("ID0160262B%2DSG8PcR2h", "M", "0000");
    }

    private void SendKeepAlivePing()
    {
      SendMessage("<Z />");
    }

    private void RecieveMessages(object sender, DoWorkEventArgs e)
    {
      var worker = sender as BackgroundWorker;
      if (worker == null)
      {
        throw new ClientException("BackgroundWorker was null.");
      }
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
        var parts = new string(Encoding.ASCII.GetChars(buffer)).Replace("&", "&amp;").Split(new[] {'\0'}, StringSplitOptions.RemoveEmptyEntries);
        Console.WriteLine("Recieved message: " + Environment.NewLine + string.Join(Environment.NewLine, parts));
        var xElements = parts.Select(XElement.Parse);
        foreach (var xElement in xElements)
        {
          if (xElement.Name == "HELO")
          {
            var auth = xElement.Attributes().FirstOrDefault(a => a.Name == "auth");
            if (auth != null)
            {
              Authenticate(auth.Value);
            }
          }
        }
      }
    }

    private void SendMessage(string message)
    {
      var stream = _client.GetStream();
      var data = Encoding.ASCII.GetBytes(message).Concat(new byte[] {0}).ToArray();
      stream.Write(data, 0, data.Length);
      Console.WriteLine("Sent message: " + message);
    }

    /// <summary>
    /// Closes the connection.
    /// </summary>
    public void Close()
    {
      if (_client != null)
      {
        _client.Close();
        _client = null;
      }
      if (_reciever != null)
      {
        _reciever.CancelAsync();
        _reciever.Dispose();
        _reciever = null;
      }
    }

    private void ContactServer(string accountId, string sex, string lobby)
    {
      var idAttribute = new XAttribute("name", accountId);
      var lobbyAttribute = new XAttribute("tid", lobby);
      var sexAttribute = new XAttribute("sx", sex);
      var xElement = new XElement("HELO", idAttribute, lobbyAttribute, sexAttribute);
      SendMessage(xElement.ToString());
    }

    private void Authenticate(string authenticationString)
    {
      var transformed = Authenticator.Transform(authenticationString);
      var valAttribute = new XAttribute("val", transformed);
      var xElement = new XElement("AUTH", valAttribute);
      SendMessage(xElement.ToString());
    }
  }
}