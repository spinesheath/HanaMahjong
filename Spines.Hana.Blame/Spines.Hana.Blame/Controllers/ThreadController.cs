// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spines.Hana.Blame.Data;
using Spines.Hana.Blame.Models;
using Spines.Hana.Blame.Models.ThreadViewModels;
using Spines.Hana.Blame.Models.Wwyd;

namespace Spines.Hana.Blame.Controllers
{
  [Authorize]
  public class ThreadController : Controller
  {
    public ThreadController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
      _context = context;
      _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> PostComment(ThreadViewModel model)
    {
      var hand = model?.Hand;
      if (hand == null || string.IsNullOrEmpty(model.Message))
      {
        return StatusCode(StatusCodes.Status400BadRequest);
      }
      var parsed = WwydHand.Parse(hand);
      if (!parsed.IsValid)
      {
        return StatusCode(StatusCodes.Status400BadRequest);
      }
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (null == user)
      {
        return StatusCode(StatusCodes.Status400BadRequest);
      }

      var normalizedHand = parsed.NormalizedRepresentation;
      var thread = await GetThread(normalizedHand);

      var comment = new Comment();
      comment.Thread = thread;
      comment.Message = model.Message;
      comment.Time = DateTime.UtcNow;
      comment.User = user;

      await _context.Comments.AddAsync(comment);
      await _context.SaveChangesAsync();

      var comments = await _context.WwydThreads.Where(t => t.Hand == normalizedHand).SelectMany(t => t.Comments).ToListAsync();
      return PartialView("Comments", comments.Select(c => new CommentViewModel{Message = c.Message, Time = c.Time, UserName = c.User.UserName}).ToList());
    }

    private async Task<WwydThread> GetThread(string normalizedHand)
    {
      var thread = await _context.WwydThreads.FirstOrDefaultAsync(t => t.Hand == normalizedHand);
      return thread ?? await CreateThreadAsync(normalizedHand);
    }

    private async Task<WwydThread> CreateThreadAsync(string normalizedHand)
    {
      var thread = new WwydThread();
      thread.Hand = normalizedHand;
      var result = await _context.WwydThreads.AddAsync(thread);
      return result.Entity;
    }

    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
  }
}