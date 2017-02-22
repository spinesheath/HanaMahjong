// Spines.Tools.AnalyzerBuilder.HonorStateMachineBuilder.cs
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
using Spines.Tools.AnalyzerBuilder.Precalculation;

namespace Spines.Tools.AnalyzerBuilder.StateProgression
{
  internal class HonorStateMachineBuilder
  {
    private readonly string _workingDirectory;
    private readonly Dictionary<int, int> _idToValue = new Dictionary<int, int>();
    private readonly Dictionary<int, int> _idToStateColumn = new Dictionary<int, int>();
    private readonly List<int> _stateColumnToId = new List<int>(); 

    public HonorStateMachineBuilder(string workingDirectory)
    {
      _workingDirectory = workingDirectory;
    }

    public void Create()
    {
      CreateLookupData();
      CreateTable();
    }

    private void CreateTable()
    {
      const int alphabetSize = 15;
      var columnCount = _stateColumnToId.Count;
      const int rowCount = alphabetSize + 1;
      var table = new int[rowCount * columnCount];

      for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
      {
        var id = _stateColumnToId[columnIndex];
        table[rowCount * columnIndex + 0] = _idToValue[id]; // value in row 0
        for (var c = 0; c < alphabetSize; ++c)
        {
          var next = GetNext(id, c);
          if (next.HasValue)
          {
            table[rowCount * columnIndex + c + 1] = rowCount * _idToStateColumn[next.Value];
          }
          else
          {
            table[rowCount * columnIndex + c + 1] = -1;
          }
        }
      }
    }

    private static int? GetNext(int s, int c)
    {
      var totalCount = 0;
      var word = new int[15];
      for (var i = 0; i < 7; ++i)
      {
        var t = (s >> i * 3) & 7;
        if (t == 7)
        {
          word[i + 1] = 4;
          totalCount += 3;
        }
        else if (t > 4)
        {
          word[i + 1] = 3;
          totalCount += 3;
          word[i + 7 + 1] = t - 5;
          totalCount += t - 5;
        }
        else
        {
          word[i + 7 + 1] = t;
          totalCount += t;
        }
      }

      if (c <= 3) // draw without meld
      {
        if (totalCount == 14) // cant draw more
        {
          return null;
        }

        var existingCount = c;
        for (var i = 0; i < 7; ++i)
        {
          if (word[i + 7 + 1] == existingCount && word[i + 1] == 0)
          {
            word[i + 7 + 1] += 1;
            break;
          }
          if (i == 6) // hand does not match up with action requirements
          {
            return null;
          }
        }
      }
      else if (c == 4) // draw with meld
      {
        if (totalCount == 14) // cant draw more
        {
          return null;
        }

        for (var i = 0; i < 7; ++i)
        {
          if (word[i + 7 + 1] == 0 && word[i + 1] == 3)
          {
            word[i + 7 + 1] = 1;
            break;
          }
          if (i == 6) // hand does not match up with action requirements
          {
            return null;
          }
        }
      }
      else if (c <= 8) // discard without meld
      {
        var existingCount = c - 4;
        for (var i = 0; i < 7; ++i)
        {
          if (word[i + 7 + 1] == existingCount && word[i + 1] == 0)
          {
            word[i + 7 + 1] -= 1;
            break;
          }
          if (i == 6) // hand does not match up with action requirements
          {
            return null;
          }
        }
      }
      else if (c == 9) // discard with meld
      {
        for (var i = 0; i < 7; ++i)
        {
          if (word[i + 7 + 1] == 1 && word[i + 1] == 3)
          {
            word[i + 7 + 1] = 0;
            break;
          }
          if (i == 6) // hand does not match up with action requirements
          {
            return null;
          }
        }
      }
      else if (c <= 11) // pon
      {
        if (totalCount == 14) // cant pon here
        {
          return null;
        }

        var existingCount = c - 8;
        for (var i = 0; i < 7; ++i)
        {
          if (word[i + 7 + 1] == existingCount && word[i + 1] == 0)
          {
            word[i + 7 + 1] -= 2;
            word[i + 1] = 3;
            break;
          }
          if (i == 6) // hand does not match up with action requirements
          {
            return null;
          }
        }
      }
      else if (c == 12) // daiminkan
      {
        if (totalCount == 14) // cant daiminkan here
        {
          return null;
        }

        for (var i = 0; i < 7; ++i)
        {
          if (word[i + 7 + 1] == 3 && word[i + 1] == 0)
          {
            word[i + 7 + 1] = 0;
            word[i + 1] = 4;
            break;
          }
          if (i == 6) // hand does not match up with action requirements
          {
            return null;
          }
        }
      }
      else if (c == 13) // chakan
      {
        for (var i = 0; i < 7; ++i)
        {
          if (word[i + 7 + 1] == 1 && word[i + 1] == 3)
          {
            word[i + 7 + 1] = 0;
            word[i + 1] = 4;
            break;
          }
          if (i == 6) // hand does not match up with action requirements
          {
            return null;
          }
        }
      }
      else if (c == 13) // ankan
      {
        for (var i = 0; i < 7; ++i)
        {
          if (word[i + 7 + 1] == 4 && word[i + 1] == 0)
          {
            word[i + 7 + 1] = 0;
            word[i + 1] = 4;
            break;
          }
          if (i == 6) // hand does not match up with action requirements
          {
            return null;
          }
        }
      }

      return GetId(word);
    }

    private void CreateLookupData()
    {
      var words = new CompactAnalyzedDataCreator(_workingDirectory).CreateHonorWords();
      foreach (var word in words)
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

    private static int GetId(IReadOnlyList<int> word)
    {
      var c = Enumerable.Range(0, 7).Select(i => word[i + 7 + 1] + word[i + 1] * 2 - word[i + 1] / 3);
      return c.OrderByDescending(x => x).Select((a, i) => a << i * 3).Sum();
    }
  }
}