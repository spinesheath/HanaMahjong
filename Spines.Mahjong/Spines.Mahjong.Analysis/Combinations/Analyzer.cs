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
      var nullAnalyzer = new NullStaggeredAnalyzer();
      var jantou2 = new StaggeredAnalyzer(ProtoGroup.Jantou2, nullAnalyzer, _concealed, _used);
      var jantou1 = new StaggeredAnalyzer(ProtoGroup.Jantou1, jantou2, _concealed, _used);
      var shuntsu111 = new StaggeredAnalyzer(ProtoGroup.Shuntsu111, jantou1, _concealed, _used);
      var shuntsu110 = new StaggeredAnalyzer(ProtoGroup.Shuntsu110, shuntsu111, _concealed, _used);
      var shuntsu101 = new StaggeredAnalyzer(ProtoGroup.Shuntsu101, shuntsu110, _concealed, _used);
      var shuntsu011 = new StaggeredAnalyzer(ProtoGroup.Shuntsu011, shuntsu101, _concealed, _used);
      var shuntsu100 = new StaggeredAnalyzer(ProtoGroup.Shuntsu100, shuntsu011, _concealed, _used);
      var shuntsu010 = new StaggeredAnalyzer(ProtoGroup.Shuntsu010, shuntsu100, _concealed, _used);
      var shuntsu001 = new StaggeredAnalyzer(ProtoGroup.Shuntsu001, shuntsu010, _concealed, _used);
      var koutsu3 = new StaggeredAnalyzer(ProtoGroup.Koutsu3, shuntsu001, _concealed, _used);
      var koutsu2 = new StaggeredAnalyzer(ProtoGroup.Koutsu2, koutsu3, _concealed, _used);
      var koutsu1 = new StaggeredAnalyzer(ProtoGroup.Koutsu1, koutsu2, _concealed, _used);

      var arrangement = new Arrangement(0, _meldCount, _meldCount * 3);
      return koutsu1.Analyze(arrangement, 0);
    }
  }
}
