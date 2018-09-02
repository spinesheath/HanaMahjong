using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Spines.Hana.Blame.Data;
using Spines.Hana.Blame.Models;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  internal class AzureCloudReplayStorage : IReplayStorage
  {
    private readonly ApplicationDbContext _context;
    
    public AzureCloudReplayStorage(IOptions<StorageOptions> options, ApplicationDbContext context)
    {
      _context = context;
      var optionsValue = options.Value;
      var storageCredentials = new StorageCredentials(optionsValue.StorageAccountName, optionsValue.StorageAccountKey);
      var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);
      _client = cloudStorageAccount.CreateCloudBlobClient();
    }

    public async Task<string> ReadXmlAsync(ReplayId id)
    {
      var replayId = id.ToString();
      var blob = GetBlob(StorageContainers.TenhouXml, replayId + ".xml");
      return await blob.DownloadTextAsync();
    }

    public async Task<string> ReadJsonAsync(ReplayId id)
    {
      var replayId = id.ToString();
      var blob = GetBlob(StorageContainers.TenhouJson, replayId + ".json");
      return await blob.DownloadTextAsync();
    }

    public async Task SaveXmlAsync(ReplayId id, string data)
    {
      var replayId = id.ToString();
      var blob = GetBlob(StorageContainers.TenhouXml, replayId + ".xml");
      await blob.UploadTextAsync(data);
    }

    public async Task<bool> Exists(ReplayId id)
    {
      var replayId = id.ToString();
      return await _context.Matches.AnyAsync(m => m.ContainerName == StorageContainers.TenhouJson && m.FileName == replayId);
    }

    public async Task<IEnumerable<ReplayId>> AllIds()
    {
      var allIds = await _context.Matches.Select(m => m.FileName).ToListAsync();
      return allIds.Select(id => new ReplayId(id));
    }

    public async Task SaveRelationalData(ReplayId id, Replay replay)
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
      var match = new Match(games, participants);
      match.ContainerName = StorageContainers.TenhouJson;
      match.FileName = id.ToString();
      match.UploadTime = DateTime.UtcNow;
      match.CreationTime = id.CreationTime;
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

    public async Task SaveJsonAsync(ReplayId id, string data)
    {
      var replayId = id.ToString();
      var blob = GetBlob(StorageContainers.TenhouJson, replayId + ".json");
      await blob.UploadTextAsync(data);
    }

    private CloudBlockBlob GetBlob(string containerName, string fileName)
    {
      var container = _client.GetContainerReference(containerName);
      return container.GetBlockBlobReference(fileName);
    }

    private readonly CloudBlobClient _client;
  }
}