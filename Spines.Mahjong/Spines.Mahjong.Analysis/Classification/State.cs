// Spines.Mahjong.Analysis.State.cs
// 
// Copyright (C) 2016  Johannes Heckl
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

namespace Spines.Mahjong.Analysis.Classification
{
  internal class State
  {
    private int _incomingTransitions;
    private readonly State[] _targetStates;

    public State(int alphabetSize)
    {
      _incomingTransitions = 0;
      _targetStates = new State[alphabetSize];
    }

    public int Id { get; set; }

    public bool IsConfluenceState => _incomingTransitions > 1;

    public State Clone(int alphabetSize)
    {
      var s = new State(alphabetSize);
      for (var i = 0; i < alphabetSize; ++i)
      {
        if (HasTransition(i))
        {
          s.CreateOutTransition(Advance(i), i);
        }
      }
      return s;
    }

    public bool HasTransition(int character)
    {
      return Advance(character) != null;
    }

    public State Advance(int character)
    {
      return _targetStates[character];
    }

    public void RedirectOutTransition(State target, int character)
    {
      if (HasTransition(character))
      {
        Advance(character)._incomingTransitions -= 1;
        SetTransition(target, character);
      }
      else
      {
        throw new InvalidOperationException("Transition doesn't exist.");
      }
    }

    public void CreateOutTransition(State target, int character)
    {
      if (HasTransition(character))
      {
        throw new InvalidOperationException("Transition already exists.");
      }
      SetTransition(target, character);
    }

    private void SetTransition(State target, int character)
    {
      _targetStates[character] = target;
      ++target._incomingTransitions;
    }

    public class StateComparer : IEqualityComparer<State>
    {
      private readonly int _alphabetSize;

      public StateComparer(int alphabetSize)
      {
        _alphabetSize = alphabetSize;
      }

      public bool Equals(State first, State second)
      {
        return first._targetStates.SequenceEqual(second._targetStates);
      }

      public int GetHashCode(State state)
      {
        var hc = 1;
        for (var i = 0; i < _alphabetSize; ++i)
        {
          hc = unchecked(hc * 17 + (state._targetStates[i] == null ? 0 : state._targetStates[i].GetHashCode()));
        }
        return hc;
      }
    }
  }
}