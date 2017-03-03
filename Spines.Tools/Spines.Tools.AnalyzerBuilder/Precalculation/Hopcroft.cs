// Spines.Tools.AnalyzerBuilder.Hopcroft.cs
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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Spines.Tools.AnalyzerBuilder.Precalculation
{
  internal class Hopcroft
  {
    public Hopcroft(IEnumerable<int> normalStates, IEnumerable<int> finalStates, int alphabetSize,
      Func<HashSet<int>, int, IEnumerable<int>> getIncomingStates)
    {
      /*
       * From Wikipedia
       * 
       * P := {F, Q \ F};
         W := {F};
         while (W is not empty) do
           choose and remove a set A from W
           for each c in Σ do
             let X be the set of states for which a transition on c leads to a state in A
             for each set Y in P for which X ∩ Y is nonempty and Y \ X is nonempty do
               replace Y in P by the two sets X ∩ Y and Y \ X
               if Y is in W
                 replace Y in W by the same two sets
               else
                 if |X ∩ Y| <= |Y \ X|
                   add X ∩ Y to W
                 else
                   add Y \ X to W
             end;
           end;
         end;
       */

      var s = Enumerable.Range(0, alphabetSize).ToList(); // The alphabet.
      var n = new HashSet<int>(normalStates); // All nonfinal states.
      var f = finalStates.Select(fs => new HashSet<int> {fs}).ToList(); // Final states are assumed to have different values.
      var p = new HashSet<HashSet<int>> {n}; // The partition.
      var q = f.Select(ff => ff.First()).ToList(); // Single element sets of the partition are collected here for performance.
      var w = new HashSet<HashSet<int>>(f); // The remaining sets to check.

      while (w.Count > 0)
      {
        var a = w.First();
        w.Remove(a);

        foreach (var c in s)
        {
          var x = new HashSet<int>(getIncomingStates(a, c));
          if (x.Count == 0)
          {
            continue; // Intersections with an empty set are always empty.
          }
          foreach (var y in p.ToList()) // p is modified in this loop, but all new elements are inserted into w and checked later.
          {
            if (IsIntersectionEmpty(x, y))
            {
              continue;
            }
            if (IsDifferenceEmpty(y, x))
            {
              continue;
            }
            p.Remove(y);
            var intersection = new HashSet<int>(Intersect(x, y));
            if (intersection.Count > 1)
            {
              p.Add(intersection);
            }
            else
            {
              q.Add(intersection.First());
            }
            var difference = new HashSet<int>(Subtract(y, x));
            if (difference.Count > 1)
            {
              p.Add(difference);
            }
            else
            {
              q.Add(difference.First());
            }

            if (w.Contains(y))
            {
              w.Remove(y);
              w.Add(intersection);
              w.Add(difference);
            }
            else
            {
              w.Add(intersection.Count <= difference.Count ? intersection : difference);
            }
          }
        }
      }

      EquivalenceGroups = p;
    }

    /// <summary>
    /// Each Set in this list contains only elements that are equivalent to each other.
    /// </summary>
    public IEnumerable<ISet<int>> EquivalenceGroups { get; }

    private static IEnumerable<int> Subtract(HashSet<int> a, HashSet<int> b)
    {
      return a.Where(aa => !b.Contains(aa));
    }

    private static IEnumerable<int> Intersect(HashSet<int> a, HashSet<int> b)
    {
      return a.Count < b.Count ? a.Where(b.Contains) : b.Where(a.Contains);
    }

    private static bool IsDifferenceEmpty(HashSet<int> a, HashSet<int> b)
    {
      return b.Count >= a.Count && a.All(b.Contains);
    }

    private static bool IsIntersectionEmpty(HashSet<int> a, HashSet<int> b)
    {
      return a.Count < b.Count ? !a.Any(b.Contains) : !b.Any(a.Contains);
    }
  }
}