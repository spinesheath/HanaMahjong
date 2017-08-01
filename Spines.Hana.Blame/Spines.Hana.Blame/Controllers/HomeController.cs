// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Spines.Hana.Blame.Models.ThreadViewModels;

namespace Spines.Hana.Blame.Controllers
{
  public class HomeController : Controller
  {
    public IActionResult Index()
    {
      var r = new Random(DateTime.Now.Millisecond);
      var count = r.Next(5, 10);
      var values = Enumerable.Range(0, count).Select(x => r.Next());
      ViewData["Thread"] = new ThreadViewModel(values.Select(c => c.ToString()));
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
  }
}