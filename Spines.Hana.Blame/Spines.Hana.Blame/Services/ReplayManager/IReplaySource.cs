using System.Threading.Tasks;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  public interface IReplaySource
  {
    Task<Replay> GetAsync(ReplayId id);
  }
}