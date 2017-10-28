// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spines.Hana.Snitch
{
  /// <summary>
  /// Watcher for the windows premium client.
  /// </summary>
  internal class PremiumWatcher : Watcher
  {
    public PremiumWatcher(Func<IEnumerable<ReplayData>, Task> resultHandler)
      : base(resultHandler)
    {
      var directory = GetDirectory();
      if (!Directory.Exists(directory))
      {
        return;
      }
      var fsw = new FileSystemWatcher(directory, FileName);
      fsw.NotifyFilter = NotifyFilters.LastWrite;
      fsw.Changed += OnFileChanged;
      fsw.EnableRaisingEvents = true;
    }

    protected override string GetPath()
    {
      return Path.Combine(GetDirectory(), FileName);
    }

    private const string FileName = "config.ini";

    private async void OnFileChanged(object sender, FileSystemEventArgs e)
    {
      await QueueChange(e.FullPath);
    }

    private static string GetDirectory()
    {
      var roaming = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      return Path.Combine(roaming, "C-EGG", "tenhou", "130");
    }
  }
}