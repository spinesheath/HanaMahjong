// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Newtonsoft.Json;
using Spines.Hana.Blame.Data;
using Spines.Hana.Blame.Models;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  public class ReplayManager
  {
    private readonly ApplicationDbContext _context;
    private readonly StorageOptions _storage;

    public ReplayManager(ApplicationDbContext context, IOptions<StorageOptions> storageOptions)
    {
      _context = context;
      _storage = storageOptions.Value;
    }

    private static readonly ConcurrentDictionary<string, string> DummyDatabase = new ConcurrentDictionary<string, string>();
    private static readonly ConcurrentQueue<string> DownloadHistory = new ConcurrentQueue<string>();
    private static readonly ConcurrentQueue<string> Errors = new ConcurrentQueue<string>();

    private static async Task<string> DownloadAsync(string replayId)
    {
      // Use replay downloader class
      var xml = await Task.FromResult("");
      DownloadHistory.Enqueue(replayId);
      return xml;
    }

    private async Task<bool> MatchExists(string replayId)
    {
      return await Task.FromResult(DummyDatabase.TryGetValue(replayId, out var t));
      //return await _context.Matches.AnyAsync(m => m.ContainerName == TenhouStorageContainerName && m.FileName == replayId);
    }

    //private static readonly TimeSpan Delay = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(10);
    private static DateTime _lastDownloadTime = DateTime.MinValue;
    private const string TenhouStorageContainerName = "tenhoureplays";
    private static readonly Regex ReplayIdRegex = new Regex(@"\A(\d{10})gm-\d{4}-\d{4}-[\da-f]{8}\z");
    private static readonly ConcurrentDictionary<string, Task<bool>> CurrentWork = new ConcurrentDictionary<string, Task<bool>>();
    private static readonly SemaphoreSlim TenhouSemaphore = new SemaphoreSlim(1, 1);

    public async Task<bool> PrepareAsync(string replayId)
    {
      // Deny invalid IDs.
      if (string.IsNullOrEmpty(replayId) || !ReplayIdRegex.IsMatch(replayId))
      {
        return false;
      }
      // If the ID is currently being downloaded, wait on the existing task.
      if (CurrentWork.TryGetValue(replayId, out var t))
      {
        return await t;
      }
      // If the replay already exists in the DB, no need to do anything.
      if (await MatchExists(replayId))
      {
        return true;
      }
      // Try to queue a new download. If it was queued, await that.
      var work = QueuedDownload(replayId);
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
      return await MatchExists(replayId);
    }

    private async Task<bool> QueuedDownload(string replayId)
    {
      // Synchronize downloads.
      await TenhouSemaphore.WaitAsync();
      try
      {
        // If the replay was downloaded by now, we are done.
        if (await MatchExists(replayId))
        {
          return true;
        }

        // Wait until at least the delay has passed since the last request.
        var nextDownloadTime = _lastDownloadTime + Delay;
        var current = DateTime.UtcNow;
        if (current < nextDownloadTime)
        {
          await Task.Delay(nextDownloadTime - current);
        }

        // Else we are sending out a request and remeber the time of that request.
        var xml = await DownloadAsync(replayId);
        _lastDownloadTime = DateTime.UtcNow;

        // If the download was unsuccessful, the ID is invalid.
        if (xml == null)
        {
          return false;
        }

        // Save the downloaded replay.
        if (!DummyDatabase.TryAdd(replayId, replayId))
        {
          Errors.Enqueue(replayId + " already in dummy DB");
        }

        //var replay = Replay.Parse(xml);
        //await Task.WhenAll(SaveToDatabase(replayId, replay), SaveToStorage(replayId, replay));
        //await SaveToDatabase(replayId, replay);
        //await SaveToStorage(replayId, replay);
      }
      finally
      {
        CurrentWork.TryRemove(replayId, out var t);
        TenhouSemaphore.Release();
      }
      return true;
    }

    private async Task SaveToDatabase(string replayId, Replay replay)
    {
      var rulesName = replay.Rules.Name;
      var ruleSet = await _context.RuleSets.FirstAsync(r => r.Name == rulesName);

      var roomName = replay.Room.Name;
      var room = await _context.Rooms.FirstAsync(r => r.Name == roomName);

      var seat = 0;
      var participants = new List<Participant>();
      foreach (var player in replay.Players)
      {
        var p = await GetOrCreatePlayer(player.Name);
        var points = replay.Owari.Points[seat];
        var score = replay.Owari.Scores[seat];
        var placement = replay.Owari.Points.Count(x => x > points) + 1;
        var participant = new Participant
        {
          Seat = seat,
          Player = p,
          Score = score,
          Points = points,
          Placement = placement,
          Rank = player.Rank,
          Rate = player.Rate,
          Gender = player.Gender
        };
        participants.Add(participant);
        seat += 1;
      }

      var games = replay.Games.Select((g, i) => new Models.Game { Index = i, FrameCount = g.Actions.Count });
      var match = new Models.Match(games, participants);
      match.ContainerName = TenhouStorageContainerName;
      match.FileName = replayId;
      match.UploadTime = DateTime.UtcNow;
      match.CreationTime = GetReplayCreationTIme(replayId);
      match.RuleSet = ruleSet;
      match.Room = room;
      match.Lobby = replay.Lobby;
      await _context.Matches.AddAsync(match);
      await _context.SaveChangesAsync();
    }

    private async Task<Models.Player> GetOrCreatePlayer(string name)
    {
      var p = await _context.Players.FirstOrDefaultAsync(x => x.Name == name);
      return p ?? (await _context.Players.AddAsync(new Models.Player { Name = name })).Entity;
    }

    private async Task SaveToStorage(string replayId, Replay replay)
    {
      var json = JsonConvert.SerializeObject(replay);
      var storageCredentials = new StorageCredentials(_storage.StorageAccountName, _storage.StorageAccountKey);
      var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);
      var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
      var container = cloudBlobClient.GetContainerReference(TenhouStorageContainerName);
      var newBlob = container.GetBlockBlobReference(replayId + ".json");
      await newBlob.UploadTextAsync(json);
    }

    /// <summary>
    /// The first 10 characters of the replayId defined the hour the match was played, in japanese time.
    /// </summary>
    /// <param name="replayId">The ID of the replay.</param>
    /// <returns>The time the replay was created, in UTC.</returns>
    private static DateTime GetReplayCreationTIme(string replayId)
    {
      var timeString = ReplayIdRegex.Match(replayId).Groups[1].Value;
      var dateTime = DateTime.ParseExact(timeString, "yyyyMMddHH", CultureInfo.InvariantCulture);
      var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
      return TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
    }
  }
}
