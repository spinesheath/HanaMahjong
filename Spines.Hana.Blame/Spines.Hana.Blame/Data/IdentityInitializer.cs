// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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
      foreach (var roleName in RoleNames.All())
      {
        if (await _roleManager.RoleExistsAsync(roleName))
          continue;
        var role = new IdentityRole(roleName);
        await _roleManager.CreateAsync(role);
      }

      if (_options.RecreateAdminAccount)
      {
        var existingAdmins = await _userManager.GetUsersInRoleAsync(RoleNames.Admin);
        foreach (var existingAdmin in existingAdmins)
        {
          await _userManager.DeleteAsync(existingAdmin);
        }
      }

      var existingUser = await _userManager.FindByEmailAsync(_options.RootAdminEmail);
      if (existingUser != null)
      {
        if (!_options.RecreateAdminAccount)
          return;
        await _userManager.DeleteAsync(existingUser);
      }

      var user = new ApplicationUser
      {
        UserName = _options.RootAdminUserName,
        Email = _options.RootAdminEmail
      };

      var userResult = await _userManager.CreateAsync(user, _options.RootAdminPassword);
      var roleResult = await _userManager.AddToRoleAsync(user, RoleNames.Admin);
      var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
      var confirmResult = await _userManager.ConfirmEmailAsync(user, code);

      if (userResult.Succeeded && roleResult.Succeeded && confirmResult.Succeeded)
      {
        return;
      }
      throw new InvalidOperationException("Failed to build user and roles");
    }
    
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly InitializeIdentityOptions _options;
  }
}