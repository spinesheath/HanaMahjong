// Spines.Tenhou.Client.MatchServer.cs
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
using System.Xml.Linq;

namespace Spines.Tenhou.Client.LocalServer
{
  internal class MatchServer
  {
    private readonly ISeedGenerator _seedGenerator;
    private readonly ISet<LobbyConnection> _queue = new HashSet<LobbyConnection>();
    private readonly IDictionary<LobbyConnection, Match> _playerToMatch = new Dictionary<LobbyConnection, Match>();

    public MatchServer(ISeedGenerator seedGenerator)
    {
      _seedGenerator = seedGenerator;
    }

    public void EnterQueue(LobbyConnection player, int lobby, MatchType matchType)
    {
      var nextFour = new List<LobbyConnection>();
      lock (_queue)
      {
        _queue.Add(player);
        if (_queue.Count == 4)
        {
          nextFour.AddRange(_queue);
          _queue.Clear();
        }
      }
      if (nextFour.Count == 4)
      {
        CreateNewMatch(nextFour);
      }
    }

    private void CreateNewMatch(List<LobbyConnection> players)
    {
      var seed = _seedGenerator.CreateSeed();
      var match = new Match(seed, players);
      lock (_playerToMatch)
      {
        foreach (var player in players)
        {
          _playerToMatch.Add(player, match);
        }
      }
    }

    public bool CanEnterQueue(LobbyConnection player, int lobby, MatchType matchType)
    {
      return !IsInQueue(player);
    }

    public bool IsInQueue(LobbyConnection player)
    {
      lock (_queue)
      {
        return _queue.Contains(player);
      }
    }

    public bool IsInMatch(LobbyConnection player, int lobby, MatchType matchType)
    {
      lock (_playerToMatch)
      {
        return _playerToMatch.ContainsKey(player);
      }
    }

    public void LeaveAll(LobbyConnection player)
    {
      throw new NotImplementedException();
    }

    public void ProcessMessage(LobbyConnection sender, XElement message)
    {
      GetMatch(sender).ProcessMessage(sender, message);
    }

    private Match GetMatch(LobbyConnection player)
    {
      lock (_playerToMatch)
      {
        return _playerToMatch[player];
      }
    }
  }
}