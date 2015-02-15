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

namespace Spines.Tenhou.Client.LocalServer
{
  internal class MatchServer
  {
    private readonly IDictionary<string, Match> _accountIdToMatch = new Dictionary<string, Match>();
    private readonly ISet<string> _queue = new HashSet<string>();
    private readonly ISeedGenerator _seedGenerator;
    private readonly LogOnService _logOnService;

    public MatchServer(ISeedGenerator seedGenerator, LogOnService logOnService)
    {
      _seedGenerator = seedGenerator;
      _logOnService = logOnService;
    }

    public bool CanEnterQueue(string accountId, int lobby, MatchType matchType)
    {
      return !IsInQueue(accountId);
    }

    public void EnterQueue(string accountId, int lobby, MatchType matchType)
    {
      Console.WriteLine(accountId + " entered the match queue.");
      var nextFour = new List<string>();
      lock (_queue)
      {
        _queue.Add(accountId);
        if (_queue.Count == 4)
        {
          nextFour.AddRange(_queue);
          _queue.Clear();
        }
      }
      if (nextFour.Count == 4)
      {
        CreateNewMatch(nextFour, lobby, matchType);
      }
    }

    public bool IsInMatch(string accountId, int lobby, MatchType matchType)
    {
      lock (_accountIdToMatch)
      {
        return _accountIdToMatch.ContainsKey(accountId);
      }
    }

    public bool IsInQueue(string accountId)
    {
      lock (_queue)
      {
        return _queue.Contains(accountId);
      }
    }

    public void LeaveAll(string accountId)
    {
      throw new NotImplementedException();
    }

    public void ProcessMessage(Message message)
    {
      GetMatch(message.SenderId).ProcessMessage(message);
    }

    private void CreateNewMatch(List<string> accountIds, int lobby, MatchType matchType)
    {
      var seed = _seedGenerator.CreateSeed();
      var match = new Match(seed, accountIds, lobby, matchType, _logOnService);
      match.Finished += OnMatchFinished;
      lock (_accountIdToMatch)
      {
        foreach (var player in accountIds)
        {
          _accountIdToMatch.Add(player, match);
        }
      }
      match.InvitePlayers();
    }

    private Match GetMatch(string accountId)
    {
      lock (_accountIdToMatch)
      {
        return _accountIdToMatch[accountId];
      }
    }

    private void OnMatchFinished(object sender, EventArgs e)
    {
      var match = (Match) sender;
      match.Finished -= OnMatchFinished;
      lock (_accountIdToMatch)
      {
        foreach (var player in match.Players)
        {
          _accountIdToMatch.Remove(player);
        }
      }
    }
  }
}