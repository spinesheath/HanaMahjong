﻿// Spines.Tenhou.Client.LoggedInState.cs
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
using System.Xml.Linq;

namespace Spines.Tenhou.Client.LocalServer
{
  internal class LoggedInState : LimitedTimeState<LocalConnection>
  {
    /// <summary>
    /// Creates a new instance of LimitedTimeState.
    /// </summary>
    public LoggedInState()
      : base(10000)
    {
    }

    public override IStateTransition<LocalConnection> Process(XElement message)
    {
      throw new NotImplementedException();
    }

    protected override IStateTransition<LocalConnection> CreateTimeOutState()
    {
      throw new NotImplementedException();
    }
  }
}