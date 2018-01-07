// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spines.Hana.Blame.Data;
using Spines.Hana.Blame.Models;
using Spines.Hana.Blame.Models.AdminViewModels;

namespace Spines.Hana.Blame.Controllers
{
  [Authorize(Roles = RoleNames.Admin)]
  public class AdminController : Controller
  {
    public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
      _context = context;
      _userManager = userManager;
    }

    public IActionResult Index(string returnUrl = null)
    {
      var userCount = _context.Users.Count();
      var commentCount = _context.Comments.Count();
      var viewModel = new IndexViewModel {UserCount = userCount, CommentCount = commentCount};
      return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> MakeCommonUser(IndexViewModel data)
    {
      if (string.IsNullOrEmpty(data.SelectedUserName))
      {
        return BadRequest();
      }
      var user = await _userManager.FindByNameAsync(data.SelectedUserName);
      if (null == user)
      {
        return BadRequest();
      }
      var roleResult = await _userManager.AddToRoleAsync(user, RoleNames.CommonUser);
      if (roleResult.Succeeded)
      {
        return RedirectToAction("Index");
      }
      return BadRequest();
    }

    [HttpPost]
    public async Task<IActionResult> RemoveCommonUser(IndexViewModel data)
    {
      if (string.IsNullOrEmpty(data.SelectedUserName))
      {
        return BadRequest();
      }
      var user = await _userManager.FindByNameAsync(data.SelectedUserName);
      if (null == user)
      {
        return BadRequest();
      }
      var roleResult = await _userManager.RemoveFromRoleAsync(user, RoleNames.CommonUser);
      if (roleResult.Succeeded)
      {
        return RedirectToAction("Index");
      }
      return BadRequest();
    }

    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
  }
}