// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Spines.Hana.Blame.Data;
using Spines.Hana.Blame.Options;
using Spines.Hana.Blame.Services;

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
      ViewData["CopyrightYear"] = DateTime.Today.Year;
      ViewData["StorageUrl"] = $"{_storage.StorageUrl}/{StorageContainers.TenhouJson}/";
      ViewData["SnitchUrl"] = $"{_storage.StorageUrl}/{StorageContainers.Binaries}/Spines.Hana.Snitch.exe";
      return View();
    }

    public IActionResult Error()
    {
      ViewData["CopyrightHolder"] = _copyright.CopyrightHolder;
      return View();
    }
    
    private readonly StorageOptions _storage;
    private readonly CopyrightOptions _copyright;
  }
}