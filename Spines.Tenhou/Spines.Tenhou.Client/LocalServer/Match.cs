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
    private readonly int _lobby;
    private readonly MatchType _matchType;
    private readonly IDictionary<LobbyConnection, bool> _players = new Dictionary<LobbyConnection, bool>();
    private readonly StateMachine<LobbyConnection, Match> _stateMachine;
    private WallGenerator _shuffler;

    public Match(string seed, IEnumerable<LobbyConnection> players, int lobby, MatchType matchType)
    {
      _lobby = lobby;
      _matchType = matchType;
      _shuffler = new WallGenerator(seed);
      foreach (var player in players)
      {
        _players.Add(player, false);
      }
      _stateMachine = new StateMachine<LobbyConnection, Match>(this, new PlayersConnectingState());
      _stateMachine.Finished += OnFinished;
    }

    public IEnumerable<LobbyConnection> Players
    {
      get { return _players.Keys; }
    }

    public event EventHandler Finished;

    public bool AllPlayersConfirmed()
    {
      return _players.All(p => p.Value);
    }

    public void ConfirmPlayer(LobbyConnection sender, int lobby, MatchType matchType)
    {
      if (_players.ContainsKey(sender))
      {
        _players[sender] = true;
      }
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

    private void OnFinished(object sender, EventArgs e)
    {
      var stateMachine = (StateMachine<LobbyConnection, Match>) sender;
      stateMachine.Finished -= OnFinished;
      EventUtility.CheckAndRaise(Finished, this);
    }

    //private void SendInit(int gameIndex)
    //{
    //  // roundNumber = seed[0] / 4
    //  // gameNumber = seed[0] % 4
    //  // timesTheOyaChanged = seed[0]
    //  // honba = seed[1]
    //  // nagare/riichi = seed[2]
    //  const int timesTheOyaChanged = 0;
    //  var seedValues = new[] { timesTheOyaChanged, 0, 0, _matchState.GetDice(0), _matchState.GetDice(1), _matchState.GetDora(0) };
    //  var seed = new XAttribute("seed", string.Join(",", seedValues));
    //  var points = Enumerable.Repeat(250, 4);
    //  var ten = new XAttribute("ten", string.Join(",", points));
    //  var oya = new XAttribute("oya", timesTheOyaChanged % 4);
    //  var connectedPlayers = GetConnectedPlayers().ToList();
    //  for (var i = 0; i < connectedPlayers.Count; ++i)
    //  {
    //    var tiles = _matchState.GetCurrentHand(i);
    //    var hai = new XAttribute("ten", string.Join(",", tiles));
    //    var process = new XElement("INIT", seed, ten, oya, hai);
    //    connectedPlayers[i].Key.Receive(process);
    //  }
    //}

    //internal class MatchState
    //{
    //  private readonly WallGenerator _shuffler;
    //  private readonly IList<PlayerState> _players;

    //  public MatchState(string seed, IEnumerable<PlayerState> playerStates)
    //  {
    //    _shuffler = new WallGenerator(seed);
    //    CurrentGameIndex = 0;
    //    CurrentOyaIndex = 0;
    //    _players = playerStates.ToList();
    //    LoadStartingHands();
    //  }

    //  private void LoadStartingHands()
    //  {
    //    for (var i = 0; i < 4; ++i)
    //    {
    //      _players[i].CurrentHand = GetStartingHand(i);
    //    }
    //  }

    //  public int CurrentGameIndex { get; private set; }

    //  public int FirstOyaIndex { get { return 0; } }

    //  public int CurrentOyaIndex { get; private set; }

    //  public PlayerState ActivePlayer { get; private set; }

    //  public int GetDice(int diceIndex)
    //  {
    //    Validate.InRange(diceIndex, 0, 1, "diceIndex");
    //    return _shuffler.GetDice(CurrentGameIndex).ToList()[diceIndex];
    //  }

    //  public int GetDora(int doraIndex)
    //  {
    //    Validate.InRange(doraIndex, 0, 4, "doraIndex");
    //    return _shuffler.GetWall(CurrentGameIndex).Skip(5 + 2 * doraIndex).First();
    //  }

    //  public IEnumerable<int> GetCurrentHand(int playerIndex)
    //  {
    //    Validate.InRange(playerIndex, 0, 3, "playerIndex");
    //    return _players[playerIndex].CurrentHand;
    //  }

    //  private IEnumerable<int> GetStartingHand(int playerIndex)
    //  {
    //    var wall = _shuffler.GetWall(CurrentGameIndex).ToList();
    //    var drawOrderIndex = (4 + playerIndex - CurrentOyaIndex) % 4;
    //    yield return wall[135 - drawOrderIndex * 4];
    //    yield return wall[134 - drawOrderIndex * 4];
    //    yield return wall[133 - drawOrderIndex * 4];
    //    yield return wall[132 - drawOrderIndex * 4];
    //    yield return wall[135 - 16 - drawOrderIndex * 4];
    //    yield return wall[134 - 16 - drawOrderIndex * 4];
    //    yield return wall[133 - 16 - drawOrderIndex * 4];
    //    yield return wall[132 - 16 - drawOrderIndex * 4];
    //    yield return wall[135 - 32 - drawOrderIndex * 4];
    //    yield return wall[134 - 32 - drawOrderIndex * 4];
    //    yield return wall[133 - 32 - drawOrderIndex * 4];
    //    yield return wall[132 - 32 - drawOrderIndex * 4];
    //    yield return wall[135 - 48 - drawOrderIndex * 4];
    //  }
    //}
  }
}