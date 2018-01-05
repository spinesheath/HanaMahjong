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
using Game = Spines.Hana.Blame.Models.Game;

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

    private async Task<FrameThread> GetFrameThread(CreateFrameComment comment, Match match, Game game, Participant participant)
    {
      var thread = await _context.FrameThreads.FirstOrDefaultAsync(t => 
        t.Match == match && 
        t.Game == game &&
        t.FrameId == comment.FrameId && 
        t.Participant == participant);
      return thread ?? await CreateThreadAsync(comment, match, game, participant);
    }

    private async Task<FrameThread> CreateThreadAsync(CreateFrameComment comment, Match match, Game game, Participant participant)
    {
      var thread = new FrameThread();
      thread.Match = match;
      thread.Game = game;
      thread.FrameId = comment.FrameId;
      thread.Participant = participant;
      var result = await _context.FrameThreads.AddAsync(thread);
      return result.Entity;
    }

    private async Task<IEnumerable<FrameComment>> GetComments(Match match)
    {
      var threads = await _context.FrameThreads.Where(t => t.Match == match).Include(t => t.Game).Include(t => t.Comments).ThenInclude(c => c.User).ToListAsync();
      var frameComments = threads.SelectMany(t => t.Comments.Select(c =>
        new FrameComment
        {
          Message = HttpUtility.HtmlEncode(c.Message),
          FrameId = t.FrameId,
          GameId = t.Game.Index,
          Timestamp = c.Time,
          UserName = HttpUtility.HtmlEncode(c.User.UserName),
          PlayerId = t.Participant.Seat
        }));
      return frameComments;
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
      if (null == match)
      {
        return Json(thread);
      }
      var comments = await GetComments(match);
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
      if (null == user)
      {
        return BadRequest();
      }
      var match = await _context.Matches.Include(m => m.Participants).Include(m => m.Games).FirstOrDefaultAsync(m => m.FileName == comment.ReplayId);
      if (null == match)
      {
        return BadRequest();
      }
      var participant = match.Participants.FirstOrDefault(p => p.Seat == comment.PlayerId);
      if (null == participant)
      {
        return BadRequest();
      }
      var game = match.Games.FirstOrDefault(g => g.Index == comment.GameId);
      if (null == game)
      {
        return BadRequest();
      }
      if (comment.FrameId > game.FrameCount)
      {
        return BadRequest();
      }

      var thread = await GetFrameThread(comment, match, game, participant);

      var frameComment = new Comment();
      frameComment.Thread = thread;
      frameComment.Message = HttpUtility.HtmlEncode(comment.Message);
      frameComment.Time = DateTime.UtcNow;
      frameComment.User = user;

      await _context.Comments.AddAsync(frameComment);
      await _context.SaveChangesAsync();

      var resultThread = new MatchComments { ReplayId = comment.ReplayId, Comments = new List<FrameComment>() };
      var comments = await GetComments(match);
      resultThread.Comments.AddRange(comments);

      return Json(resultThread);
    }

    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ReplayManager _replayManager;
  }
}