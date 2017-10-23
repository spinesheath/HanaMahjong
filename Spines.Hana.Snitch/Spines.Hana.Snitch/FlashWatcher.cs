// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spines.Hana.Snitch
{
  internal class FlashWatcher : Watcher
  {
    public FlashWatcher(Func<IEnumerable<ReplayData>, Task> resultHandler)
      : base(resultHandler)
    {
      var roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      var shared = Path.Combine(roaming, "Macromedia", "Flash Player", "#SharedObjects");
      if (!Directory.Exists(shared))
      {
        return;
      }
      var mjv = Directory.GetDirectories(shared, "mjv.jp", SearchOption.AllDirectories).FirstOrDefault();
      if (mjv == null)
      {
        return;
      }
      var fsw = new FileSystemWatcher(mjv, "mjinfo.sol");
      fsw.NotifyFilter = NotifyFilters.FileName;
      fsw.Renamed += OnRenamed;
      fsw.EnableRaisingEvents = true;
    }

    protected override Regex ReplayRegex { get; } = new Regex(@"file=(\d{10}gm%2D\d{4}%2D\d{4}%2Dx[\da-f]{12}).*oya=(\d).*sc=([\d\.,-]*)");

    private async void OnRenamed(object sender, FileSystemEventArgs e)
    {
      await QueueChange(e.FullPath);
    }
  }
}