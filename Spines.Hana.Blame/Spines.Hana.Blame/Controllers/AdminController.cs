// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spines.Hana.Blame.Data;
using Spines.Hana.Blame.Models.AdminViewModels;

namespace Spines.Hana.Blame.Controllers
{
  [Authorize(Roles = RoleNames.Admin)]
  public class AdminController : Controller
  {
    public AdminController(ApplicationDbContext context)
    {
      _context = context;
    }

    public IActionResult Index()
    {
      var userNames = _context.Users.Select(u => u.UserName).ToList();
      var threadCount = _context.Threads.Count();
      var commentCount = _context.Comments.Count();
      var viewModel = new IndexViewModel(userNames, threadCount, commentCount);
      return View(viewModel);
    }

    private readonly ApplicationDbContext _context;
  }
}