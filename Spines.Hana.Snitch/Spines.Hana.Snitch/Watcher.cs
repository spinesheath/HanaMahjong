// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Spines.Hana.Snitch
{
  internal abstract class Watcher
  {
    public event EventHandler HistoryUpdated;

    /// <summary>
    /// Scans for existing files that have not been added to the history yet.
    /// </summary>
    public async Task Scan()
    {
      var path = GetPath();
      if (File.Exists(path))
      {
        await QueueChange(path);
      }
    }

    protected Watcher(Func<IEnumerable<ReplayData>, Task> resultHandler)
    {
      _resultHandler = resultHandler;
    }

    /// <summary>
    /// Gets the path to the replay file.
    /// </summary>
    protected abstract string GetPath();

    /// <summary>
    /// Queues a timestamp, then waits until a second after the last timestamp.
    /// FileSystemWatcher sometimes raises multiple events for a single modification.
    /// </summary>
    protected async Task QueueChange(string path)
    {
      _fileChangeQueue.Enqueue(DateTime.UtcNow);
      if (_semaphore.CurrentCount == 0)
      {
        return;
      }
      await _semaphore.WaitAsync();
      try
      {
        await ClearQueue();
        var newReplays = ReadFile(path);
        await _resultHandler(newReplays);
      }
      catch (IOException)
      {
        await QueueChange(path);
        Logger.Warn($"IOException on file change {path}");
      }
      catch (Exception ex)
      {
        Logger.Error(ex, "on file change");
      }
      finally
      {
        _semaphore.Release();
      }
    }

    private readonly Func<IEnumerable<ReplayData>, Task> _resultHandler;
    private readonly ConcurrentQueue<DateTime> _fileChangeQueue = new ConcurrentQueue<DateTime>();
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    private IEnumerable<ReplayData> ReadFile(string path)
    {
      var lines = File.ReadAllLines(path);
      var results = ReplayId.GetData(lines.Reverse());

      var recent = new HashSet<string>(History.All().Select(r => r.Id));
      var newReplays = results.Where(r => !recent.Contains(r.Id)).ToList();
      if (!newReplays.Any())
      {
        return Enumerable.Empty<ReplayData>();
      }

      History.Append(newReplays);

      HistoryUpdated?.Invoke(this, EventArgs.Empty);

      return newReplays;
    }

    /// <summary>
    /// Waits until a second after the last timestamp in the queue.
    /// </summary>
    private async Task ClearQueue()
    {
      while (_fileChangeQueue.TryDequeue(out var next))
      {
        var target = next + TimeSpan.FromSeconds(1);
        var now = DateTime.UtcNow;
        if (target > now)
        {
          await Task.Delay(target - now);
        }
      }
    }
  }
}