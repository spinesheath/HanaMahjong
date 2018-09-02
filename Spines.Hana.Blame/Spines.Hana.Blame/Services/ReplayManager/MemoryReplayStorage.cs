using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  internal class MemoryReplayStorage : IReplayStorage
  {
    public async Task<string> ReadXmlAsync(ReplayId id)
    {
      var replayId = id.ToString();
      return await Task.FromResult(_xmlFiles.ContainsKey(replayId) ? _xmlFiles[replayId] : null);
    }

    public async Task<string> ReadJsonAsync(ReplayId id)
    {
      var replayId = id.ToString();
      return await Task.FromResult(_jsonFiles.ContainsKey(replayId) ? _jsonFiles[replayId] : null);
    }

    public async Task SaveJsonAsync(ReplayId id, string data)
    {
      var replayId = id.ToString();
      _jsonFiles[replayId] = data;
      await Task.Yield();
    }

    public async Task SaveXmlAsync(ReplayId id, string data)
    {
      var replayId = id.ToString();
      _xmlFiles[replayId] = data;
      await Task.Yield();
    }

    public async Task<bool> Exists(ReplayId id)
    {
      return await Task.FromResult(_allIds.Contains(id.ToString()));
    }

    public async Task<IEnumerable<ReplayId>> AllIds()
    {
      return await Task.FromResult(_allIds.Select(id => new ReplayId(id)));
    }

    public async Task SaveRelationalData(ReplayId id, Replay replay)
    {
      _allIds.Add(id.ToString());
      await Task.Yield();
    }

    private readonly Dictionary<string, string> _xmlFiles = new Dictionary<string, string>();
    private readonly Dictionary<string, string> _jsonFiles = new Dictionary<string, string>();
    private readonly HashSet<string> _allIds = new HashSet<string>();
  }
}