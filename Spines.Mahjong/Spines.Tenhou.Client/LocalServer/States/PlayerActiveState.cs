// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Spines.Utility;

namespace Spines.Tenhou.Client.LocalServer.States
{
  internal class PlayerActiveState : StateBase
  {
    public PlayerActiveState(Match match, int milliseconds)
      : base(milliseconds)
    {
      _match = match;
    }

    public override IState Process(Message message)
    {
      if (message.Content.Name == "D")
      {
        var tile = new Tile(InvariantConvert.ToInt32(message.Content.Attribute("p").Value));
        if (!_match.IsActive(message.SenderId) || !_match.HasTileInClosedHand(message.SenderId, tile))
        {
          return this;
        }
        _match.SendDiscard(tile);
        if (_match.CanDraw())
        {
          _match.SendDraw();
        }
        else
        {
          _match.SendRyuukyoku();
        }
      }
      return this;
    }

    private readonly Match _match;
  }
}