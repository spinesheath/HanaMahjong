using System.Net.Http;
using System.Threading.Tasks;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  public class TenhouNet : IReplaySource
  {
    private readonly HttpClient _client;

    public TenhouNet(HttpClient client)
    {
      _client = client;
    }

    public async Task<Replay> GetAsync(ReplayId id)
    {
      var response = await _client.GetAsync($"https://tenhou.net/0/log/?{id}");
      if (response.IsSuccessStatusCode)
      {
        var xml = await response.Content.ReadAsStringAsync();
        return Replay.Parse(xml);
      }
      return null;
    }
  }
}