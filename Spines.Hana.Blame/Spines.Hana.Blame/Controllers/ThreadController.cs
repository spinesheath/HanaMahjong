// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spines.Hana.Blame.Data;
using Spines.Hana.Blame.Models;
using Spines.Hana.Blame.Models.ThreadViewModels;
using Spines.Hana.Blame.Services.ReplayManager;
using FrameComment = Spines.Hana.Blame.Models.ThreadViewModels.FrameComment;

namespace Spines.Hana.Blame.Controllers
{
  [Authorize(Roles = RoleNames.CommonUser)]
  public class ThreadController : Controller
  {
    public ThreadController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ReplayManager replayManager)
    {
      _context = context;
      _userManager = userManager;
      _replayManager = replayManager;
    }

    private async Task<IEnumerable<FrameComment>> GetComments(Match match, ApplicationUser user)
    {
      var comments = await _context.FrameComments.Where(c => c.Match == match).Include(c => c.User).ToListAsync();
      return comments.Select(c =>
        new FrameComment
        {
          Message = HttpUtility.HtmlEncode(c.Message),
          FrameId = c.FrameIndex,
          GameId = c.GameIndex,
          Timestamp = c.Time,
          UserName = HttpUtility.HtmlEncode(c.User.UserName),
          PlayerId = c.SeatIndex,
          Editable = c.User == user,
          Id = c.Id
        });
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Comments(string replayId)
    {
      var thread = new MatchComments { ReplayId = replayId, Comments = new List<FrameComment>() };
      if (!_replayManager.IsValidId(replayId))
      {
        return Json(thread);
      }
      var match = await _context.Matches.Include(m => m.Participants).FirstOrDefaultAsync(m => m.FileName == replayId);
      if (match == null)
      {
        return Json(thread);
      }
      var user = await _userManager.GetUserAsync(HttpContext.User);
      var comments = await GetComments(match, user);
      thread.Comments.AddRange(comments);
      return Json(thread);
    }

    [HttpPost]
    public async Task<IActionResult> Comment(CreateFrameComment comment)
    {
      if (!_replayManager.IsValidId(comment.ReplayId))
      {
        return BadRequest();
      }
      if (string.IsNullOrWhiteSpace(comment.Message))
      {
        return BadRequest();
      }
      if (comment.FrameId < 0 || comment.GameId < 0 || comment.PlayerId < 0)
      {
        return BadRequest();
      }
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null)
      {
        return BadRequest();
      }
      var match = await _context.Matches.Include(m => m.Participants).Include(m => m.Games).FirstOrDefaultAsync(m => m.FileName == comment.ReplayId);
      if (match == null)
      {
        return BadRequest();
      }
      var participant = match.Participants.FirstOrDefault(p => p.Seat == comment.PlayerId);
      if (participant == null)
      {
        return BadRequest();
      }
      var game = match.Games.FirstOrDefault(g => g.Index == comment.GameId);
      if (game == null)
      {
        return BadRequest();
      }
      if (comment.FrameId >= game.FrameCount)
      {
        return BadRequest();
      }

      var frameComment = new Models.FrameComment();
      frameComment.Message = HttpUtility.HtmlEncode(comment.Message);
      frameComment.Time = DateTime.UtcNow;
      frameComment.User = user;
      frameComment.FrameIndex = comment.FrameId;
      frameComment.GameIndex = comment.GameId;
      frameComment.SeatIndex = comment.PlayerId;
      frameComment.Match = match;

      await _context.Comments.AddAsync(frameComment);
      await _context.SaveChangesAsync();

      var resultThread = new MatchComments { ReplayId = comment.ReplayId, Comments = new List<FrameComment>() };
      var comments = await GetComments(match, user);
      resultThread.Comments.AddRange(comments);

      return Json(resultThread);
    }

    [HttpPost]
    public async Task<IActionResult> Remove(long id)
    {
      if (id < 0)
      {
        return BadRequest();
      }
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null)
      {
        return BadRequest();
      }
      var comment = await _context.FrameComments.Include(f => f.Match).FirstOrDefaultAsync(c => c.Id == id && c.User == user);
      if (comment == null)
      {
        return BadRequest();
      }

      var match = comment.Match;

      _context.Comments.Remove(comment);
      await _context.SaveChangesAsync();

      var resultThread = new MatchComments { ReplayId = match.FileName, Comments = new List<FrameComment>() };
      var comments = await GetComments(match, user);
      resultThread.Comments.AddRange(comments);

      return Json(resultThread);
    }

    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ReplayManager _replayManager;
  }
}