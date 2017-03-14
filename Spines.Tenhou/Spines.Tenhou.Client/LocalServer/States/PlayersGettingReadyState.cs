// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Tenhou.Client.LocalServer.States
{
  internal class PlayersGettingReadyState : StateBase
  {
    public PlayersGettingReadyState(Match match)
    {
      _match = match;
    }

    public override IState Process(Message message)
    {
      // TODO if timer runs out, start match anyways
      RestartTimer();
      if (message.Content.Name == "GOK")
      {
        _match.ConfirmGo(message.SenderId);
        return this;
      }
      if (message.Content.Name == "NEXTREADY")
      {
        _match.ConfirmPlayerIsReady(message.SenderId);
        if (!_match.AreAllPlayersReadyForNextGame())
        {
          return this;
        }
        _match.StartNextGame();
        var timePerTurn = _match.TimePerTurn;
        var extraTime = _match.GetRemainingExtraTime();
        return new PlayerActiveState(_match, timePerTurn + extraTime);
      }
      return this;
    }

    private readonly Match _match;
  }
}