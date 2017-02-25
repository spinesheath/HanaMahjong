// Spines.Tools.AnalyzerBuilder.ProgressiveSuitStateMachineBuilder.cs
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
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Tools.AnalyzerBuilder.Precalculation
{
  internal class ProgressiveSuitStateMachineBuilder : IStateMachineBuilder
  {
    /// <summary>
    /// The size of the alphabet.
    /// </summary>
    public int AlphabetSize => 15 + 1;

    /// <summary>
    /// The transitions for the specified language.
    /// </summary>
    public IReadOnlyList<int> Transitions => _transitions;

    public void SetLanguage(IEnumerable<WordWithValue> language)
    {
      CreateLookupData(language);
      CreateTransitions();
    }

    /// <summary>
    /// Is the transition one that describes can not be reached with a legal word?
    /// </summary>
    /// <param name="transition">The Id of the transtion.</param>
    /// <returns>True, if the transition can not be reached, false otherwise.</returns>
    public bool IsNull(int transition)
    {
      return _nullTransitionIds.Contains(transition);
    }

    /// <summary>
    /// Is the transition one that describes a result?
    /// </summary>
    /// <param name="transition">The Id of the transtion.</param>
    /// <returns>True, if the transition is a result, false otherwise.</returns>
    public bool IsResult(int transition)
    {
      return transition % AlphabetSize == 0;
    }

    private readonly Dictionary<int, int> _idToStateColumn = new Dictionary<int, int>();
    private readonly Dictionary<int, int> _idToValue = new Dictionary<int, int>();
    private readonly List<int> _stateColumnToId = new List<int>();
    private ISet<int> _nullTransitionIds;
    private int[] _transitions;

    private void CreateTransitions()
    {
      var columnCount = _stateColumnToId.Count;
      _transitions = new int[AlphabetSize * columnCount];
      _nullTransitionIds = new HashSet<int>();

      for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
      {
        var id = _stateColumnToId[columnIndex];
        _transitions[AlphabetSize * columnIndex] = _idToValue[id]; // value in row 0
        for (var c = 1; c < AlphabetSize; ++c)
        {
          var next = GetNext(id, c - 1);
          var transitionId = AlphabetSize * columnIndex + c;
          if (next.HasValue)
          {
            _transitions[transitionId] = AlphabetSize * _idToStateColumn[next.Value];
          }
          else
          {
            _nullTransitionIds.Add(transitionId);
          }
        }
      }
    }

    private void CreateLookupData(IEnumerable<WordWithValue> language)
    {
      foreach (var word in language)
      {
        var id = GetId(word.Word);
        if (!_idToValue.ContainsKey(id))
        {
          _idToValue.Add(id, word.Value);
        }
      }

      _stateColumnToId.AddRange(_idToValue.Keys.OrderBy(x => x));
      for (var i = 0; i < _stateColumnToId.Count; ++i)
      {
        _idToStateColumn.Add(_stateColumnToId[i], i);
      }
    }

    private static int? GetNext(int s, int c)
    {
      var word = new int[19];
      return GetId(word);
    }

    private static int GetId(IReadOnlyList<int> word)
    {
      return 0;
    }
  }
}