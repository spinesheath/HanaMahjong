// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows.Forms;

namespace Spines.Hana.Snitch
{
  internal static class Paths
  {
    public static string HistorySuccess { get; } = GetPath("recent.txt");
    public static string HistoryFail { get; } = GetPath("failed.txt");
    public static string Log { get; } = GetPath("log.txt");

    private static string GetPath(string fileName)
    {
      var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      var snitch = Path.Combine(appData, Application.ProductName);
      if (!Directory.Exists(snitch))
      {
        Directory.CreateDirectory(snitch);
      }
      return Path.Combine(snitch, fileName);
    }
  }
}