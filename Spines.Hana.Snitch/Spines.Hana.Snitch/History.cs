// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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
      if (!File.Exists(Path))
      {
        return Enumerable.Empty<string>();
      }
      var recent = File.ReadAllLines(Path);
      if (recent.Length <= 200)
      {
        return recent.Reverse().Take(count);
      }
      // Trim to 100 lines at startup if more than 200.
      var remaining = recent.Skip(recent.Length - 100).ToList();
      File.WriteAllLines(Path, remaining);
      remaining.Reverse();
      return remaining.Take(count);
    }

    public static void Append(IEnumerable<string> ids)
    {
      File.AppendAllLines(Path, ids);
    }

    private static readonly string Path = GetPath();

    private static string GetPath()
    {
      var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      var snitch = System.IO.Path.Combine(appData, Application.ProductName);
      if (!Directory.Exists(snitch))
      {
        Directory.CreateDirectory(snitch);
      }
      return System.IO.Path.Combine(snitch, "recent.txt");
    }
  }
}