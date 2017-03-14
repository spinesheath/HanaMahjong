// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Spines.Tenhou.Client.LocalServer
{
  internal class MatchServer
  {
    public MatchServer(ISeedGenerator seedGenerator, LogOnService logOnService)
    {
      _seedGenerator = seedGenerator;
      _logOnService = logOnService;
    }

    public bool CanEnterQueue(string accountId)
    {
      return !IsInQueue(accountId);
    }

    public void EnterQueue(string accountId, int lobby, MatchType matchType)
    {
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

    public bool IsInMatch(string accountId)
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

    public void ProcessMessage(Message message)
    {
      GetMatch(message.SenderId).ProcessMessage(message);
    }

    private readonly IDictionary<string, Match> _accountIdToMatch = new Dictionary<string, Match>();
    private readonly ISet<string> _queue = new HashSet<string>();
    private readonly ISeedGenerator _seedGenerator;
    private readonly LogOnService _logOnService;

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