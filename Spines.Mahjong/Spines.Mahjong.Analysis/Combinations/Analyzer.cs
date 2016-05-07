using System.Collections.Generic;
using System.Linq;

namespace Spines.Mahjong.Analysis.Combinations
{
  internal class Analyzer
  {
    private readonly int _meldCount;
    private readonly List<int> _concealed;
    private readonly List<int> _used;

    /// <summary>
    /// Creates a new instance of Analyzer.
    /// </summary>
    public Analyzer(Combination concealedTiles, Combination meldedTiles, int meldCount)
    {
      _meldCount = meldCount;
      _concealed = concealedTiles.Counts.ToList();
      _used = meldedTiles.Counts.ToList();
    }

    /// <summary>
    /// Returns all possible arrangements for the given hand.
    /// </summary>
    public IEnumerable<Arrangement> Analyze()
    {
      var arrangement = new Arrangement(0, _meldCount, _meldCount * 3);
      return Analyze(arrangement, 0, 0);
    }

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

    private IEnumerable<Arrangement> Analyze(Arrangement arrangement, int currentTileType, int currentProtoGroup)
    {
      if (currentTileType >= 9)
      {
        yield return arrangement;
        yield break;
      }
      foreach (var a in Analyze(arrangement, currentTileType + 1, 0))
      {
        yield return a;
      }
      for (var i = currentProtoGroup; i < _protoGroups.Count; ++i)
      {
        var protoGroup = _protoGroups[i];
        if (CanNotInsert(arrangement, currentTileType, protoGroup))
        {
          continue;
        }
        protoGroup.Insert(_concealed, _used, currentTileType);
        var added = protoGroup.IsJantou ? arrangement.AddJantou(protoGroup.Value) : arrangement.AddMentsu(protoGroup.Value);
        foreach (var a in Analyze(added, currentTileType, i))
        {
          yield return a;
        }
        protoGroup.Remove(_concealed, _used, currentTileType);
      }
    }

    private bool CanNotInsert(Arrangement arrangement, int currentTileType, ProtoGroup protoGroup)
    {
      if (protoGroup.IsJantou && arrangement.Jantou > 0)
      {
        return true;
      }
      if (!protoGroup.IsJantou && arrangement.Mentsu > 3)
      {
        return true;
      }
      return !protoGroup.CanInsert(_concealed, _used, currentTileType);
    }
  }
}
