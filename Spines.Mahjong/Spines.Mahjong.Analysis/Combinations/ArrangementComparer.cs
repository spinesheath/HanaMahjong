// Spines.Mahjong.Analysis.ArrangementComparer.cs
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
  /// Determines whether an arrangement is worse than another.
  /// </summary>
  public class ArrangementComparer
  {
    private readonly int _tileCount;

    /// <summary>
    /// Creates a new instance of ArrangementComparer.
    /// </summary>
    public ArrangementComparer(IEnumerable<int> concealedTiles, int meldCount)
    {
      Validate.InRange(meldCount, 0, 4, nameof(meldCount));

      // All melds count as 3 tiles for determining worse arrangements.
      _tileCount = concealedTiles.Sum() + meldCount * 3;
    }

    /// <summary>
    /// Creates a new instance of ArrangementComparer.
    /// </summary>
    public ArrangementComparer(int tileCount)
    {
      _tileCount = Validate.InRange(tileCount, 0, 14, nameof(tileCount));
    }

    /// <summary>
    /// Determines whether an arrangement is worse than another.
    /// </summary>
    public bool IsWorseThan(Arrangement lhs, Arrangement rhs)
    {
      Validate.NotNull(lhs, nameof(lhs));
      Validate.NotNull(rhs, nameof(rhs));

      if (lhs == rhs)
      {
        return false;
      }
      // Same mentsu but better pairs.
      if (lhs.JantouValue < rhs.JantouValue && lhs.MentsuCount == rhs.MentsuCount && lhs.MentsuValue == rhs.MentsuValue)
      {
        return true;
      }
      // Same TotalValue and MentsuCount, but higher PairValue is worse.
      if (lhs.JantouValue > rhs.JantouValue && lhs.MentsuCount == rhs.MentsuCount && lhs.TotalValue == rhs.TotalValue)
      {
        return true;
      }
      // Not enough tiles in other suits to reach the same value.
      var tilesInOtherSuits = 14 - _tileCount;
      if (lhs.TotalValue + tilesInOtherSuits < rhs.TotalValue)
      {
        return true;
      }
      // If there are no tiles in other suits and the total value is equal, take the higher mentsu value (arbitrary choice).
      if (tilesInOtherSuits == 0 && lhs.TotalValue == rhs.TotalValue && lhs.MentsuValue < rhs.MentsuValue)
      {
        return true;
      }
      // If there is exactly one tile in other suits:
      if (tilesInOtherSuits == 1)
      {
        if (lhs.TotalValue < rhs.TotalValue)
        {
          return true;
        }
        if (lhs.HasJantou && rhs.HasJantou && lhs.TotalValue == rhs.TotalValue && lhs.MentsuValue < rhs.MentsuValue)
        {
          return true;
        }
      }
      // Not enough unused groups to reach the same value in other suits.
      var maxWithUnusedGroups = (4 - lhs.MentsuCount) * 3 + (lhs.HasJantou ? 0 : 2);
      if (lhs.TotalValue + maxWithUnusedGroups < rhs.TotalValue)
      {
        return true;
      }
      // Both with or without jantou.
      if (lhs.JantouValue == rhs.JantouValue)
      {
        // Perfect with more mentsu is better than perfect with less mentsu.
        if (lhs.MentsuCount < rhs.MentsuCount)
        {
          return IsPerfect(lhs) && IsPerfect(rhs);
        }
        // Lower value with same MentsuCount is worse.
        if (lhs.MentsuCount == rhs.MentsuCount)
        {
          return lhs.MentsuValue < rhs.MentsuValue;
        }
        // Same value with more mentsu is worse.
        if (lhs.MentsuCount > rhs.MentsuCount)
        {
          return lhs.TotalValue <= rhs.TotalValue;
        }
      }
      return false;
    }

    private static bool IsPerfect(Arrangement arrangement)
    {
      return arrangement.MentsuValue == arrangement.MentsuCount * 3 && arrangement.JantouValue != 1;
    }
  }
}