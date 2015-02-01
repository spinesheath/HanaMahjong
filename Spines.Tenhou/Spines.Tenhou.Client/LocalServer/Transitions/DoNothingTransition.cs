﻿// Spines.Tenhou.Client.DoNothingTransition.cs
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

using Spines.Tenhou.Client.LocalServer.States;

namespace Spines.Tenhou.Client.LocalServer.Transitions
{
  internal class DoNothingTransition<TSender, THost> : IStateTransition<TSender, THost>
  {
    private readonly IState<TSender, THost> _nextState;

    public DoNothingTransition(IState<TSender, THost> nextState)
    {
      _nextState = nextState;
    }

    public void Execute(THost host)
    {
    }

    public IState<TSender, THost> PrepareNextState(TSender sender, THost host)
    {
      return PrepareNextStateEmpty(host);
    }

    public IState<TSender, THost> PrepareNextStateEmpty(THost host)
    {
      return _nextState;
    }
  }
}