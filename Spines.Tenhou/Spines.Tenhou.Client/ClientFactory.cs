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

using System.Collections.Generic;
using Spines.Tenhou.Client.LocalServer;
using Spines.Utility;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Creates clients for tenhou.net.
  /// </summary>
  public static class ClientFactory
  {
    /// <summary>
    /// Creates a client that is connected to a dummy connection.
    /// </summary>
    public static ITenhouReceiver CreateDummyClient()
    {
      var logger = new ConsoleLogger();
      var connection = new DummyTenhouConnection(logger);
      var logOnInformation = new LogOnInformation("ID0160262B-SG8PcR2h", "M", 0);
      var sender = new TenhouSender(connection, logOnInformation);
      var ai = new TsumokiriAI(sender);
      var lobbyClient = new AutoJoinLobbyClient(sender);
      var receiver = new TenhouReceiver(connection, sender, lobbyClient, ai);
      connection.Connect();
      return receiver;
    }

    /// <summary>
    /// Creates a local match.
    /// </summary>
    public static IEnumerable<ITenhouReceiver> CreateLocalMatch()
    {
      var server = new LocalLobbyServer(new SeedGenerator());
      for (var i = 0; i < 4; ++i)
      {
        var connection = new LocalConnection(server);
        var sender = new TenhouSender(connection, new LogOnInformation("AccountId" + i, "M", 0));
        var client = new AutoJoinLobbyClient(sender);
        var ai = new TsumokiriAI(sender);
        var receiver = new TenhouReceiver(connection, sender, client, ai);
        connection.Connect();
        yield return receiver;
      }
    }
  }
}