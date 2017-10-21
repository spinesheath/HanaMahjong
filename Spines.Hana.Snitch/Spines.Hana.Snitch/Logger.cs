// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Spines.Hana.Snitch
{
  /// <summary>
  /// A small application like this doen't need a real logger yet.
  /// </summary>
  internal static class Logger
  {
    public static void Error(Exception e, string message)
    {
      var now = DateTime.Now.ToString("G", CultureInfo.InvariantCulture);
      Log($"Error | {now} | {message} | {e.GetType().Name} | {e.Message}");
    }

    public static void Warn(string message)
    {
      var now = DateTime.Now.ToString("G", CultureInfo.InvariantCulture);
      Log($"Warn | {now} | {message}");
    }

    private static readonly object LogLock = new object();

    private static void Log(string message)
    {
      lock (LogLock)
      {
        if (File.Exists(Paths.Log))
        {
          var allLines = File.ReadAllLines(Paths.Log);
          if (allLines.Length > 200)
          {
            File.WriteAllLines(Paths.Log, allLines.Skip(allLines.Length - 100));
          }
        }

        File.AppendAllLines(Paths.Log, new[] {message});
      }
    }
  }
}