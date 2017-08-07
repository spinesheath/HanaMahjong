// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spines.Hana.Blame.Data;

namespace Spines.Hana.Blame.Models.ThreadViewModels
{
  internal class ThreadManager
  {
    public ThreadManager(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<IEnumerable<CommentViewModel>> GetCommentViewModelsAsync(string normalizedHand)
    {
      var selectMany = _context.WwydThreads.Where(t => t.Hand == normalizedHand).SelectMany(t => t.Comments);
      var includableQueryable = selectMany.Select(c => new {message = c.Message, time = c.Time, userName = c.User.UserName});
      var comments = await includableQueryable.ToListAsync();
      return comments.Select(c => new CommentViewModel {Message = c.message, Time = c.time, UserName = c.userName});
    }

    private readonly ApplicationDbContext _context;
  }
}