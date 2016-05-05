using System.Collections.Generic;
using System.Linq;

namespace Spines.Mahjong.Analysis.Combinations
{
  internal class Analyzer
  {
    private readonly int _meldCount;
    private readonly IList<int> _concealed;
    private readonly IList<int> _used;

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
      return Analyze(arrangement, 0);
    }

    private IEnumerable<Arrangement> Analyze(Arrangement arrangement, int currentTileType)
    {
      //foreach (var arr in followUp_.Analyze(availableTiles, usedTiles, 0, a))
      //{
      //  yield return arr;
      //}
      if (arrangement.Mentsu >= 4)
      {
        yield return arrangement;
      }
      else
      {
        for (var i = currentTileType; i < 9; ++i)
        {
          const int tilesToOccupy = 3; // a koutsu occupies 3 tiles if completed
          const int targetValue = 3; // a proto-koutsu can have 1 to 3 tiles
          if (_concealed[i] - _used[i] >= targetValue && _used[i] <= 4 - tilesToOccupy)
          {
            _used[i] += tilesToOccupy;
            _concealed[i] -= targetValue;
            var current = arrangement.AddMentsu(tilesToOccupy);
            foreach (var arr in Analyze(current, i))
            {
              yield return arr;
            }
            _concealed[i] += targetValue;
            _used[i] -= tilesToOccupy;
          }
        }
      }
    }
  }
}
