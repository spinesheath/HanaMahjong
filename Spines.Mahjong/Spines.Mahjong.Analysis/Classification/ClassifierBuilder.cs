// Spines.Mahjong.Analysis.ClassifierBuilder.cs
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

using System.Collections.Generic;
using Spines.Utility;

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// Creates the transition table for a Classifier.
  /// </summary>
  public class ClassifierBuilder
  {
    private readonly StateManager _stateManager;

    /// <summary>
    /// Creates a minimized dfa and the corresponding transition table.
    /// </summary>
    public ClassifierBuilder(int alphabetSize, int wordLength)
    {
      AlphabetSize = alphabetSize;
      WordLength = wordLength;
      _stateManager = new StateManager(AlphabetSize, WordLength);
    }

    /// <summary>
    /// The size of the alphabet.
    /// </summary>
    public int AlphabetSize { get; }

    /// <summary>
    /// The length of the words.
    /// </summary>
    public int WordLength { get; }

    /// <summary>
    /// Creates the classifier transitions for the specified language.
    /// </summary>
    public IEnumerable<int> CreateTransitions()
    {
      return _stateManager.GetCompactTransitions();
    }

    /// <summary>
    /// Checks if the word already exists in the dfa.
    /// </summary>
    private bool HasBeenAdded(IEnumerable<int> word)
    {
      var curState = _stateManager.StartingState;
      foreach (var c in word)
      {
        if (curState.HasTransition(c))
        {
          curState = curState.Advance(c);
        }
        else
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Adds the words to the dfa.
    /// </summary>
    public void AddWord(WordWithValue word)
    {
      Validate.NotNull(word, nameof(word));
      MergeWord(word);
    }

    /// <summary>
    /// Adds a word to the dfa and minimizes the dfa.
    /// </summary>
    private void MergeWord(WordWithValue word)
    {
      if (HasBeenAdded(word.Word))
      {
        return;
      }
      var curUnique = _stateManager.StartingState;
      var i = 0;
      var monofluentStates = new Stack<State>();
      // We need the previous state for each state.
      monofluentStates.Push(curUnique);
      // Traverse common prefix before the first confluence state.
      while (i < WordLength - 1 &&
             curUnique.HasTransition(word.Word[i]) &&
             !curUnique.Advance(word.Word[i]).IsConfluenceState)
      {
        curUnique = curUnique.Advance(word.Word[i]);
        monofluentStates.Push(curUnique);
        i += 1;
      }
      // Here curUnique points to the last State before the first confluence State,
      // or the last state in the common prefix if there is no confluence state.
      // Here i is the index of the character of the transition away from curUnique.

      if (curUnique != _stateManager.StartingState)
      {
        // This state will have an outgoing transition changed.
        // It has to be reinserted.
        // Have to remove it before changing because the transitions are the dictionary key.
        _stateManager.RemoveUniqueState(curUnique, GetHeight(i - 1));
      }

      // Clone rest of common prefix.
      var lastAdded = curUnique;
      var clones = new Stack<State>();
      // We need the previous state for each clone.
      clones.Push(lastAdded);
      while (i < WordLength - 1 &&
             curUnique.HasTransition(word.Word[i]))
      {
        curUnique = curUnique.Advance(word.Word[i]);
        var clone = curUnique.Clone(AlphabetSize);
        lastAdded.RedirectOutTransition(clone, word.Word[i]);
        lastAdded = clone;
        clones.Push(clone);
        i += 1;
      }
      // Here curUnique points to the last state in the common prefix if continued on the unique path.
      // Here lastAdded points to the last state in the common prefix if continued on the cloned path.
      // Here i is the index of the character of the transition away from lastAdded.

      // Add the rest of the word to the dfa.
      AddSuffix(lastAdded, word, i);

      // Merge clones into the state machine.
      i -= 1;
      i = MergeStates(word.Word, i, clones);

      // Fix monofluent States:
      // remove them from the dfa and either insert them back in or merge them with an equivalent state.
      while (monofluentStates.Count > 1)
      {
        var curState = monofluentStates.Pop();
        var prevState = monofluentStates.Peek();
        var uniqueState = _stateManager.TryGetEquivalentUniqueState(curState, GetHeight(i));
        if (uniqueState == null)
        {
          _stateManager.InsertUniqueState(curState, GetHeight(i));
          break;
        }
        if (prevState != _stateManager.StartingState)
        {
          _stateManager.RemoveUniqueState(prevState, GetHeight(i - 1));
        }
        prevState.RedirectOutTransition(uniqueState, word.Word[i]);
        i -= 1;
      }
    }

    /// <summary>
    /// The height of a state that is reached by a letter with the given index in the word.
    /// </summary>
    private int GetHeight(int incomingCharacterIndex)
    {
      return WordLength - incomingCharacterIndex - 1;
    }

    /// <summary>
    /// Merges the states into the dfa.
    /// </summary>
    /// <param name="word">The current word.</param>
    /// <param name="i">The index of the letter that leads to the top state in the stack.</param>
    /// <param name="states">
    /// The states to be merged plus one predecessor at the bottom, ordered by height with top being the
    /// lowest height.
    /// </param>
    /// <returns>The index of the letter that leads to the predecessor at the bottom of the stack.</returns>
    private int MergeStates(IReadOnlyList<int> word, int i, Stack<State> states)
    {
      while (states.Count > 1)
      {
        var curState = states.Pop();
        var prevState = states.Peek();
        var uniqueState = _stateManager.TryGetEquivalentUniqueState(curState, GetHeight(i));
        if (uniqueState == null)
        {
          _stateManager.InsertUniqueState(curState, GetHeight(i));
        }
        else
        {
          prevState.RedirectOutTransition(uniqueState, word[i]);
        }
        i -= 1;
      }
      return i;
    }

    /// <summary>
    /// Adds the rest of the word to the dfa.
    /// </summary>
    /// <param name="parent">The state to attack the suffix to.</param>
    /// <param name="word">The current word.</param>
    /// <param name="wordPos">The index of the first letter of the suffix.</param>
    private void AddSuffix(State parent, WordWithValue word, int wordPos)
    {
      var states = new Stack<State>();
      states.Push(parent);
      // Create new States.
      var i = wordPos;
      while (i < WordLength - 1)
      {
        var prevState = states.Peek();
        var newState = new State(AlphabetSize);
        prevState.CreateOutTransition(newState, word.Word[i]);
        states.Push(newState);
        i += 1;
      }
      // Connect to final State.
      var final = _stateManager.GetOrCreateFinalState(word.Value);
      states.Peek().CreateOutTransition(final, word.Word[WordLength - 1]);
      // Merge new States with unique States.
      i -= 1;
      MergeStates(word.Word, i, states);
    }

    /// <summary>
    /// Returns the indices of transitions that represent final values.
    /// </summary>
    /// <returns>A sequence of indices in the transitions table.</returns>
    public IEnumerable<int> GetResultIndices()
    {
      return _stateManager.GetResultIndices();
    }
  }
}