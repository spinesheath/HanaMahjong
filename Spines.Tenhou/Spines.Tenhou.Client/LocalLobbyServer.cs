// Spines.Tenhou.Client.LocalLobbyServer.cs
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
using System.Xml.Linq;

namespace Spines.Tenhou.Client
{
  internal class LocalLobbyServer
  {
    private readonly IDictionary<LocalMahjongConnection, AccountInformation> _connections =
      new Dictionary<LocalMahjongConnection, AccountInformation>();

    private readonly IDictionary<LocalMahjongConnection, LocalMatchServer> _playersInMatches =
      new Dictionary<LocalMahjongConnection, LocalMatchServer>();

    /// <summary>
    /// The connections that are currently trying to join a match.
    /// </summary>
    private readonly ISet<LocalMahjongConnection> _matchQueue = new HashSet<LocalMahjongConnection>();

    private readonly IDictionary<LocalMatchServer, IEnumerable<LocalMahjongConnection>> _matchServers =
      new Dictionary<LocalMatchServer, IEnumerable<LocalMahjongConnection>>();

    private readonly ISeedGenerator _seedGenerator;

    public LocalLobbyServer(ISeedGenerator seedGenerator)
    {
      _seedGenerator = seedGenerator;
    }

    public void Connect(LocalMahjongConnection connection, AccountInformation accountInformation)
    {
      lock (_connections)
      {
        if (!_connections.ContainsKey(connection))
        {
          _connections.Add(connection, accountInformation);
        }
      }
    }

    public void Send(LocalMahjongConnection connection, XElement message)
    {
      AccountInformation accountInformation;
      lock (_connections)
      {
        if (!_connections.ContainsKey(connection))
        {
          return;
        }
        accountInformation = _connections[connection];
      }
      ProcessMessage(connection, accountInformation, message);
    }

    private static void LogInUser(LocalMahjongConnection connection, AccountInformation accountInformation)
    {
      var uname = new XAttribute("uname", accountInformation.UserName.EncodedName);
      var auth = new XAttribute("auth", "20141229-cc32e3fd");
      var expire = new XAttribute("expire", "20141230");
      var days = new XAttribute("expiredays", "2");
      var scale = new XAttribute("ratingscale", "PF3=1.000000&PF4=1.000000&PF01C=0.582222&PF02C=0.501632&" +
                                                "PF03C=0.414869&PF11C=0.823386&PF12C=0.709416&PF13C=0.586714&" +
                                                "PF23C=0.378722&PF33C=0.535594&PF1C00=8.000000");
      connection.Receive(new XElement("HELO", uname, auth, expire, days, scale));
    }

    private IEnumerable<LocalMahjongConnection> AddPlayerAndTake4(LocalMahjongConnection connection)
    {
      lock (_playersInMatches)
      {
        if (_playersInMatches.ContainsKey(connection))
        {
          return Enumerable.Empty<LocalMahjongConnection>();
        }
      }
      lock (_matchQueue)
      {
        _matchQueue.Add(connection);
        if (_matchQueue.Count <= 3)
        {
          return Enumerable.Empty<LocalMahjongConnection>();
        }
        var players = _matchQueue.Take(4).ToList();
        foreach (var localMahjongConnection in players)
        {
          _matchQueue.Remove(localMahjongConnection);
        }
        return players;
      }
    }

    private void OnMatchFinished(object sender, EventArgs e)
    {
      var localMatchServer = (LocalMatchServer) sender;
      localMatchServer.Finished -= OnMatchFinished;
      var players = Enumerable.Empty<LocalMahjongConnection>();
      lock (_matchServers)
      {
        if (_matchServers.ContainsKey(localMatchServer))
        {
          players = _matchServers[localMatchServer];
          _matchServers.Remove(localMatchServer);
        }
      }
      lock (_playersInMatches)
      {
        foreach (var localMahjongConnection in players)
        {
          _playersInMatches.Remove(localMahjongConnection);
        }
      }
    }

    private void ProcessMessage(LocalMahjongConnection connection, AccountInformation account, XElement message)
    {
      if (message.Name == "HELO")
      {
        LogInUser(connection, account);
      }
      else if (message.Name == "JOIN")
      {
        var t = message.Attribute("t").Value.Split(new[] {','});
        if (t.Length == 2)
        {
          ProposeMatch(connection);
        }
        else if (t.Length == 3)
        {
          AcceptMatch(connection);
        }
      }
    }

    private void AcceptMatch(LocalMahjongConnection connection)
    {
      lock (_playersInMatches)
      {
        if (_playersInMatches.ContainsKey(connection))
        {
          _playersInMatches[connection].AcceptMatch(connection);
        }
      }
    }

    private void ProposeMatch(LocalMahjongConnection connection)
    {
      var players = AddPlayerAndTake4(connection).ToList();
      if (players.Count < 4)
      {
        return;
      }
      var seed = _seedGenerator.CreateSeed();
      var localMatchServer = new LocalMatchServer(seed);
      lock (_playersInMatches)
      {
        foreach (var localMahjongConnection in players)
        {
          _playersInMatches.Add(localMahjongConnection, localMatchServer);
        }
      }
      localMatchServer.Finished += OnMatchFinished;
      localMatchServer.Start();
      lock (_matchServers)
      {
        _matchServers.Add(localMatchServer, players);
      }
      foreach (var localMahjongConnection in players)
      {
        localMahjongConnection.Receive(new XElement("REJOIN", new XAttribute("t", "0,9,r")));
      }
    }
  }
}