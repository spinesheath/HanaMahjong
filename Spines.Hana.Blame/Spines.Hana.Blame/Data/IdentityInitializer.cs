// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Spines.Hana.Blame.Models;

namespace Spines.Hana.Blame.Data
{
  public class IdentityInitializer
  {
    public IdentityInitializer(IOptions<InitializeIdentityOptions> optionsAccessor, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
      _options = optionsAccessor.Value;
      _userManager = userManager;
      _roleManager = roleManager;
    }

    public async Task Seed()
    {
      var user = await _userManager.FindByEmailAsync(_options.RootAdminEmail);
      if (user == null)
      {
        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
          var role = new IdentityRole("Admin");
          role.Claims.Add(new IdentityRoleClaim<string> {ClaimType = "IsAdmin", ClaimValue = "True"});
          await _roleManager.CreateAsync(role);
        }

        user = new ApplicationUser
        {
          UserName = _options.RootAdminEmail,
          Email = _options.RootAdminEmail
        };

        var userResult = await _userManager.CreateAsync(user, _options.RootAdminPassword);
        var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
        var claimResult = await _userManager.AddClaimAsync(user, new Claim("SuperUser", "True"));
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmResult = await _userManager.ConfirmEmailAsync(user, code);

        if (!userResult.Succeeded || !roleResult.Succeeded || !claimResult.Succeeded || !confirmResult.Succeeded)
        {
          throw new InvalidOperationException("Failed to build user and roles");
        }
      }
    }

    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly InitializeIdentityOptions _options;
  }
}