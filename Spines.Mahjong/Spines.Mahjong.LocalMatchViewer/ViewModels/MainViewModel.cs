// Spines.Mahjong.LocalMatchViewer.MainViewModel.cs
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
using System.Collections.ObjectModel;
using System.Threading;
using Spines.Tenhou.Client;

namespace Spines.Mahjong.LocalMatchViewer.ViewModels
{
  internal class MainViewModel : ViewModelBase, ILobbyClient, IMatchClient
  {
    public MainViewModel()
    {
      Messages = new ObservableCollection<string>();
      var receivers = ClientFactory.CreateLocalMatch();
      foreach (var receiver in receivers)
      {
        receiver.AddLobbyListener(this);
        receiver.AddMatchListener(this);
      }
    }

    private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

    public ObservableCollection<string> Messages { get; private set; }

    /// <summary>
    /// Called when the client is logged on.
    /// </summary>
    /// <param name="accountInformation">Information about the account.</param>
    /// <param name="authenticationValue">The value to authenticate.</param>
    public void LoggedOn(AccountInformation accountInformation, string authenticationValue)
    {
      AddMessageOnGuiThread("Logged on");
    }

    /// <summary>
    /// Called when a match is started.
    /// </summary>
    /// <param name="matchInformation">Information about the match.</param>
    public void MatchStarted(MatchInformation matchInformation)
    {
      AddMessageOnGuiThread("Match started");
    }

    /// <summary>
    /// Called when the connection to the server has been established.
    /// </summary>
    public void Connected()
    {
      AddMessageOnGuiThread("Connected");
    }

    /// <summary>
    /// Asks the client whether to join a match.
    /// </summary>
    /// <param name="proposal">The proposed match.</param>
    public void ProposeMatch(MatchProposal proposal)
    {
      AddMessageOnGuiThread("Match proposed");
    }

    /// <summary>
    /// Called when a match starts.
    /// </summary>
    /// <param name="matchInformation">Information about the match.</param>
    public void Start(MatchInformation matchInformation)
    {
      AddMessageOnGuiThread("Match started");
    }

    /// <summary>
    /// Provides the client with information about the players in a match.
    /// </summary>
    /// <param name="players">Information about the players.</param>
    public void UpdatePlayers(IEnumerable<PlayerInformation> players)
    {
      AddMessageOnGuiThread("Players updated");
    }

    /// <summary>
    /// Tells the client which player is the first dealer.
    /// </summary>
    /// <param name="firstDealerIndex">The playerIndex of the first dealer.</param>
    public void SetFirstDealer(int firstDealerIndex)
    {
      AddMessageOnGuiThread("First dealer set");
    }

    /// <summary>
    /// Tells the client the id of the match log.
    /// </summary>
    /// <param name="logId">The id of the match log.</param>
    public void SetLogId(string logId)
    {
      AddMessageOnGuiThread("Log ID set");
    }

    /// <summary>
    /// Tells the client that the active player drew a tile.
    /// </summary>
    /// <param name="tile">The tile that was drawn.</param>
    public void DrawTile(Tile tile)
    {
      AddMessageOnGuiThread("Tile drawn");
    }

    /// <summary>
    /// Tells the cilent that an opponent drew a tile.
    /// </summary>
    /// <param name="playerIndex">The player index of the opponent.</param>
    public void OpponentDrawTile(int playerIndex)
    {
      AddMessageOnGuiThread("Opponent drew tile");
    }

    /// <summary>
    /// Tells the client that a tile was discarded.
    /// </summary>
    /// <param name="discardInformation">Information about the discard.</param>
    public void Discard(DiscardInformation discardInformation)
    {
      AddMessageOnGuiThread("Discard");
    }

    private void AddMessage(object state)
    {
      Messages.Add((string)state);
    }

    private void AddMessageOnGuiThread(string message)
    {
      _synchronizationContext.Send(AddMessage, message);
    }
  }
}