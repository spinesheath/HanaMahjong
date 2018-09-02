using System.Threading.Tasks;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  public class MemoryReplaySource : IReplaySource
  {
    public async Task<Replay> GetAsync(ReplayId id)
    {
      return await Task.FromResult(Replay.Null());
    }
  }
}