// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  public class ReplayManager
  {
    public ReplayManager(IReplayStorage replayStorage, IReplaySource replaySource, ILoggerFactory loggerFactory)
    {
      _replaySource = replaySource;
      _logger = loggerFactory.CreateLogger<ReplayManager>();
      _replayStorage = replayStorage;
    }

    public async Task<string> JsonFor(string replayId)
    {
      var id = new ReplayId(replayId);
      if (id.IsValid)
      {
        return await _replayStorage.ReadJsonAsync(id);
      }

      return null;
    }

    public async Task<HttpStatusCode> PrepareAsync(string replayId)
    {
      var id = new ReplayId(replayId);
      // Deny invalid IDs.
      if (!id.IsValid)
      {
        return HttpStatusCode.BadRequest;
      }

      // If the ID is currently being downloaded, wait on the existing task.
      if (CurrentWork.TryGetValue(replayId, out var t))
      {
        return await t;
      }

      // If the replay already exists in the DB, no need to do anything.
      if (await _replayStorage.Exists(id))
      {
        return HttpStatusCode.NoContent;
      }

      // Try to queue a new download. If it was queued, await that.
      var work = QueuedDownload(id);
      if (CurrentWork.TryAdd(replayId, work))
      {
        return await work;
      }

      // If we were unable to queue the download, that means a download with the same ID is currently running, so try to await that.
      if (CurrentWork.TryGetValue(replayId, out var t2))
      {
        return await t2;
      }

      // If we didn't find the ongoing download, it must have completed somewhere between the TryAdd and the TryGetValue,
      // so all we have to check if the download was successful.
      return await _replayStorage.Exists(id) ? HttpStatusCode.NoContent : HttpStatusCode.NotFound;
    }

    public bool IsValidId(string replayId)
    {
      return new ReplayId(replayId).IsValid;
    }

    public async Task ReparseReplays()
    {
      var replayIds = await _replayStorage.AllIds();
      foreach (var replayId in replayIds)
      {
        var xml = await _replayStorage.ReadXmlAsync(replayId);
        var replay = Replay.Parse(xml);
        var json = JsonConvert.SerializeObject(replay);
        await _replayStorage.SaveJsonAsync(replayId, json);
      }
    }

    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(5);
    private static readonly ConcurrentDictionary<string, Task<HttpStatusCode>> CurrentWork = new ConcurrentDictionary<string, Task<HttpStatusCode>>();
    private static readonly SemaphoreSlim TenhouSemaphore = new SemaphoreSlim(1, 1);
    private readonly ILogger<ReplayManager> _logger;
    private readonly IReplayStorage _replayStorage;
    private readonly IReplaySource _replaySource;

    private async Task<HttpStatusCode> QueuedDownload(ReplayId replayId)
    {
      await TenhouSemaphore.WaitAsync();
      try
      {
        // If the replay was downloaded by now, we are done.
        if (await _replayStorage.Exists(replayId))
        {
          return HttpStatusCode.NoContent;
        }

        // Wait for a while to let tenhou make the replay available.
        // Also prevents too many replays being requested from tenhou at once.
        await Task.Delay(Delay);

        var replay = await _replaySource.GetAsync(replayId);
        if (replay == null)
        {
          return HttpStatusCode.NotFound;
        }
        else
        {
          // Save the downloaded replay.
          var json = JsonConvert.SerializeObject(replay);
          await Task.WhenAll(
            _replayStorage.SaveRelationalData(replayId, replay),
            _replayStorage.SaveJsonAsync(replayId, json),
            _replayStorage.SaveXmlAsync(replayId, replay.RawData));
        }
      }
      catch (Exception e)
      {
        _logger.LogError(e, "Failed to download or store replay.");
        return HttpStatusCode.InternalServerError;
      }
      finally
      {
        CurrentWork.TryRemove(replayId.ToString(), out var unused);
        TenhouSemaphore.Release();
      }

      return HttpStatusCode.NoContent;
    }
  }
}