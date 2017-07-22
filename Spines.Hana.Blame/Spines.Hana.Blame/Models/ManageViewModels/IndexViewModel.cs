// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Spines.Hana.Blame.Models.ManageViewModels
{
  public class IndexViewModel
  {
    public bool HasPassword { get; set; }

    public IList<UserLoginInfo> Logins { get; set; }

    public string PhoneNumber { get; set; }

    public bool TwoFactor { get; set; }

    public bool BrowserRemembered { get; set; }
  }
}