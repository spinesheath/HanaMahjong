// Spines.Tenhou.Client.ClientFactory.cs
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

using Spines.Utility;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Creates clients for tenhou.net.
  /// </summary>
  public static class ClientFactory
  {
    /// <summary>
    /// Creates a client that is connected to a dummy server.
    /// </summary>
    public static ITenhouReceiver CreateDummyClient()
    {
      var logger = new ConsoleLogger();
      var server = new DummyTenhouTcpClient(logger);
      var logOnInformation = new LogOnInformation("ID0160262B-SG8PcR2h", "M", 0);
      var sender = new TenhouSender(server, logOnInformation);
      var ai = new TsumokiriAI(sender);
      var lobbyClient = new AutoJoinLobbyClient(sender);
      var receiver = new TenhouReceiver(server, sender, lobbyClient, ai);
      server.Connect();
      return receiver;
    }
  }
}