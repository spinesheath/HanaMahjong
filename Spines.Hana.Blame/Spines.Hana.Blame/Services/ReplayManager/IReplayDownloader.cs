// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  internal interface IReplayDownloader
  {
    Task<string> DownloadAsync(string id);
  }
}