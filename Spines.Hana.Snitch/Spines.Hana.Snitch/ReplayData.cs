// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Hana.Snitch
{
  internal class ReplayData
  {
    public ReplayData(string id, int position)
    {
      Id = id;
      Position = position;
    }

    public string Id { get; }
    public int Position { get; }
  }
}