// Spines.Tenhou.Client.Match.cs
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
using Spines.Tenhou.Client.LocalServer.States;
using Spines.Utility;

namespace Spines.Tenhou.Client.LocalServer
{
  // TODO why do the initial messages joins take so long



  internal class Match
  {
    private const int CurrentNagareCount = 0;
    private const int CurrentHonbaCount = 0;
    private readonly int _firstOyaIndex;
    private readonly int _lobby;
    private readonly LogOnService _logOnService;
    private readonly MatchType _matchType;

    private readonly IDictionary<string, PlayerStatus> _players =
      new Dictionary<string, PlayerStatus>();

    private readonly WallGenerator _shuffler;
    private readonly StateMachine _stateMachine;
    private int _currentActivePlayerIndex;
    private int _currentGameIndex = 0;
    // roundNumber = seed[0] / 4
    // gameNumber = seed[0] % 4
    private Wall _currentWall;
    private int _timesOyaChanged = 0;

    public Match(string seed, IEnumerable<string> accountIds, int lobby, MatchType matchType, LogOnService logOnService)
    {
      _lobby = lobby;
      _matchType = matchType;
      _logOnService = logOnService;
      _shuffler = new WallGenerator(seed);
      _firstOyaIndex = _shuffler.GetWall(0).First().Id % 4;
      var playerIndex = 0;
      foreach (var player in accountIds)
      {
        _players.Add(player, new PlayerStatus(playerIndex));
        playerIndex += 1;
      }
      _stateMachine = new StateMachine(new PlayersConnectingState(this));
      _stateMachine.Finished += OnFinished;
    }

    public IEnumerable<string> Players
    {
      get { return _players.Keys; }
    }

    /// <summary>
    /// Milliseconds per turn.
    /// </summary>
    public int TimePerTurn
    {
      get { return 5000; }
    }

    public event EventHandler Finished;

    public bool AreAllPlayersParticipating()
    {
      return _players.All(p => p.Value.IsParticipating);
    }

    /// <summary>
    /// Checks if all players are ready for the next game.
    /// </summary>
    /// <returns>True if all players sent NEXTREADY after the GO message or the end of the last game.</returns>
    public bool AreAllPlayersReadyForNextGame()
    {
      return _players.All(p => p.Value.IsReadyForNextGame);
    }

    public bool CanDraw()
    {
      return _currentWall.HasNextDraw;
    }

    /// <summary>
    /// Confirms that a player sent a GOK message after the GO message.
    /// </summary>
    /// <param name="accountId">The player that sent the message.</param>
    public void ConfirmGo(string accountId)
    {
      if (_players.ContainsKey(accountId))
      {
        _players[accountId].AcceptedGo = true;
      }
    }

    /// <summary>
    /// Confirms that the player participates in the match.
    /// </summary>
    /// <param name="accountId">The participating player.</param>
    /// <param name="lobby">The lobby of the match.</param>
    /// <param name="matchType">The type of the match.</param>
    public void ConfirmPlayerAsParticipant(string accountId, int lobby, MatchType matchType)
    {
      if (_players.ContainsKey(accountId))
      {
        _players[accountId].IsParticipating = true;
      }
    }

    public void ConfirmPlayerIsReady(string accountId)
    {
      if (_players.ContainsKey(accountId))
      {
        _players[accountId].IsReadyForNextGame = true;
      }
    }

    /// <summary>
    /// Returns the remaining extra time for the active player.
    /// </summary>
    /// <returns>The remaining extra time for the active player</returns>
    public int GetRemainingExtraTime()
    {
      return 0;
    }

    public bool HasTileInClosedHand(string accountId, Tile tile)
    {
      return _players[accountId].HasTileInClosedHand(tile);
    }

    public void InvitePlayers()
    {
      var t = new XAttribute("t", InvariantConvert.Format("{0},{1},r", _lobby, _matchType.TypeId));
      SendToAll(new XElement("REJOIN", t));
    }

    public bool IsActive(string accountId)
    {
      return _players[accountId].PlayerIndex == _currentActivePlayerIndex;
    }

    public void ProcessMessage(Message message)
    {
      _stateMachine.Process(message);
    }

