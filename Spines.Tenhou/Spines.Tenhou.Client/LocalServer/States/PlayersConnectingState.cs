// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Tenhou.Client.LocalServer.States
{
  internal class PlayersConnectingState : StateBase
  {
    public PlayersConnectingState(Match match)
    {
      _match = match;
    }

    public override IState Process(Message message)
    {
      RestartTimer();
      if (message.Content.Name != "JOIN")
      {
        return this;
      }
      _match.ConfirmPlayerAsParticipant(message.SenderId);
      if (!_match.AreAllPlayersParticipating())
      {
        return this;
      }
      _match.SendGo();
      _match.SendTaikyoku();
      _match.SendUn();
      return new PlayersGettingReadyState(_match);
    }

    private readonly Match _match;
  }
}