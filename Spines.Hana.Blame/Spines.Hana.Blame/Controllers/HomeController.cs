// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Spines.Hana.Blame.Data;
using Spines.Hana.Blame.Options;

namespace Spines.Hana.Blame.Controllers
{
  public class HomeController : Controller
  {
    public HomeController(IOptions<CopyrightOptions> copyrightOptions, IOptions<StorageOptions> storageOptions, IOptions<SnitchOptions> snitchOptions)
    {
      _copyright = copyrightOptions.Value;
      _snitch = snitchOptions.Value;
    }

    public IActionResult Index()
    {
      ViewData["CopyrightHolder"] = _copyright.CopyrightHolder;
      ViewData["CopyrightYear"] = DateTime.Today.Year;
      ViewData["SnitchUrl"] = _snitch.SnitchUrl;
      return View();
    }

    public IActionResult Error()
    {
      ViewData["CopyrightHolder"] = _copyright.CopyrightHolder;
      return View();
    }
    
    private readonly CopyrightOptions _copyright;
    private readonly SnitchOptions _snitch;
  }
}