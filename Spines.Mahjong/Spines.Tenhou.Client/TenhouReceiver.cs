// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Spines.Utility;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Recieves and translates messages from Tenhou.net.
  /// </summary>
  internal class TenhouReceiver : ITenhouReceiver
  {
    /// <summary>
    /// Adds a listener for lobby messages.
    /// </summary>
    /// <param name="listener">The listener.</param>
    public void AddLobbyListener(ILobbyClient listener)
    {
      _lobbyListeners.Add(listener);
    }

    /// <summary>
    /// Adds a listener for match messages.
    /// </summary>
    /// <param name="listener">The listener.</param>
    public void AddMatchListener(IMatchClient listener)
    {
      _matchListeners.Add(listener);
    }

    private readonly IList<ILobbyClient> _lobbyListeners = new List<ILobbyClient>();
    private readonly IList<IMatchClient> _matchListeners = new List<IMatchClient>();
    private readonly Dictionary<string, Action<XElement>> _messageActions = new Dictionary<string, Action<XElement>>();
    private readonly TenhouSender _sender;
    private readonly ITenhouConnection _connection;

    private void Broadcast(Action<IMatchClient> action)
    {
      Broadcast(_matchListeners, action);
    }

    private void Broadcast(Action<ILobbyClient> action)
    {
      Broadcast(_lobbyListeners, action);
    }

    private void InitializeMessageActions()
    {
      _messageActions.Add("HELO", OnLoggedOn);
      _messageActions.Add("REJOIN", OnReceivedRejoin);
      _messageActions.Add("GO", OnReceivedGo);
      _messageActions.Add("LN", m => { });
      _messageActions.Add("UN", OnUpdatePlayers);
      _messageActions.Add("TAIKYOKU", OnReceivedTaikyoku);
      _messageActions.Add("INIT", m => { });
      _messageActions.Add("REACH", m => { });
      _messageActions.Add("N", m => { });
      _messageActions.Add("DORA", m => { });
      _messageActions.Add("PROF", m => { });
      _messageActions.Add("RANKING", m => { });
      _messageActions.Add("CHAT", m => { });
      _messageActions.Add("AGARI", OnReceivedAgariOrRyuukyoku);
      _messageActions.Add("RYUUKYOKU", OnReceivedAgariOrRyuukyoku);
    }

    private void OnConnected(object sender, EventArgs e)
    {
      Broadcast(client => client.Connected());
    }

    private void OnLoggedOn(XElement message)
    {
      var authenticationString = message.Attribute("auth").Value;
      Broadcast(client => client.LoggedOn(new AccountInformation(message), authenticationString));
    }

    private void OnReceivedAgariOrRyuukyoku(XElement message)
    {
      if (message.Attributes().Any(a => a.Name == "owari"))
      {
        _connection.Send(new XElement("BYE"));
        _connection.Send(new XElement("BYE"));
        _sender.LogOn();
      }
      else
      {
        _connection.Send(new XElement("NEXTREADY"));
      }
    }

    private void OnReceivedGo(XElement message)
    {
      _connection.Send(new XElement("GOK"));
      _connection.Send(new XElement("NEXTREADY"));
      var matchInformation = new MatchInformation(message);
      Broadcast(client => client.Start(matchInformation));
      Broadcast(client => client.MatchStarted(matchInformation));
    }

    private void OnReceivedRejoin(XElement message)
    {
      var parts = message.Attribute("t").Value.Split(',');
      var lobby = InvariantConvert.ToInt32(parts[0]);
      var matchType = new MatchType(InvariantConvert.ToInt32(parts[1]));
      Broadcast(client => client.ProposeMatch(new MatchProposal(lobby, matchType)));
    }

    private void OnReceivedTaikyoku(XElement message)
    {
      var firstDealerIndex = InvariantConvert.ToInt32(message.Attribute("oya").Value);
      var logId = message.Attribute("log").Value;
      Broadcast(client => client.SetFirstDealer(firstDealerIndex));
      Broadcast(client => client.SetLogId(logId));
    }

    private void OnUpdatePlayers(XElement message)
    {
      Broadcast(client => client.UpdatePlayers(GetPlayers(message)));
    }

    private void ReceivedMessage(ITenhouConnection sender, ReceivedMessageEventArgs arguments)
    {
      var nodeName = arguments.Message.Name.LocalName;
      if (_messageActions.ContainsKey(nodeName))
      {
        _messageActions[nodeName](arguments.Message);
      }
      else if (IsPlayerDraw(nodeName))
      {
        Broadcast(client => client.DrawTile(CreateTileFromDrawOrDiscard(arguments.Message)));
      }
      else if (IsOpponentDraw(nodeName))
      {
        Broadcast(client => client.OpponentDrawTile(GetPlayerIndexFromDrawOrDiscard(arguments.Message)));
      }
      else if (IsDiscard(nodeName))
      {
        Broadcast(client => client.Discard(new DiscardInformation(arguments.Message)));
      }
    }

    private static void Broadcast<T>(IEnumerable<T> listeners, Action<T> action)
    {
      foreach (var listener in listeners)
      {
        action(listener);
      }
    }

    /// <summary>
    /// Creates a tile from the name of a draw or discard message.
    /// </summary>
    /// <param name="message">The draw or discard message.</param>
    /// <returns>The drawn or discarded tile.</returns>
    private static Tile CreateTileFromDrawOrDiscard(XElement message)
    {
      return new Tile(InvariantConvert.ToInt32(message.Name.LocalName.Substring(1)));
    }

    /// <summary>
    /// Extracts the player index from a draw or discard message.
    /// </summary>
    /// <param name="message">The draw or discard message.</param>
    /// <returns>The player index.</returns>
    private static int GetPlayerIndexFromDrawOrDiscard(XElement message)
    {
      return message.Name.LocalName[0] - 'T';
    }

    private static IEnumerable<PlayerInformation> GetPlayers(XElement message)
    {
      var playerCount = message.Attributes().Count(a => a.Name.LocalName[0] == 'n');
      return Enumerable.Range(0, playerCount).Select(i => new PlayerInformation(message, i));
    }

    private static bool IsDiscard(string nodeName)
    {
      return "DEFGdefg".Contains(nodeName[0]) && nodeName.Any(char.IsNumber);
    }

    private static bool IsOpponentDraw(string nodeName)
    {
      return nodeName == "U" || nodeName == "V" || nodeName == "W";
    }

    private static bool IsPlayerDraw(string nodeName)
    {
      return nodeName[0] == 'T' && nodeName.Any(char.IsNumber);
    }

    /// <summary>
    /// Creates a new Instance of TenhouReceiver using the specified ITenhouTcpClient.
    /// </summary>
    /// <param name="connection">The ITenhouTcpClient used to connect to the connection.</param>
    /// <param name="sender">Used to send messages to the connection.</param>
    /// <param name="lobbyClient">A connection for the tenhou lobby.</param>
    /// <param name="matchClient">A match connection.</param>
    internal TenhouReceiver(ITenhouConnection connection, TenhouSender sender, ILobbyClient lobbyClient,
      IMatchClient matchClient)
    {
      _connection = Validate.NotNull(connection, "connection");
      _sender = Validate.NotNull(sender, "sender");
      _lobbyListeners.Add(Validate.NotNull(lobbyClient, "lobbyClient"));
      _matchListeners.Add(Validate.NotNull(matchClient, "matchClient"));
      InitializeMessageActions();
      // No need to unsubscribe as the connection doen't live longer than the reveiver.
      _connection.ReceivedMessage += ReceivedMessage;
      _connection.Connected += OnConnected;
    }
  }
}