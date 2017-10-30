// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Spines.Hana.Blame.Data;
using Spines.Hana.Blame.Models;

namespace Spines.Hana.Blame.Controllers
{
  public class HomeController : Controller
  {
    public HomeController(IOptions<CopyrightOptions> copyrightOptions, IOptions<StorageOptions> storageOptions, ApplicationDbContext context)
    {
      _context = context;
      _storage = storageOptions.Value;
      _copyright = copyrightOptions.Value;
    }

    public async Task<IActionResult> Index()
    {
      ViewData["CopyrightHolder"] = _copyright.CopyrightHolder;
      ViewData["CopyrightYear"] = DateTime.Today.Year;
      ViewData["StorageUrl"] = _storage.StorageUrl;
      var fileNames = await _context.Matches.OrderByDescending(r => r.CreationTime).Select(r => r.FileName).Take(10).ToArrayAsync();
      ViewData["ReplayIds"] = fileNames;
      return View();
    }

    public IActionResult Error()
    {
      ViewData["CopyrightHolder"] = _copyright.CopyrightHolder;
      return View();
    }

    public IActionResult GetThread(string hand)
    {
      return ViewComponent("Thread", hand);
    }

    private readonly ApplicationDbContext _context;

    private readonly StorageOptions _storage;

    private readonly CopyrightOptions _copyright;
  }
}