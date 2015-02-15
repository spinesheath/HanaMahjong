// Spines.Tenhou.Client.PlayerActiveState.cs
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

using Spines.Utility;

namespace Spines.Tenhou.Client.LocalServer.States
{
  internal class PlayerActiveState : StateBase
  {
    private readonly Match _match;

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
  }
}