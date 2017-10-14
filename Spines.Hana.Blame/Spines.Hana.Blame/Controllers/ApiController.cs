// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Newtonsoft.Json;
using Spines.Hana.Blame.Data;
using Spines.Hana.Blame.Models;
using Spines.Hana.Blame.Services.ReplayManager;
using Game = Spines.Hana.Blame.Models.Game;
using Match = Spines.Hana.Blame.Models.Match;
using Player = Spines.Hana.Blame.Models.Player;

namespace Spines.Hana.Blame.Controllers
{
  public class ApiController : Controller
  {
    public ApiController(ApplicationDbContext context, IOptions<StorageOptions> storageOptions)
    {
      _context = context;
      _storage = storageOptions.Value;
    }

    [HttpGet]
    public async Task<IActionResult> Snitch(string replayId)
    {
      if (string.IsNullOrEmpty(replayId) || !ReplayIdRegex.IsMatch(replayId))
      {
        return BadRequest();
      }

      var exists = await MatchExists(replayId);
      if (exists)
      {
        return Json(replayId);
      }

      var replayManager = new ReplayManager();
      var replay = await replayManager.GetReplay(replayId);
      if (replay == null)
      {
        return BadRequest();
      }

      await SaveToDatabase(replayId, replay);
      await SaveToStorage(replayId, replay);

      return Json(replayId);
    }

    private const string TenhouStorageContainerName = "tenhoureplays";
    private static readonly Regex ReplayIdRegex = new Regex(@"\A\d{10}gm-\d{4}-\d{4}-[\da-f]{8}\z");
    private readonly ApplicationDbContext _context;
    private readonly StorageOptions _storage;

    private async Task SaveToDatabase(string replayId, Replay replay)
    {
      var participants = await Task.WhenAll(replay.Players.Select(async (p, i) => new Participant {Seat = i, Player = await GetOrCreatePlayer(p)}));
      var games = replay.Games.Select((g, i) => new Game {Index = i, FrameCount = g.Actions.Count});
      var match = new Match(games, participants);
      match.ContainerName = TenhouStorageContainerName;
      match.FileName = replayId;
      match.UploadTime = DateTime.UtcNow;
      await _context.Matches.AddAsync(match);
      await _context.SaveChangesAsync();
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

    private async Task<bool> MatchExists(string replayId)
    {
      return await _context.Matches.AnyAsync(m => m.ContainerName == TenhouStorageContainerName && m.FileName == replayId);
    }

    private async Task<Player> GetOrCreatePlayer(Services.ReplayManager.Player player)
    {
      var p = await _context.Players.FirstOrDefaultAsync(x => x.Name == player.Name);
      return p ?? (await _context.Players.AddAsync(new Player {Name = player.Name})).Entity;
    }
  }
}