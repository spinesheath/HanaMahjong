// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Spines.Hana.Snitch
{
  /// <summary>
  /// A history of successfully snitched replays and a separate history of failed attempts.
  /// </summary>
  internal static class History
  {
    public static void Success(ReplayData replay)
    {
      Append(replay, Paths.HistorySuccess);
    }

    public static void Fail(ReplayData replay)
    {
      Append(replay, Paths.HistoryFail);
    }

    public static IEnumerable<ReplayData> Successful()
    {
      return Recent(int.MaxValue);
    }

    public static IEnumerable<ReplayData> Recent(int count)
    {
      return Parse(Paths.HistorySuccess, count);
    }

    public static IEnumerable<ReplayData> All()
    {
      return Successful().Concat(Failed());
    }

    public static IEnumerable<ReplayData> Failed()
    {
      return Parse(Paths.HistoryFail, int.MaxValue);
    }

    private const int MaxHistoryLength = 1000;

    private static void Append(ReplayData replay, string path)
    {
      File.AppendAllLines(path, new[] {ReplayToString(replay)});
    }

    private static IEnumerable<ReplayData> Parse(string path, int count)
    {
      if (!File.Exists(path))
      {
        return Enumerable.Empty<ReplayData>();
      }
      var recent = File.ReadAllLines(path).Select(StringToReplay).ToList();
      if (recent.Count <= MaxHistoryLength)
      {
        recent.Reverse();
        return recent.Take(count);
      }
      // Trim to 100 lines at startup if more than max.
      var remaining = recent.Skip(recent.Count - 100).ToList();
      File.WriteAllLines(path, remaining.Select(ReplayToString));
      remaining.Reverse();
      return remaining.Take(count);
    }

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