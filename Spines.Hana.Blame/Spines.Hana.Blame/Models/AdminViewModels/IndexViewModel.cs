// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace Spines.Hana.Blame.Models.AdminViewModels
{
  public class IndexViewModel
  {
    public IndexViewModel(IEnumerable<string> userNames, int threadCount, int commentCount)
    {
      var userList = userNames.ToList();
      UserNames = userList;
      UserCount = userList.Count;
      ThreadCount = threadCount;
      CommentCount = commentCount;
    }

    public int UserCount { get; }
    public IEnumerable<string> UserNames { get; }
    public int ThreadCount { get; }
    public int CommentCount { get; }
  }
}