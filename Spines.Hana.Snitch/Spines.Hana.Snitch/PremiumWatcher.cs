// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spines.Hana.Snitch
{
  internal class PremiumWatcher : Watcher
  {
    public PremiumWatcher(Func<IEnumerable<ReplayData>, Task> resultHandler)
      : base(resultHandler)
    {
      var roaming = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      var folder = Path.Combine(roaming, "C-EGG", "tenhou", "130");
      if (!Directory.Exists(folder))
      {
        return;
      }
      var fsw = new FileSystemWatcher(folder, "config.ini");
      fsw.NotifyFilter = NotifyFilters.LastWrite;
      fsw.Changed += OnFileChanged;
      fsw.EnableRaisingEvents = true;
    }

    protected override Regex ReplayRegex { get; } = new Regex(@"^\d+=file=(\d{10}gm-\d{4}-\d{4}-[\da-f]{8}).*oya=(\d).*sc=(.*)$");

    private async void OnFileChanged(object sender, FileSystemEventArgs e)
    {
      await QueueChange(e.FullPath);
    }
  }
}