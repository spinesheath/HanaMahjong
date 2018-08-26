using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Spines.Hana.Blame.Data;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  internal class Storage : IStorage
  {
    public Storage(IOptions<StorageOptions> options)
    {
      var storageCredentials = new StorageCredentials(options.Value.StorageAccountName, options.Value.StorageAccountKey);
      var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);
      _client = cloudStorageAccount.CreateCloudBlobClient();
    }

    public async Task<string> ReadXmlAsync(string fileNameWithoutExtension)
    {
      var container = _client.GetContainerReference(StorageContainers.TenhouXml);
      var xmlBlob = container.GetBlockBlobReference(fileNameWithoutExtension + ".xml");
      return await xmlBlob.DownloadTextAsync();
    }

    public async Task SaveJsonAsync(string fileNameWithoutExtension, string data)
    {
      var container = _client.GetContainerReference(StorageContainers.TenhouJson);
      var newBlob = container.GetBlockBlobReference(fileNameWithoutExtension + ".json");
      await newBlob.UploadTextAsync(data);
    }

    public async Task SaveXmlAsync(string fileNameWithoutExtension, string data)
    {
      var container = _client.GetContainerReference(StorageContainers.TenhouXml);
      var newBlob = container.GetBlockBlobReference(fileNameWithoutExtension + ".xml");
      await newBlob.UploadTextAsync(data);
    }

    private readonly CloudBlobClient _client;
  }
}