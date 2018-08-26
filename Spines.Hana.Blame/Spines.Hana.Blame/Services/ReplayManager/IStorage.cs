using System.Threading.Tasks;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  public interface IStorage
  {
    Task<string> ReadXmlAsync(string fileNameWithoutExtension);
    Task SaveJsonAsync(string fileNameWithoutExtension, string data);
    Task SaveXmlAsync(string fileNameWithoutExtension, string data);
  }
}