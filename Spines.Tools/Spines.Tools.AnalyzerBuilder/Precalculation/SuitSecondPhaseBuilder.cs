// Spines.Tools.AnalyzerBuilder.SuitSecondPhaseBuilder.cs
// 
// Copyright (C) 2017  Johannes Heckl
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

using System.Collections.Generic;
using System.Linq;
using Spines.Tools.AnalyzerBuilder.Classification;
using Spines.Utility;

namespace Spines.Tools.AnalyzerBuilder.Precalculation
{
  internal class SuitSecondPhaseBuilder : IStateMachineBuilder
  {
    public SuitSecondPhaseBuilder(string workingDirectory, int meldCount)
    {
      _workingDirectory = workingDirectory;
      _meldCount = meldCount;
    }

    /// <summary>
    /// The transitions for the specified language.
    /// </summary>
    public IReadOnlyList<int> Transitions { get; private set; }

    /// <summary>
    /// The states at which the transitions can be entered.
    /// </summary>
    /// <returns>The ids of the states.</returns>
    public IReadOnlyList<int> EntryStates { get; private set; }

    /// <summary>
    /// The size of the alphabet.
    /// </summary>
    public int AlphabetSize => 5;

    public void SetLanguage()
    {
      var transitions = new UnweightedSuitTransitionsCreator(_workingDirectory).Create().ToList();

      var entryStates = GetEntryStates(transitions);
      var concealedStates = GetConcealedStates(entryStates, transitions);

      var oldToNewTransitions = new Dictionary<int, int>();
      var orderedEntryStates = entryStates.OrderBy(x => x);
      foreach (var entryState in orderedEntryStates)
      {
        oldToNewTransitions.Add(entryState, oldToNewTransitions.Count);
      }
      foreach (var state in concealedStates.Except(entryStates))
      {
        oldToNewTransitions.Add(state, oldToNewTransitions.Count);
      }
      _statesWithFinalValues = new HashSet<int>();

      var concealedTransitions = new int[concealedStates.Count * 5].Populate(-1);
      foreach (var state in concealedStates)
      {
        foreach (var c in Enumerable.Range(0, 5))
        {
          var next = transitions[state + c];
          if (next == -1)
          {
            continue; // null transitions
          }
          if (oldToNewTransitions.ContainsKey(next))
          {
            concealedTransitions[oldToNewTransitions[state] * 5 + c] = oldToNewTransitions[next] * 5;
              // normal transitions
          }
          else
          {
            _statesWithFinalValues.Add(oldToNewTransitions[state]);
            concealedTransitions[oldToNewTransitions[state] * 5 + c] = next; // final values
          }
        }
      }

      Transitions = concealedTransitions;
      EntryStates = entryStates.Select(e => oldToNewTransitions[e]).ToList();
    }

    /// <summary>
    /// Is the transition one that describes can not be reached with a legal word?
    /// </summary>
    /// <param name="transition">The Id of the transtion.</param>
    /// <returns>True, if the transition can not be reached, false otherwise.</returns>
    public bool IsNull(int transition)
    {
      return Transitions[transition] == -1;
    }

    /// <summary>
    /// Is the transition one that describes a result?
    /// </summary>
    /// <param name="transition">The Id of the transtion.</param>
    /// <returns>True, if the transition is a result, false otherwise.</returns>
    public bool IsResult(int transition)
    {
      return _statesWithFinalValues.Contains(transition / 5);
    }

    private readonly string _workingDirectory;
    private readonly int _meldCount;
    private HashSet<int> _statesWithFinalValues;

    private HashSet<int> GetEntryStates(List<int> transitions)
    {
      var entryStates = new HashSet<int>();
      var meldLanguage = _meldCount.Yield().Concat(Enumerable.Repeat(Enumerable.Range(0, 5), 9)).CartesianProduct();

      foreach (var word in meldLanguage)
      {
        var current = 0;
        foreach (var c in word)
        {
          current = transitions[current + c];
          if (current == -1)
          {
            break;
          }
        }
        if (current != -1)
        {
          entryStates.Add(current);
        }
      }
      return entryStates;
    }

    private static HashSet<int> GetConcealedStates(HashSet<int> entryStates, List<int> transitions)
    {
      var concealedStates = new HashSet<int>();

      var statesInCurrentLevel = new HashSet<int>(entryStates);
      for (var i = 0; i < 9; ++i)
      {
        var statesInPreviousLevel = statesInCurrentLevel;
        statesInCurrentLevel = new HashSet<int>();
        foreach (var state in statesInPreviousLevel)
        {
          concealedStates.Add(state);

          foreach (var c in Enumerable.Range(0, 5))
          {
            var n = transitions[state + c];
            if (n == -1)
            {
              continue;
            }
            statesInCurrentLevel.Add(n);
          }
        }
      }
      return concealedStates;
    }
  }
}