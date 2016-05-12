// Spines.Mahjong.Analysis.SuitAnalyzer.cs
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
using System.Linq;
using Spines.Utility;

namespace Spines.Mahjong.Analysis.Combinations
{
  /// <summary>
  /// Analyzes the part of a hand in a single suit.
  /// </summary>
  public class SuitAnalyzer
  {
    private readonly ISet<Arrangement> _arrangements = new HashSet<Arrangement>();
    private readonly List<int> _concealed;
    private readonly int _meldCount;

    private readonly IReadOnlyList<ProtoGroup> _protoGroups = new List<ProtoGroup>
    {
      ProtoGroup.Jantou2,
      ProtoGroup.Jantou1,
      ProtoGroup.Shuntsu111,
      ProtoGroup.Shuntsu110,
      ProtoGroup.Shuntsu101,
      ProtoGroup.Shuntsu011,
      ProtoGroup.Shuntsu100,
      ProtoGroup.Shuntsu010,
      ProtoGroup.Shuntsu001,
      ProtoGroup.Koutsu3,
      ProtoGroup.Koutsu2,
      ProtoGroup.Koutsu1
    };

    private readonly List<int> _used;

    /// <summary>
    /// Creates a new instance of SuitAnalyzer.
    /// </summary>
    public SuitAnalyzer(Combination concealedTiles, Combination meldedTiles, int meldCount)
    {
      Validate.NotNull(concealedTiles, nameof(concealedTiles));
      Validate.NotNull(meldedTiles, nameof(meldedTiles));
      Validate.InRange(meldCount, 0, 4, nameof(meldCount));

      _meldCount = meldCount;
      _concealed = concealedTiles.Counts.ToList();
      _used = meldedTiles.Counts.ToList();
    }

    /// <summary>
    /// Returns all possible arrangements for the given hand.
    /// </summary>
    public IEnumerable<Arrangement> Analyze()
    {
      var comparer = new ArrangementComparer(_concealed, _meldCount);
      var arrangement = new Arrangement(0, _meldCount, _meldCount * 3);
      Analyze(arrangement, 0, 0);
      return _arrangements.Where(a => !_arrangements.Any(other => comparer.IsWorseThan(a, other))).OrderBy(a => a.Id);
    }

    private void Analyze(Arrangement arrangement, int currentTileType, int currentProtoGroup)
    {
      if (currentTileType >= 9)
      {
        _arrangements.Add(arrangement);
        return;
      }

      Analyze(arrangement, currentTileType + 1, 0);

      for (var i = currentProtoGroup; i < _protoGroups.Count; ++i)
      {
        var protoGroup = _protoGroups[i];
        if (CanNotInsert(arrangement, currentTileType, protoGroup))
        {
          continue;
        }
        protoGroup.Insert(_concealed, _used, currentTileType);
        var isJantou = protoGroup == ProtoGroup.Jantou1 || protoGroup == ProtoGroup.Jantou2;
        var added = isJantou ? arrangement.SetJantouValue(protoGroup.Value) : arrangement.AddMentsu(protoGroup.Value);
        Analyze(added, currentTileType, i);
        protoGroup.Remove(_concealed, _used, currentTileType);
      }
    }

    private bool CanNotInsert(Arrangement arrangement, int currentTileType, ProtoGroup protoGroup)
    {
      var isJantou = protoGroup == ProtoGroup.Jantou1 || protoGroup == ProtoGroup.Jantou2;
      if (isJantou && arrangement.JantouValue != 0)
      {
        return true;
      }
      if (!isJantou && arrangement.MentsuCount == 4)
      {
        return true;
      }
      return !protoGroup.CanInsert(_concealed, _used, currentTileType);
    }
  }
}