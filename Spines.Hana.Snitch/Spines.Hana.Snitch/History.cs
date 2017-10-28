// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Spines.Hana.Snitch
{
  internal static class History
  {
    public static IEnumerable<ReplayData> All()
    {
      return Recent(int.MaxValue);
    }

    public static IEnumerable<ReplayData> Recent(int count)
    {
      if (!File.Exists(Paths.History))
      {
        return Enumerable.Empty<ReplayData>();
      }
      var recent = File.ReadAllLines(Paths.History).Select(StringToReplay).ToList();
      if (recent.Count <= MaxHistoryLength)
      {
        recent.Reverse();
        return recent.Take(count);
      }
      // Trim to 100 lines at startup if more than max.
      var remaining = recent.Skip(recent.Count - 100).ToList();
      File.WriteAllLines(Paths.History, remaining.Select(ReplayToString));
      remaining.Reverse();
      return remaining.Take(count);
    }

    public static void Append(ReplayData replay)
    {
      File.AppendAllLines(Paths.History, new []{ReplayToString(replay)});
    }

    private const int MaxHistoryLength = 1000;

    private static ReplayData StringToReplay(string value)
    {
      var parts = value.Split(';');
      var id = parts[0];
      var position = Convert.ToInt32(parts[1]);
      return new ReplayData(id, position);
    }

    private static string ReplayToString(ReplayData replay)
    {
      return $"{replay.Id};{replay.Position}";
    }
  }
}