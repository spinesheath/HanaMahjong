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
    private TcpClient _client;
    private BackgroundWorker _reciever;

    /// <summary>
    /// Disposes the Connection.
    /// </summary>
    public void Dispose()
    {
      Close();
    }

    /// <summary>
    /// Opens the Connection.
    /// </summary>
    public void Open()
    {
      const string server = "c.mjv.jp";
      const int port = 10080;
      var ip = IPAddress.Parse("133.242.10.78");
      _client = new TcpClient();
      _client.Connect(ip, port);
      _reciever = new BackgroundWorker {WorkerSupportsCancellation = true, WorkerReportsProgress = true};
      _reciever.DoWork += RecieveMessages;
      _reciever.RunWorkerAsync();
      Console.WriteLine("Connected to: " + server + " on port: " + port);
      SendMessage(_client, "<Z />");
      Thread.Sleep(2000);
      SendMessage(_client, "<HELO name=\"ID0160262B%2DSG8PcR2h\" tid=\"0000\" sx=\"M\" />");
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
          continue;
        }
        var buffer = new byte[1024];
        stream.Read(buffer, 0, buffer.Length);
        Console.WriteLine("Recieved message: " + new string(Encoding.ASCII.GetChars(buffer)).TrimEnd(new[]{'\0'}));
      }
    }

    private static void SendMessage(TcpClient client, string message)
    {
      var stream = client.GetStream();
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
      }
      if (_reciever != null)
      {
        _reciever.CancelAsync();
      }
    }

    private void ContactServer(string accountId, string sex, string lobby)
    {
      var idAttribute = new XAttribute("name", accountId);
      var lobbyAttribute = new XAttribute("tid", lobby);
      var sexAttribute = new XAttribute("sx", sex);
      var x = new XElement("HELO", idAttribute, lobbyAttribute, sexAttribute);
    }

    private void Authenticate(string authenticationString)
    {
      var authenticator = new Authenticator();
      var transformed = authenticator.Transform(authenticationString);
      var a = new XAttribute("val", transformed);
      var x = new XElement("AUTH", a);
    }
  }
}