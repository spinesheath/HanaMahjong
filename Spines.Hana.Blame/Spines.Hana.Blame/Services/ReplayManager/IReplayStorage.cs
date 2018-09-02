using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  public interface IReplayStorage
  {
    Task<string> ReadXmlAsync(ReplayId id);
    Task<string> ReadJsonAsync(ReplayId id);
    Task SaveJsonAsync(ReplayId id, string data);
    Task SaveXmlAsync(ReplayId id, string data);
    Task<bool> Exists(ReplayId id);
    Task<IEnumerable<ReplayId>> AllIds();
    Task SaveRelationalData(ReplayId id, Replay replay);
  }
}