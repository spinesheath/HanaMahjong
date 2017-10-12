// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Hana.Blame.Data
{
  /// <summary>
  /// Settings for cloud storage.
  /// </summary>
  public class StorageOptions
  {
    /// <summary>
    /// Name of the storage account.
    /// </summary>
    public string StorageAccountName { get; set; }

    /// <summary>
    /// Key of the storage account.
    /// </summary>
    public string StorageAccountKey { get; set; }

    /// <summary>
    /// Url of the storage.
    /// </summary>
    public string StorageUrl { get; set; }
  }
}