    public void SendDiscard(Tile tile)
    {
      // TODO discard from hand or tsumokiri
      const string characters = "defg";
      foreach (var player in _players)
      {
        var characterIndex = (4 + _currentActivePlayerIndex - player.Value.PlayerIndex) % 4;
        _logOnService.Send(player.Key, new XElement(characters.Substring(characterIndex, 1) + tile.Id));
      }

      _currentActivePlayerIndex = (_currentActivePlayerIndex + 1) % 4;
    }

    public void SendDraw()
    {
      var tile = _currentWall.PopDraw();
      const string characters = "TUVW";
      foreach (var player in _players)
      {
        if (player.Value.PlayerIndex == _currentActivePlayerIndex)
        {
          player.Value.AddTile(tile);
          _logOnService.Send(player.Key, new XElement("T" + tile.Id));
        }
        else
        {
          var characterIndex = (4 + _currentActivePlayerIndex - player.Value.PlayerIndex) % 4;
          _logOnService.Send(player.Key, new XElement(characters.Substring(characterIndex, 1)));
        }
      }
    }

    public void SendGo()
    {
      var type = new XAttribute("type", "9");
      var lobby = new XAttribute("lobby", "0");
      var gpid = new XAttribute("gpid", "7167A1C7-5FA3ECC6");
      SendToAll(new XElement("GO", type, lobby, gpid));
    }

    public void SendRyuukyoku()
    {
      var ba = new XAttribute("ba", "0,2");
      var sc = new XAttribute("sc", "250,0,250,0,250,0,250,0");
      var owari = new XAttribute("owari", "526,63.0,-55,-56.0,23,-38.0,506,31.0");
      SendToAll(new XElement("RYUUKYOKU", ba, sc, owari));
    }

    // TODO Send Owari

    public void SendTaikyoku()
    {
      // TODO how is oya calculated? In replays it's always 0 at the start, in live matches not
      var oya = new XAttribute("oya", 0);
      var log = new XAttribute("log", "2012102722gm-0009-0000-9e067f8e");
      SendToAll(new XElement("TAIKYOKU", oya, log));
    }

    public void SendUn()
    {
      var dan = new XAttribute("dan", "12,12,12,10");
      var rate = new XAttribute("rate", "1704.57,1675.00,1701.91,1618.53");
      var sx = new XAttribute("sx", "M,M,M,M");
      var n0 = new XAttribute("n0", "player0");
      var n1 = new XAttribute("n1", "player1");
      var n2 = new XAttribute("n2", "player2");
      var n3 = new XAttribute("n3", "player3");
      SendToAll(new XElement("UN", n0, n1, n2, n3, dan, rate, sx));
    }

    public void StartNextGame()
    {
      var dice = _shuffler.GetDice(_currentGameIndex).ToList();
      _currentWall = new Wall(_shuffler.GetWall(_currentGameIndex));
      _currentActivePlayerIndex = _firstOyaIndex;
      var firstDora = _currentWall.GetDora(0);
      var seedValues = new[] {_timesOyaChanged, CurrentHonbaCount, CurrentNagareCount, dice[0], dice[1], firstDora.Id};
      var seed = new XAttribute("seed", string.Join(",", seedValues));
      var points = Enumerable.Repeat(250, 4);
      var ten = new XAttribute("ten", string.Join(",", points));
      var oyaIndex = (_firstOyaIndex + _timesOyaChanged) % 4;
      var oya = new XAttribute("oya", oyaIndex);
      foreach (var player in _players)
      {
        var tiles = _currentWall.GetStartingHand(oyaIndex, player.Value.PlayerIndex);
        var hai = new XAttribute("hai", string.Join(",", tiles));
        var message = new XElement("INIT", seed, ten, oya, hai);

        _logOnService.Send(player.Key, message);
      }
      SendDraw();
    }

    private void OnFinished(object sender, EventArgs e)
    {
      var stateMachine = (StateMachine) sender;
      stateMachine.Finished -= OnFinished;
      EventUtility.CheckAndRaise(Finished, this);
    }

    private void SendToAll(XElement message)
    {
      foreach (var player in _players)
      {
        _logOnService.Send(player.Key, message);
      }
    }
  }
}