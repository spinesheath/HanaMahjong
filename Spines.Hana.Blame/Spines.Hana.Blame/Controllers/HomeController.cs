// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Spines.Hana.Blame.Models;
using Spines.Hana.Blame.Services.ReplayManager;

namespace Spines.Hana.Blame.Controllers
{
  public class HomeController : Controller
  {
    public HomeController(IOptions<CopyrightOptions> optionsAccessor)
    {
      _options = optionsAccessor.Value;
    }

    public IActionResult Index()
    {
      var replayManager = new ReplayManager();
      var replay = replayManager.GetReplay("x");
      ViewData["GameData"] = replay.Data;
      ViewData["CopyrightHolder"] = _options.CopyrightHolder;
      return View();
    }

    public IActionResult Contact()
    {
      ViewData["Message"] = "Your contact page.";

      return View();
    }

    public IActionResult Error()
    {
      return View();
    }

    public IActionResult GetThread(string hand)
    {
      return ViewComponent("Thread", hand);
    }

    private readonly CopyrightOptions _options;
  }
}