// Spines.Mahjong.Analysis.StateManager.cs
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
using Spines.Utility;

namespace Spines.Tools.AnalyzerBuilder.Classification
{
  internal class StateManager
  {
    private readonly int _alphabetSize;
    private readonly Dictionary<int, FinalState> _finalStates;
    private readonly int _heights;
    private readonly Dictionary<State, State>[] _uniqueStates;
    private int[] _transitions;

    public StateManager(int alphabetSize, int wordLength)
    {
      _alphabetSize = alphabetSize;
      _heights = wordLength + 1;
      _uniqueStates = new Dictionary<State, State>[_heights];
      for (var i = 0; i < _heights; ++i)
      {
        _uniqueStates[i] = new Dictionary<State, State>(new StateComparer());
      }
      _finalStates = new Dictionary<int, FinalState>();
      StartingState = new State(_alphabetSize);
      InsertUniqueState(StartingState, _heights - 1);
    }

    public State StartingState { get; }

    /// <summary>
    /// If this method is called before all states are finalized, the result will not be correct.
    /// Usage:
    /// int current = 0;
    /// foreach(int c in word)
    /// current = table[current + c];
    /// return current;
    /// </summary>
    public IReadOnlyList<int> Transitions => _transitions;

    /// <summary>
    /// Indices of the transitions that contain a final value in the transitions array.
    /// </summary>
    public ISet<int> ResultIndexes { get; private set; }

    /// <summary>
    /// Assigns each state a unique Id and creates a transition table.
    /// </summary>
    public void CompactTransitions()
    {
      // Give each state a unique Id.
      var id = 0;
      for (var i = _heights - 1; i >= 0; --i)
      {
        var row = _uniqueStates[i];
        foreach (var state in row)
        {
          state.Value.Id = id;
          id += 1;
        }
      }
      // Create the actual machine.
      _transitions = new int[id * _alphabetSize].Populate(-1);
      ResultIndexes = new HashSet<int>();
      for (var i = _heights - 1; i > 0; --i)
      {
        var row = _uniqueStates[i];
        foreach (var svp in row)
        {
          var state = svp.Value;
          for (var j = 0; j < _alphabetSize; ++j)
          {
            var nextState = state.Advance(j);
            if (nextState != null)
            {
              var index = GetIndex(state.Id, j);
              if (i == 1)
              {
                var finalState = (FinalState) nextState;
                _transitions[index] = finalState.Value;
                ResultIndexes.Add(index);
              }
              else
              {
                _transitions[index] = nextState.Id * _alphabetSize;
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// The index of a transition in the transition table.
    /// </summary>
    private int GetIndex(int state, int character)
    {
      return _alphabetSize * state + character;
    }

    public void RemoveUniqueState(State state, int height)
    {
      if (!_uniqueStates[height].Remove(state))
      {
        throw new InvalidOperationException("State isn't unique.");
      }
    }

    public void InsertUniqueState(State state, int height)
    {
      _uniqueStates[height].Add(state, state);
    }

    public State TryGetEquivalentUniqueState(State state, int height)
    {
      var statesAtHeight = _uniqueStates[height];
      State uniqueState;
      var isRedundant = statesAtHeight.TryGetValue(state, out uniqueState);
      return isRedundant ? uniqueState : null;
    }

    public State GetOrCreateFinalState(int value)
    {
      if (_finalStates.ContainsKey(value))
      {
        return _finalStates[value];
      }
      var final = new FinalState(_alphabetSize, value);
      _finalStates.Add(value, final);
      return final;
    }
  }
}