// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Spines.Hana.Blame.Data;
using Spines.Hana.Blame.Models;

namespace Spines.Hana.Blame.Controllers
{
  public class HomeController : Controller
  {
    public HomeController(IOptions<CopyrightOptions> copyrightOptions, IOptions<StorageOptions> storageOptions)
    {
      _storage = storageOptions.Value;
      _copyright = copyrightOptions.Value;
    }

    public IActionResult Index()
    {
      ViewData["CopyrightHolder"] = _copyright.CopyrightHolder;
      ViewData["StorageUrl"] = _storage.StorageUrl;
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

    private readonly StorageOptions _storage;

    private readonly CopyrightOptions _copyright;
  }
}