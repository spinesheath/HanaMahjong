// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spines.Hana.Blame.Models.ThreadViewModels;

namespace Spines.Hana.Blame.Controllers
{
  [Authorize]
  public class ThreadController : Controller
  {
    [HttpPost]
    public async Task<IActionResult> PostComment(ThreadViewModel model, string returnUrl = null)
    {
      await Task.Delay(1);
      return PartialView("Comments", new List<string> {"a", "b", "c"});
    }
  }
}