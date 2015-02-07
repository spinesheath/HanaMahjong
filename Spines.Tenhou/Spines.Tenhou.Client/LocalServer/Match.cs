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
  internal class Match
  {
    private const int CurrentNagareCount = 0;
    private const int CurrentHonbaCount = 0;
    private readonly int _firstOyaIndex;
    private readonly int _lobby;
    private readonly MatchType _matchType;

    private readonly IDictionary<LobbyConnection, PlayerStatus> _players =
      new Dictionary<LobbyConnection, PlayerStatus>();

    private readonly WallGenerator _shuffler;
    private readonly StateMachine<LobbyConnection, Match> _stateMachine;
    private int _currentGameIndex = 0;
    // roundNumber = seed[0] / 4
    // gameNumber = seed[0] % 4
    private Wall _currentWall;
    private int _timesOyaChanged = 0;

    public Match(string seed, IEnumerable<LobbyConnection> players, int lobby, MatchType matchType)
    {
      _lobby = lobby;
      _matchType = matchType;
      _shuffler = new WallGenerator(seed);
      _firstOyaIndex = _shuffler.GetDice(0).First() % 4;
      var playerIndex = 0;
      foreach (var player in players)
      {
        _players.Add(player, new PlayerStatus(playerIndex));
        playerIndex += 1;
      }
      _stateMachine = new StateMachine<LobbyConnection, Match>(this, new PlayersConnectingState());
      _stateMachine.Finished += OnFinished;
    }

    public IEnumerable<LobbyConnection> Players
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

    /// <summary>
    /// Confirms that a player sent a GOK message after the GO message.
    /// </summary>
    /// <param name="player">The player that sent the message.</param>
    public void ConfirmGo(LobbyConnection player)
    {
      if (_players.ContainsKey(player))
      {
        _players[player].AcceptedGo = true;
      }
    }

    /// <summary>
    /// Confirms that the player participates in the match.
    /// </summary>
    /// <param name="player">The participating player.</param>
    /// <param name="lobby">The lobby of the match.</param>
    /// <param name="matchType">The type of the match.</param>
    public void ConfirmPlayerAsParticipant(LobbyConnection player, int lobby, MatchType matchType)
    {
      if (_players.ContainsKey(player))
      {
        _players[player].IsParticipating = true;
      }
    }

    public void ConfirmPlayerIsReady(LobbyConnection player)
    {
      if (_players.ContainsKey(player))
      {
        _players[player].IsReadyForNextGame = true;
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

    public void InvitePlayers()
    {
      var t = new XAttribute("t", InvariantConvert.Format("{0},{1},r", _lobby, _matchType.TypeId));
      SendToAll(new XElement("REJOIN", t));
    }

    public void ProcessMessage(LobbyConnection sender, XElement message)
    {
      _stateMachine.ProcessMessage(sender, message);
    }

    public void SendToAll(XElement message)
    {
      foreach (var player in _players.Keys)
      {
        player.SendToClient(message);
      }
    }

    public void StartNextGame()
    {
      var dice = _shuffler.GetDice(_currentGameIndex).ToList();
      _currentWall = new Wall(_shuffler.GetWall(_currentGameIndex));
      var firstDora = _currentWall.GetDora(0);
      var seedValues = new[] {_timesOyaChanged, CurrentHonbaCount, CurrentNagareCount, dice[0], dice[1], firstDora};
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
        player.Key.SendToClient(message);
      }
      SendDraw(oyaIndex);
    }

    private void OnFinished(object sender, EventArgs e)
    {
      var stateMachine = (StateMachine<LobbyConnection, Match>) sender;
      stateMachine.Finished -= OnFinished;
      EventUtility.CheckAndRaise(Finished, this);
    }

    private void SendDraw(int playerIndex)
    {
      var tile = _currentWall.PopDraw();
      const string characters = "TUVW";
      foreach (var player in _players)
      {
        if (player.Value.PlayerIndex == playerIndex)
        {
          player.Key.SendToClient(new XElement("T" + tile));
        }
        else
        {
          var characterIndex = (4 + playerIndex - player.Value.PlayerIndex) % 4;
          player.Key.SendToClient(new XElement(characters.Substring(characterIndex, 1)));
        }
      }
    }
  }
}