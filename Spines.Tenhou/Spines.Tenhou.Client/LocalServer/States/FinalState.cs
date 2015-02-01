// Spines.Tenhou.Client.FinalState.cs
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

using System.Xml.Linq;
using Spines.Tenhou.Client.LocalServer.Transitions;

namespace Spines.Tenhou.Client.LocalServer.States
{
  internal class FinalState<TSender, THost> : IState<TSender, THost>
  {
    public IStateTransition<TSender, THost> Process(TSender sender, XElement message)
    {
      return ProcessEmpty();
    }

    public bool IsFinal
    {
      get { return true; }
    }

    public IStateTransition<TSender, THost> ProcessEmpty()
    {
      return new DoNothingTransition<TSender, THost>(this);
    }
  }
}