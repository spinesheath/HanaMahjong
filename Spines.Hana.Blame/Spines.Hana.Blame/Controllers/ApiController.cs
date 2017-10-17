// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Spines.Hana.Blame.Services.ReplayManager;

namespace Spines.Hana.Blame.Controllers
{
  public class ApiController : Controller
  {
    public ApiController(ReplayManager replayManager)
    {
      _replayManager = replayManager;
    }

    [HttpGet]
    public async Task<IActionResult> Snitch(string replayId)
    {
      var success = await _replayManager.PrepareAsync(replayId);
      if (success)
      {
        return Json(replayId);
      }
      return BadRequest();
    }

    private readonly ReplayManager _replayManager;
  }
}