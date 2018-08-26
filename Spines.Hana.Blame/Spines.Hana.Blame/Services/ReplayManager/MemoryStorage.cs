using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  internal class MemoryStorage : IStorage
  {
    public async Task<string> ReadXmlAsync(string fileNameWithoutExtension)
    {
      return await Task.FromResult(_xmlFiles.ContainsKey(fileNameWithoutExtension) ? _xmlFiles[fileNameWithoutExtension] : null);
    }

    public async Task SaveJsonAsync(string fileNameWithoutExtension, string data)
    {
      await Task.Yield();
    }

    public async Task SaveXmlAsync(string fileNameWithoutExtension, string data)
    {
      _xmlFiles[fileNameWithoutExtension] = data;
      await Task.Yield();
    }

    private readonly Dictionary<string, string> _xmlFiles = new Dictionary<string, string>();
  }
}