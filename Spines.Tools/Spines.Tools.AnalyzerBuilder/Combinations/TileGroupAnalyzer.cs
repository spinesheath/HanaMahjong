// Spines.Mahjong.Analysis.TileGroupAnalyzer.cs
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

namespace Spines.Tools.AnalyzerBuilder.Combinations
{
  /// <summary>
  /// Analyzes the part of a hand in a single suit.
  /// </summary>
  internal class TileGroupAnalyzer
  {
    private static readonly IReadOnlyList<ProtoGroup> SuitProtoGroups = new List<ProtoGroup>
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

    private static readonly IReadOnlyList<ProtoGroup> HonorProtoGroups = new List<ProtoGroup>
    {
      ProtoGroup.Jantou2,
      ProtoGroup.Jantou1,
      ProtoGroup.Koutsu3,
      ProtoGroup.Koutsu2,
      ProtoGroup.Koutsu1
    };

    private readonly ISet<Arrangement> _arrangements = new HashSet<Arrangement>();
    private readonly List<int> _concealed;
    private readonly int _meldCount;

    private readonly IReadOnlyList<ProtoGroup> _protoGroups;
    private readonly int _tileTypeCount;

    private readonly List<int> _used;
    private int _jantouValue;
    private int _usedMelds;

    /// <summary>
    /// Creates a new instance of TileGroupAnalyzer.
    /// </summary>
    private TileGroupAnalyzer(Combination concealedTiles, Combination meldedTiles, int meldCount, bool allowShuntsu)
    {
      Validate.NotNull(concealedTiles, nameof(concealedTiles));
      Validate.NotNull(meldedTiles, nameof(meldedTiles));
      Validate.InRange(meldCount, 0, 4, nameof(meldCount));

      _meldCount = meldCount;
      _concealed = concealedTiles.Counts.ToList();
      _used = meldedTiles.Counts.ToList();
      _tileTypeCount = concealedTiles.Counts.Count;
      _protoGroups = allowShuntsu ? SuitProtoGroups : HonorProtoGroups;
    }

    /// <summary>
    /// Creates a new instance of TileGroupAnalyzer for analyzing honors.
    /// </summary>
    public static TileGroupAnalyzer ForHonors(Combination concealedTiles, Combination meldedTiles, int meldCount)
    {
      return new TileGroupAnalyzer(concealedTiles, meldedTiles, meldCount, false);
    }

    /// <summary>
    /// Creates a new instance of TileGroupAnalyzer for analyzing suits.
    /// </summary>
    public static TileGroupAnalyzer ForSuits(Combination concealedTiles, Combination meldedTiles, int meldCount)
    {
      return new TileGroupAnalyzer(concealedTiles, meldedTiles, meldCount, true);
    }

    /// <summary>
    /// Returns all possible arrangements for the given hand.
    /// </summary>
    public IEnumerable<Arrangement> Analyze()
    {
      var comparer = new ArrangementComparer();
      var arrangement = new Arrangement(0, _meldCount, _meldCount * 3);
      _usedMelds = _meldCount;
      _jantouValue = 0;
      Analyze(arrangement, 0, 0);
      var arrangements =
        _arrangements.Where(a => !_arrangements.Any(other => comparer.IsWorseThan(a, other))).OrderBy(a => a.Id);
      var compacter = new ArrangementGroupCompacter();
      return compacter.GetCompacted(arrangements);
    }

    private void Analyze(Arrangement arrangement, int currentTileType, int currentProtoGroup)
    {
      if (currentTileType >= _tileTypeCount)
      {
        _arrangements.Add(arrangement);
        return;
      }

      Analyze(arrangement, currentTileType + 1, 0);

      // Inlined a bunch of things for about 25% performance gain.
      var count = _protoGroups.Count;
      for (var i = currentProtoGroup; i < count; ++i)
      {
        var protoGroup = _protoGroups[i];
        var isJantou = i <= 1;
        if (isJantou)
        {
          if (_jantouValue != 0 || !protoGroup.CanInsert(_concealed, _used, currentTileType))
          {
            continue;
          }

          protoGroup.Insert(_concealed, _used, currentTileType);

          var oldJantouValue = _jantouValue;
          _jantouValue = protoGroup.Value;
          var added = arrangement.SetJantouValue(_jantouValue);

          Analyze(added, currentTileType, i);

          protoGroup.Remove(_concealed, _used, currentTileType);
          _jantouValue = oldJantouValue;
        }
        else
        {
          if (_usedMelds == 4 || !protoGroup.CanInsert(_concealed, _used, currentTileType))
          {
            continue;
          }

          protoGroup.Insert(_concealed, _used, currentTileType);

          _usedMelds += 1;
          var added = arrangement.AddMentsu(protoGroup.Value);

          Analyze(added, currentTileType, i);

          protoGroup.Remove(_concealed, _used, currentTileType);
          _usedMelds -= 1;
        }
      }
    }
  }
}