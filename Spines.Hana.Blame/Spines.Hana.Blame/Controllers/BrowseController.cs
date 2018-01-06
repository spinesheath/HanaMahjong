// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spines.Hana.Blame.Data;
using Spines.Hana.Blame.Models.BrowseViewModels;

namespace Spines.Hana.Blame.Controllers
{
  public class BrowseController : Controller
  {
    private readonly ApplicationDbContext _context;

    public BrowseController(ApplicationDbContext context)
    {
      _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> GetReplays(SearchParameters parameters)
    {
      var playerName = parameters.PlayerName;
      if (string.IsNullOrEmpty(playerName))
      {
        return BadRequest();
      }
      var participants = _context.Players.Where(p => p.Name.Contains(playerName)).SelectMany(p => p.Participants);
      var matches = _context.Matches.Join(participants, m => m.Id, p => p.MatchId, (m, p) => m);
      var data = await matches.Join(_context.Participants, m => m.Id, p => p.MatchId, (m, p) => new { m.FileName, m.CreationTime, p.Player.Name, p.Seat}).ToListAsync();
      var replays = data.GroupBy(m => m.FileName).Select(g => new ReplayViewModel { Id = g.Key, Timestamp = g.First().CreationTime, Participants = g.OrderBy(x => x.Seat).Select(x => x.Name).ToList()});
      return Json(replays);
    }
  }
}