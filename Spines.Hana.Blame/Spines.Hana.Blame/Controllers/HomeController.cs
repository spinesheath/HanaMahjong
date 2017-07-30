// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Mvc;
using Spines.Hana.Blame.Models.Wwyd;

namespace Spines.Hana.Blame.Controllers
{
  public class HomeController : Controller
  {
    public IActionResult Index()
    {
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

    public IActionResult Wwyd(string h)
    {
      var hand = WwydHand.Parse(h);
      if (hand.IsValid)
      {
        ViewData["Hand"] = hand.NormalizedRepresentation;
      }

      return View();
    }
  }
}