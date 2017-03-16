// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

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