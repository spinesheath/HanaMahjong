// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Spines.Hana.Blame.Data;
using Spines.Hana.Blame.Models;
using Match = Spines.Hana.Blame.Models.Match;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  public class ReplayManager
  {
    public ReplayManager(ApplicationDbContext context, IStorage storage, HttpClient client, ILoggerFactory loggerFactory)
    {
      _context = context;
      _client = client;
      _logger = loggerFactory.CreateLogger<ReplayManager>();
      _storage = storage;
    }

    public async Task<HttpStatusCode> PrepareAsync(string replayId)
    {
      // Deny invalid IDs.
      if (!IsValidId(replayId))
      {
        return HttpStatusCode.BadRequest;
      }

      // If the ID is currently being downloaded, wait on the existing task.
      if (CurrentWork.TryGetValue(replayId, out var t))
      {
        return await t;
      }

      // If the replay already exists in the DB, no need to do anything.
      if (await MatchExists(replayId))
      {
        return HttpStatusCode.NoContent;
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
      return await MatchExists(replayId) ? HttpStatusCode.NoContent : HttpStatusCode.NotFound;
    }

    public bool IsValidId(string replayId)
    {
      return !string.IsNullOrEmpty(replayId) && ReplayIdRegex.IsMatch(replayId);
    }

    public async Task ReparseReplays()
    {
      var replayIds = await _context.Matches.Select(m => m.FileName).ToListAsync();
      foreach (var replayId in replayIds)
      {
        var xml = await _storage.ReadXmlAsync(replayId);
        var replay = Replay.Parse(xml);
        var json = JsonConvert.SerializeObject(replay);
        await _storage.SaveJsonAsync(replayId, json);
      }
    }

    private readonly ApplicationDbContext _context;
    private readonly HttpClient _client;
    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(5);
    private static readonly Regex ReplayIdRegex = new Regex(@"\A(\d{10})gm-[\da-f]{4}-[\da-f]{4}-[\da-f]{8}\z");
    private static readonly ConcurrentDictionary<string, Task<HttpStatusCode>> CurrentWork = new ConcurrentDictionary<string, Task<HttpStatusCode>>();
    private static readonly SemaphoreSlim TenhouSemaphore = new SemaphoreSlim(1, 1);
    private readonly ILogger<ReplayManager> _logger;
    private readonly IStorage _storage;

    private async Task<bool> MatchExists(string replayId)
    {
      return await _context.Matches.AnyAsync(m => m.ContainerName == StorageContainers.TenhouJson && m.FileName == replayId);
    }

    private async Task<HttpStatusCode> QueuedDownload(string replayId)
    {
      await TenhouSemaphore.WaitAsync();
      try
      {
        // If the replay was downloaded by now, we are done.
        if (await MatchExists(replayId))
        {
          return HttpStatusCode.NoContent;
        }

        // Wait for a while to let tenhou make the replay available.
        // Also prevents too many replays being requested from tenhou at once.
        await Task.Delay(Delay);

        var response = await _client.GetAsync($"http://e.mjv.jp/0/log/?{replayId}");
        if (response.IsSuccessStatusCode)
        {
          var xml = await response.Content.ReadAsStringAsync();
          if (string.IsNullOrEmpty(xml))
          {
            return HttpStatusCode.NotFound;
          }

          // Save the downloaded replay.
          var replay = Replay.Parse(xml);
          var json = JsonConvert.SerializeObject(replay);
          await Task.WhenAll(
            SaveToDatabase(replayId, replay),
            _storage.SaveJsonAsync(replayId, json),
            _storage.SaveXmlAsync(replayId, xml));
        }
        else
        {
          return response.StatusCode;
        }
      }
      catch (Exception e)
      {
        _logger.LogError(e, "Failed to download or store replay.");
        return HttpStatusCode.InternalServerError;
      }
      finally
      {
        CurrentWork.TryRemove(replayId, out var unused);
        TenhouSemaphore.Release();
      }

      return HttpStatusCode.NoContent;
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

      var games = replay.Games.Select((g, i) => new Models.Game {Index = i, FrameCount = g.Actions.Count});
      var match = new Match(games, participants);
      match.ContainerName = StorageContainers.TenhouJson;
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
      return p ?? (await _context.Players.AddAsync(new Models.Player {Name = name})).Entity;
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