// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Spines.Hana.Snitch
{
  internal static class History
  {
    public static IEnumerable<string> All()
    {
      return Recent(int.MaxValue);
    }

    public static IEnumerable<string> Recent(int count)
    {
      if (!File.Exists(Paths.History))
      {
        return Enumerable.Empty<string>();
      }
      var recent = File.ReadAllLines(Paths.History);
      if (recent.Length <= 200)
      {
        return recent.Reverse().Take(count);
      }
      // Trim to 100 lines at startup if more than 200.
      var remaining = recent.Skip(recent.Length - 100).ToList();
      File.WriteAllLines(Paths.History, remaining);
      remaining.Reverse();
      return remaining.Take(count);
    }

    public static void Append(IEnumerable<string> ids)
    {
      File.AppendAllLines(Paths.History, ids);
    }
  }
}