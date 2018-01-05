// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Hana.Blame.Models.AdminViewModels
{
  public class IndexViewModel
  {
    public int UserCount { get; set; }
    public int ThreadCount { get; set; }
    public int CommentCount { get; set; }
    public string SelectedUserName { get; set; }
  }
}